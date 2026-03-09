import {schema} from "prosemirror-schema-basic" // TODO: make custom schema
import {EditorState} from "prosemirror-state"
import {EditorView} from "prosemirror-view"
import {toggleMark, setBlockType, wrapIn} from "prosemirror-commands"

window.initProseMirror = (elementId) => {
    const target = document.getElementById(elementId);
    const state = EditorState.create({schema});

    window.editorView = new EditorView(target, { state });
    console.log("ProseMirror initialized!");
};

window.applyCSharpUpdate = (update) => {
    switch (update.type){
        case "marks":
            updateMarks(update, window.editorView);
            break;
    }
}

function updateMarks(update, editorView) {
    editorView.focus();
    let command = undefined;
    if(update.method === "toggle"){
        command = toggleMark(schema.marks[update.markName]);
    }

    if(command){
        command(editorView.state, editorView.dispatch, editorView);
    }
}