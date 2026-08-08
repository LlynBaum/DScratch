import {findNode, findTextNodeAtOffset, getAbsolutOffset, getElementFromNode, getNodeId} from "./nodeHelper";

export type SelectionDirection = "none" | "forward" | "backward"

export interface SelectionInfo {
    direction: SelectionDirection;
    anchorId: string;
    anchorOffset: number;
    focusId: string | null;
    focusOffset: number | null;
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

export function registerSelection() {
    document.addEventListener("selectionchange", handleSelectionChange);
}

export function snapshotSelection(selectionInfo: SelectionInfo) {
    snapshot = {
        selection: window.getSelection(),
        absolutAnchorOffset: selectionInfo.anchorOffset,
        anchorId: selectionInfo.anchorId,
        absolutFocusOffset: selectionInfo.focusOffset,
        focusId: selectionInfo.focusId
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
        direction: selection?.direction as SelectionDirection ?? "none",
        anchorId: anchorId,
        anchorOffset: anchorOffset,
        focusId: focusId,
        focusOffset: focusOffset
    };
}

export function setSelectionSave(selection: SelectionInfo) {
    if (!currentSelection) {
        resetSnapshot();
        return;
    }
    
    const currentParent = getElementFromNode(currentSelection.anchorNode!);
    const currentParentId = getNodeId(currentParent);
    const currentOffset = getAbsolutOffset(currentParent, currentSelection.anchorNode!, currentSelection.anchorOffset);
    
    if(!snapshot) {
        setSelection(selection);
        resetSnapshot();
        return;
    }
    
    const userMovedNatively =
        currentParentId !== snapshot.anchorId ||
        currentOffset !== snapshot.absolutAnchorOffset;
    
    if (userMovedNatively) {
        resetSnapshot();
        return;
    }

    setSelection(selection);
    resetSnapshot();
}

export function setSelection(selectionInfo: SelectionInfo) {
    if (selectionInfo.direction === "none") {
        setCursorPosition(selectionInfo.anchorId, selectionInfo.anchorOffset);
    } else {
        setCursorSelection(selectionInfo);
    }
}

function setCursorPosition(parentId: string, offset: number) {
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
    selection?.addRange(range);
}

function setCursorSelection(selectionInfo: SelectionInfo) {
    const anchorElement = findNode(selectionInfo.anchorId);
    if (!anchorElement) return;

    const focusElement = findNode(selectionInfo.focusId!);
    if (!focusElement) return;
    
    const anchor = findTextNodeAtOffset(anchorElement, selectionInfo.anchorOffset);
    const focus = findTextNodeAtOffset(focusElement, selectionInfo.focusOffset!);

    const selection = window.getSelection();
    selection?.removeAllRanges();

    const range = document.createRange();

    const { start, end } = asStartEnd(anchor, focus);
    range.setStart(start.node!, start.relativeOffset);
    range.setEnd(end.node!, end.relativeOffset);
    selection?.addRange(range);
    
    function asStartEnd(anchor: { node: Text | null; relativeOffset: number }, focus: { node: Text | null; relativeOffset: number }) {
        return selectionInfo.direction === "forward" 
            ? { start: anchor, end: focus }
            : { start: focus, end: anchor };
    }
}

let timeout: any;
function handleSelectionChange() {
    clearTimeout(timeout);
    timeout = setTimeout(async () => {
        const selection = getSelection();
        await window.editor.bridgeReference?.invokeMethodAsync("OnSelectionChange", selection);
    }, 100);
}