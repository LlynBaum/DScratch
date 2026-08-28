const NODE_ID_ATTRIBUTE = "data-dnode-id";

export function getAbsolutOffset(parent: Element, targetNode: Node, relativeOffset?: number) {
    // 0 is falsy, but a valid relative option, since the rest can be at a split paragraph. So have to explicitly only check null and undefined
    if(relativeOffset === undefined || relativeOffset === null) {
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
    
    if (!currentNode) {
        return 0;
    }
    
    const splitPartIndex = getSplitPartIndex(parent);
    if (splitPartIndex === "2") {
        const counterPart = getSplitCounterPart(parent);
        absolutOffset += counterPart?.textContent?.length ?? 0;
    }

    return absolutOffset;
}

export function getElementFromNode(node: Node): Element {
    return node.nodeType == Node.ELEMENT_NODE
        ? node as Element
        : node.parentElement?.closest("[data-dnode-id]")!;
}

export function findTextNodeAtOffset(parent: Element, offset: number): { node: Node | null, relativeOffset: number } {
    const splitPartIndex = getSplitPartIndex(parent);
    switch (splitPartIndex) {
        case "1": {
            const result = find(parent, offset);
            if (result.node) return result;
            let counterPart = getSplitCounterPart(parent);
            if (!counterPart) return result;
            return find(counterPart!, offset - (parent.textContent?.length ?? 0));
        }
        case "2": {
            let counterPart = getSplitCounterPart(parent);
            if (counterPart) {
                const result = find(counterPart, offset);
                if (result.node) return result;
            }
            return find(parent, offset - (counterPart?.textContent?.length ?? 0));
        }
        default:
            return find(parent, offset);
    }

    function find(targetParent: Element, offset: number) {
        const walker = document.createTreeWalker(targetParent, NodeFilter.SHOW_TEXT);

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
}

export function findNode(nodeId: string) {
    return document.querySelector<HTMLElement>(`[${NODE_ID_ATTRIBUTE}="${nodeId}"]`);
}

export function findNodeLast(nodeId: string) {
    const elements = document.querySelectorAll<HTMLElement>(`[${NODE_ID_ATTRIBUTE}="${nodeId}"]`);
    return elements[elements.length - 1];
}

export function findNodeAll(nodeId: string) {
    return document.querySelectorAll<HTMLElement>(`[${NODE_ID_ATTRIBUTE}="${nodeId}"]`);
}

export function getNodeId(element: Element) {
    return element.getAttribute(NODE_ID_ATTRIBUTE);
}

export function getSplitPartIndex(domElement: Element) {
    return domElement.closest("[data-split-part]")?.getAttribute("data-split-part") ?? null;
}

export function getSplitCounterPart(domElement: Element) {
    const splitElement = domElement.closest("[data-split-part]");
    const splitPart = splitElement?.getAttribute("data-split-part");
    const splitCounterPart = splitPart === "1" ? "2" : "1";
    
    const nodeId = getNodeId(domElement);
    return document.querySelector(`[data-split-part="${splitCounterPart}"][${NODE_ID_ATTRIBUTE}="${nodeId}"], [data-split-part="${splitCounterPart}"] [${NODE_ID_ATTRIBUTE}="${nodeId}"]`);
}