import schema from "./schema";
import {EditorState} from "prosemirror-state"
import {EditorView} from "prosemirror-view"
import {toggleMark, setBlockType, wrapIn} from "prosemirror-commands"

interface Update {
    type: string;
    method: string;
    [key: string]: string;
}

declare global {
    interface Window {
        initProseMirror?: (elementId: string) => void;
        editorView: EditorView;
        applyCSharpUpdate?: (update: Update) => void;
    }
}

window.initProseMirror = (elementId: string) => {
    const target = document.getElementById(elementId);
    const state = EditorState.create({schema});

    window.editorView = new EditorView(target, { state });
    console.log("ProseMirror initialized!");
};

window.applyCSharpUpdate = (update: Update) => {
    switch (update.type){
        case "marks":
            updateMarks(update, window.editorView);
            break;
    }
}

function updateMarks(update: Update, editorView: EditorView) {
    editorView.focus();
    let command = undefined;
    if(update.method === "toggle"){
        command = toggleMark(schema.marks[update.markName]);
    }

    if(command){
        command(editorView.state, editorView.dispatch, editorView);
    }
}