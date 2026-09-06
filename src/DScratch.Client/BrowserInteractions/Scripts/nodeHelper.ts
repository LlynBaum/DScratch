const NODE_ID_ATTRIBUTE = "data-dnode-id";

export function getAbsolutOffset(parent: Element, targetNode: Node, relativeOffset?: number) {
    // 0 is falsy, but a valid relative option, since the rest can be at a split paragraph. So have to explicitly only check null and undefined
    if(relativeOffset === undefined || relativeOffset === null) {
        return 0;
    }

    const nodes = getAllNodes(parent);

    const targetElement = targetNode.nodeType === Node.ELEMENT_NODE 
        ? targetNode as Element 
        : targetNode.parentElement!;
    
    const partIndex = getSplitPartIndex(targetElement);

    const relativeParent = nodes.find(n => getSplitPartIndex(n) === partIndex) ?? parent;
    let absolutOffset = findRelativeOffset(relativeParent);
    
    const previousNodes = nodes.filter(n => {
        const index = getSplitPartIndex(n);
        return !!index && !!partIndex && index < partIndex;
    });

    previousNodes.forEach(n => absolutOffset += n.textContent.length);
    return absolutOffset;
    
    function findRelativeOffset(parentElement: Element) {
        const walker = document.createTreeWalker(parentElement, NodeFilter.SHOW_TEXT);

        let absolutOffset = 0;
        let currentNode = walker.nextNode();

        while (currentNode) {
            if(currentNode == targetNode) {
                absolutOffset += relativeOffset!;
                break;
            }

            absolutOffset += currentNode.nodeValue?.length || 0;
            currentNode = walker.nextNode();
        }

        return !currentNode ? 0 : absolutOffset;
    }
}

export function getElementFromNode(node: Node): Element {
    return node.nodeType == Node.ELEMENT_NODE
        ? node as Element
        : node.parentElement?.closest("[data-dnode-id]")!;
}

export function findTextNodeAtOffset(parent: Element, offset: number): { node: Node | null, relativeOffset: number } {
    const nodes = getAllNodes(parent);
    
    let remainingOffset = offset;
    for (const node of nodes) {
        const contentLength = node.textContent.length;
        
        if (contentLength < remainingOffset) {
            remainingOffset -= contentLength;
            continue;
        }
        
        const result = find(node, remainingOffset);
        if (result.node) {
            return result;
        }

        remainingOffset -= contentLength;
    }
    
    return {
        node: null,
        relativeOffset: 0
    };

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
    return findNodeIn(window.editor.node!, nodeId);
}

export function findNodeIn(parent: Element, nodeId: string) {
    return parent.querySelector<HTMLElement>(`[${NODE_ID_ATTRIBUTE}="${nodeId}"]`);
    
}

export function findLastNode(nodeId: string) {
    const elements = document.querySelectorAll<HTMLElement>(`[${NODE_ID_ATTRIBUTE}="${nodeId}"]`);
    return elements[elements.length - 1];
}

export function findLastNodeWithSibling(nodeId: string, siblingId: string) {
    const elements = document.querySelectorAll<HTMLElement>(`[${NODE_ID_ATTRIBUTE}="${nodeId}"]:has([${NODE_ID_ATTRIBUTE}="${siblingId}"])`);
    return elements[elements.length - 1];
}

export function findAllNode(nodeId: string) {
    return document.querySelectorAll<HTMLElement>(`[${NODE_ID_ATTRIBUTE}="${nodeId}"]`);
}

export function getNodeId(element: Element) {
    return element.getAttribute(NODE_ID_ATTRIBUTE);
}

export function getSplitPartIndex(node: Node) {
    const domElement = node.nodeType === Node.ELEMENT_NODE ? node as Element : node.parentElement!;
    const result = domElement.closest("[data-split-part]")?.getAttribute("data-split-part") ?? null;
    return result ? Number(result) : null;
}

export function getAllNodes(domElement: Element) {
    const nodeId = getNodeId(domElement);
    return [...document.querySelectorAll<HTMLElement>(`[${NODE_ID_ATTRIBUTE}="${nodeId}"]`)];
}