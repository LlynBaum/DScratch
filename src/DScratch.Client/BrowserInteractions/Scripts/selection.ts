import {findNode, findTextNodeAtOffset, getAbsolutOffset, getElementFromNode, getNodeId} from "./nodeHelper";

export type SelectionDirection = "none" | "forward" | "backward"

export interface SelectionInfo {
    Direction: SelectionDirection;
    AnchorId: string;
    AnchorOffset: number;
    FocusId: string | null;
    FocusOffset: number | null;
}

interface SelectionSnapshot {
    selection: Selection | null;
    absolutAnchorOffset: number;
    anchorId: string;
    absolutFocusOffset: number | null;
    focusId: string | null;
}

interface CurrentSelectionInfo {
    anchorOffset: number;
    anchorNode: Node | null;
}

let snapshot: SelectionSnapshot | null = null;
let currentSelection: CurrentSelectionInfo | null = null;

export function snapshotSelection(selectionInfo: SelectionInfo) {
    snapshot = {
        selection: window.getSelection(),
        absolutAnchorOffset: selectionInfo.AnchorOffset,
        anchorId: selectionInfo.AnchorId,
        absolutFocusOffset: selectionInfo.FocusOffset,
        focusId: selectionInfo.FocusId
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

export function getSelection(): SelectionInfo {
    const selection = window.getSelection();

    const anchorElement = getElementFromNode(selection?.anchorNode!);
    const focusElement = getElementFromNode(selection?.focusNode!);

    const anchorId = getNodeId(anchorElement)!;
    const anchorOffset = getAbsolutOffset(anchorElement, selection?.anchorNode!, selection?.anchorOffset);
    const focusId = getNodeId(focusElement);
    const focusOffset = getAbsolutOffset(focusElement, selection?.focusNode!, selection?.focusOffset);
    
    return {
        Direction: selection?.direction as SelectionDirection ?? "none",
        AnchorId: anchorId,
        AnchorOffset: anchorOffset,
        FocusId: focusId,
        FocusOffset: focusOffset
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
        setSelectionFrom(parentId, offset);
        resetSnapshot();
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
    const element = findNode(parentId);
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