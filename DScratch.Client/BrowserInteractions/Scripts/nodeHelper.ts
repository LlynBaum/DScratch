const NODE_ID_ATTRIBUTE = "data-dnode-id";

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

export function findTextNodeAtOffset(parent: Element, offset: number){
    const walker = document.createTreeWalker(parent, NodeFilter.SHOW_TEXT);

    let currentOffset = 0;
    let currentNode = walker.nextNode() as Text | null;

    while (currentNode) {
        const nodeLength = currentNode.textContent?.length || 0;

        if (currentOffset + nodeLength >= offset) {
            return { node: currentNode, relativeOffset: offset - currentOffset };
        }

        currentOffset += nodeLength;
        currentNode = walker.nextNode() as Text | null;
    }
    return { node: null, relativeOffset: 0 };
}

export function getNodeId(element: Element) {
    return element.getAttribute(NODE_ID_ATTRIBUTE);
}