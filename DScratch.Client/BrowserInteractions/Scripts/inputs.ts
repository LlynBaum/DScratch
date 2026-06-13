import {getSelection, snapshotSelection} from "./selection";
import {getAbsolutOffset, getElementFromNode, getNodeId} from "./nodeHelper";

const handledTypes = [
    "insertText",
    "insertParagraph",
    "deleteContentBackward",
    "deleteContentForward",
    "deleteWordBackward",
    "deleteWordForward"
];

export async function handleInput(event: InputEvent, bridgeReference: any) {
    if (!handledTypes.includes(event.inputType)) {
        return; // Let the browser handle unsupported inputs natively for now
    }
    
    event.preventDefault();
    if (isInvalidUserAction()) {
        return;
    }

    const payload = getSelection();
    snapshotSelection(payload);
    
    try {
        await bridgeReference?.invokeMethodAsync("OnKeyPressCallbackAsync", payload);
    } catch (e) {
        console.error(e, "Failed to send event with anchor ", payload.AnchorId);
    }
}

function isInvalidUserAction() {
    const selection = window.getSelection();
    if (!selection) return false;
    
    return selection?.anchorNode === window.editor.node 
        || selection?.focusNode === window.editor.node;
}