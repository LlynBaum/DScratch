import {findTextNodeAtOffset, getAbsolutOffset, getElementFromNode, getNodeId} from "./nodeHelper";

interface SelectionSnapshot {
    selection: Selection | null;
    absolutAnchorOffset: number;
    anchorId: string;
    absolutFocusOffset: number | null;
    focusId: string | null;
}

interface SelectionInfo {
    anchorOffset: number;
    anchorNode: Node | null;
}

let snapshot: SelectionSnapshot | null = null;
let currentSelection: SelectionInfo | null = null;

export function snapshotSelection(offset: number, anchorId: string, endOffset: number, focusId: string | null) {
    snapshot = {
        selection: window.getSelection(),
        absolutAnchorOffset: offset,
        anchorId: anchorId,
        absolutFocusOffset: endOffset,
        focusId: focusId
    }
}

export function resetSnapshot() {
    snapshot = null;
}

export function saveSelection() {
    const selection = window.getSelection();
    if (!selection) {
        currentSelection = null;
        return;
    }
    
    currentSelection = {
        anchorOffset: selection.anchorOffset,
        anchorNode: selection.anchorNode
    };
}

export function setSelection(parentId: string, offset: number) {
    if (!currentSelection) {
        resetSnapshot();
        return;
    }
    
    const currentParent = getElementFromNode(currentSelection.anchorNode!);
    const currentParentId = getNodeId(currentParent);
    const currentOffset = getAbsolutOffset(currentParent, currentSelection.anchorNode!, currentSelection.anchorOffset);

    /*const currentFocusParent = currentSelection.focusNode && getElementFromNode(currentSelection.focusNode!);
    const currentFocusParentId = currentFocusParent && currentFocusParent.getAttribute("data-dnode-id");
    const currentFocusOffset = getAbsolutOffset(currentParent, currentSelection.focusNode!, currentSelection.focusOffset);*/
    
    if(!snapshot) {
        return;
    }
    
    const userMovedNatively =
        currentParentId !== snapshot.anchorId ||
        currentOffset !== snapshot.absolutAnchorOffset;
    
    /*
    currentFocusParentId !== (snapshot.endPath && snapshot.endPath[0]) ||
    currentFocusOffset !== snapshot.absolutEndOffset
    */
    
    if (userMovedNatively) {
        resetSnapshot();
        return;
    }

    setSelectionFrom(parentId, offset);
    resetSnapshot();
}

function setSelectionFrom(parentId: string, offset: number) {
    const element = document.querySelector<HTMLElement>(`[data-dnode-id="${parentId}"]`);
    if (!element) return;
    
    const { node, relativeOffset } = findTextNodeAtOffset(element, offset);

    const selection = window.getSelection();
    selection?.removeAllRanges();
    
    const range = document.createRange();
    
    if (node) {
        range.setStart(node, relativeOffset);
    } else {
        range.setStart(element, 0);
    }
    
    range.collapse(true);
    selection?.addRange(range)
}