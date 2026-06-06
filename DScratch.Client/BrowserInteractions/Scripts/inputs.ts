import { snapshotSelection } from "./selection";
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

    const selection = window.getSelection();

    const anchorElement = getElementFromNode(selection?.anchorNode!);
    const focusElement = getElementFromNode(selection?.focusNode!);

    const anchorId = getNodeId(anchorElement)!;
    const anchorOffset = getAbsolutOffset(anchorElement, selection?.anchorNode!, selection?.anchorOffset);
    const focusId = getNodeId(focusElement);
    const focusOffset = getAbsolutOffset(focusElement, selection?.focusNode!, selection?.focusOffset);

    const payload = {
        InputType: event.inputType,
        Data: event.data,
        Selection: {
            Direction: selection?.direction,
            AnchorId: anchorId,
            AnchorOffset: anchorOffset,
            FocusId: focusId,
            FocusOffset: focusOffset
        }
    }

    snapshotSelection(anchorOffset, anchorId, focusOffset, focusId);
    await bridgeReference?.invokeMethodAsync("OnKeyPressCallbackAsync", payload);
}