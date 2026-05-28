export function getAbsolutOffset(parent: Element, targetNode: Node, relativeOffset?: number) {
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

export function getElementFromNode(node: Node): Element {
    return node.nodeType == Node.ELEMENT_NODE
        ? node as Element
        : node.parentElement?.closest("[data-dnode-id]")!;
}