import {schema} from "prosemirror-schema-basic"
import {EditorState} from "prosemirror-state"
import {EditorView} from "prosemirror-view"

window.initProseMirror = (elementId) => {
    const target = document.getElementById(elementId);
    const state = EditorState.create({schema});

    window.view = new EditorView(target, { state });
    console.log("ProseMirror initialized!");
};

window.getEditorContent = () => {
    return JSON.stringify(window.view.state.doc.toJSON());
};