import {getSelection, snapshotSelection} from "./selection";
import {metadataController} from "./metatada";

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

    const selectionInfo = getSelection();
    snapshotSelection(selectionInfo);
    
    const metadata = metadataController.getActive(selectionInfo.anchorId);
    
    const payload = {
        InputType: event.inputType,
        Data: event.data,
        Selection: selectionInfo,
        Metadata: [...metadata.fromSelection, ...metadata.fromId] // TODO: C# has to get that and use it if possible.
    };
    
    try {
        await bridgeReference?.invokeMethodAsync("OnKeyPressCallbackAsync", payload);
    } catch (e) {
        console.error(e, "Failed to send event with anchor ", selectionInfo.anchorId);
    }
    
    if (event.inputType !== "insertParagraph") {
        metadataController.discardOnSelectionChange();
        metadataController.discard(metadata.fromId);
    }
}

function isInvalidUserAction() {
    const selection = window.getSelection();
    if (!selection) return false;
    
    return selection?.anchorNode === window.editor.node 
        || selection?.focusNode === window.editor.node;
}