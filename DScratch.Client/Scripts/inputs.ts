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

    const selection = getSelection();

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

    await bridgeReference?.invokeMethodAsync("OnKeyPressCallback", payload);
}

function getPath(element: Element): string[] {
    const result: string[] = [];
    let current = element.parentElement; // Event is fired on text node within p element

    while (current && !current.hasAttribute("contenteditable")) {
        result.push(current.getAttribute("data-path-id")!);
        current = current.parentElement;
    }

    return result;
}

function getAbsolutOffset(parent: Element, targetNode: Node, relativeOffset?: number) {
    if(!relativeOffset) {
        return 0;
    }

    const walker = document.createTreeWalker(parent, NodeFilter.SHOW_TEXT);

    let absolutOffset = 0;
    let currentNode = walker.nextNode();

    while (currentNode) {
        if(currentNode == targetNode) {
            absolutOffset += relativeOffset;
            break;
        }

        absolutOffset += currentNode.nodeValue?.length || 0;
        currentNode = walker.nextNode();
    }

    return absolutOffset;
}

function getElementFromNode(node: Node) {
    return  node.nodeType == Node.ELEMENT_NODE
        ? node as Element
        : node.parentElement?.closest("[data-path-id]")!;
}