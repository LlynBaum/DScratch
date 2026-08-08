import {getSelection, snapshotSelection} from "./selection";

const handledTypes = [
    "insertText",
    "insertParagraph",
    "deleteContentBackward",
    "deleteContentForward",
    "deleteWordBackward",
    "deleteWordForward"
];

export function registerInput() {
    window.editor.node?.addEventListener("beforeinput", async event => await handleInput(event));
}

async function handleInput(event: InputEvent) {
    if (!handledTypes.includes(event.inputType)) {
        return; // Let the browser handle unsupported inputs natively for now
    }
    
    event.preventDefault();
    if (isInvalidUserAction()) {
        return;
    }

    const selectionInfo = getSelection();
    const payload = {
        InputType: event.inputType,
        Data: event.data,
        Selection: selectionInfo
    };
    
    snapshotSelection(selectionInfo);
    
    try {
        await window.editor.bridgeReference?.invokeMethodAsync("OnKeyPressCallbackAsync", payload);
    } catch (e) {
        console.error(e, "Failed to send event with anchor ", selectionInfo.anchorId);
    }
}

function isInvalidUserAction() {
    const selection = window.getSelection();
    if (!selection) return false;
    
    return selection?.anchorNode === window.editor.node 
        || selection?.focusNode === window.editor.node;
}