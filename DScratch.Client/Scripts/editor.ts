import {schema} from "./schema";
import {EditorState} from "prosemirror-state"
import {EditorView} from "prosemirror-view"
import {toggleMark, setBlockType, wrapIn} from "prosemirror-commands"
import {Attrs, Slice} from "prosemirror-model";
import {ReplaceStep, Transform} from "prosemirror-transform";

interface Update {
    readonly type: string;
    readonly name: string;
    readonly attrs?: Attrs | null;
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
    console.info("ProseMirror initialized!");
};

window.applyCSharpUpdate = (update: Update) => {
    window.editorView.focus();
    switch (update.type){
        case "insertNode":
            insertNode(update, window.editorView);
            break;
        case "setBlockType":
            updateNodeType(update, window.editorView);
            break;
        case "toggleMark":
            updateMarks(update, window.editorView);
            break;
        case "wrapIn":
            updateWrap(update, window.editorView);
            break;
            
    }
}

function insertNode(update: Update, editorView: EditorView){
    const tr = new Transform(editorView.state.doc);
    const node = schema.nodes[update.name].create();
    
    if(editorView.state.selection.from != editorView.state.selection.to){ // TODO: does this basically replace the content with the thing that should be inserted?
        tr.delete(editorView.state.selection.from, editorView.state.selection.to);
    }
    tr.insert(editorView.state.selection.from, node);
}

function updateNodeType(update: Update, editorView: EditorView){
    const command = setBlockType(schema.nodes[update.name], update.attrs);
    command(editorView.state, editorView.dispatch, editorView);
}

function updateMarks(update: Update, editorView: EditorView) {
    const command = toggleMark(schema.marks[update.name], update.attrs);
    command(editorView.state, editorView.dispatch, editorView);
}

function updateWrap(update: Update, editorView: EditorView) {
    const command = wrapIn(schema.nodes[update.name], update.attrs);
    command(editorView.state, editorView.dispatch, editorView);
}