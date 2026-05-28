import { snapshotSelection } from "./selection";
import { getAbsolutOffset, getElementFromNode } from "./nodeHelper";

const handledTypes = [
    "insertText",
    "insertParagraph",
    "deleteContentBackward",
    "deleteContentForward"
];

export async function handleInput(event: InputEvent, bridgeReference: any) {
    if (!handledTypes.includes(event.inputType)) {
        return; // Let the browser handle unsupported inputs natively for now
    }

    event.preventDefault();

    const selection = window.getSelection();

    const anchorElement = getElementFromNode(selection?.anchorNode!);
    const focusElement = getElementFromNode(selection?.focusNode!);

    const path = getPath(anchorElement);
    const offset = getAbsolutOffset(anchorElement, selection?.anchorNode!, selection?.anchorOffset);
    const endPath = getPath(focusElement);
    const endOffset = getAbsolutOffset(focusElement, selection?.focusNode!, selection?.focusOffset);

    const payload = {
        InputType: event.inputType,
        Data: event.data,
        Path: path,
        Selection: {
            Offset: offset,
            Direction: selection?.direction,
            End: endPath,
            EndOffset: endOffset
        }
    }

    snapshotSelection(offset, path, endOffset, endPath);
    await bridgeReference?.invokeMethodAsync("OnKeyPressCallbackAsync", payload);
}

function getPath(element: Element): string[] {
    const result: string[] = [];
    let current: Element | null = element;

    while (current && current.hasAttribute("data-dnode-id")) {
        result.push(current.getAttribute("data-dnode-id")!);
        current = current.parentElement;
    }

    return result;
}