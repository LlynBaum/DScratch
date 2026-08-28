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

let lastEditorSelection: SelectionInfo | null = null;
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

export function getEditorSelection(): SelectionInfo | null {
    const selection = window.getSelection();
    if (!selection) return null;

    const anchorElement = selection.anchorNode && getElementFromNode(selection.anchorNode);
    const focusElement = selection.focusNode && getElementFromNode(selection.focusNode);

    if (!anchorElement) return lastEditorSelection;

    const anchorId = getNodeId(anchorElement);
    const anchorOffset = getAbsolutOffset(anchorElement, selection?.anchorNode!, selection?.anchorOffset);
    
    if (!anchorId) return lastEditorSelection;

    const focusId = focusElement ? getNodeId(focusElement) : anchorId;
    const focusOffset = focusElement ? getAbsolutOffset(focusElement, selection?.focusNode!, selection?.focusOffset) : anchorOffset;
    
    return {
        direction: selection?.direction as SelectionDirection ?? "none",
        anchorId: anchorId,
        anchorOffset: anchorOffset,
        focusId: focusId,
        focusOffset: focusOffset
    };
}

export function restoreEditorSelection() {
    if (!lastEditorSelection) return;
    setSelection(lastEditorSelection);
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
    lastEditorSelection = selectionInfo;
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
    
    const targetNode = node ?? element;
    const targetOffset = node ? relativeOffset : 0;

    const range = document.createRange();
    range.setStart(targetNode, targetOffset);
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

    const anchorNode = anchor.node ?? anchorElement;
    const anchorOffset = anchor.node ? anchor.relativeOffset : 0;
    const focusNode = focus.node ?? focusElement;
    const focusOffset = focus.node ? focus.relativeOffset : 0;

    selection?.setBaseAndExtent(anchorNode, anchorOffset, focusNode, focusOffset);
}

let timeout: any;
function handleSelectionChange() {
    const selection = getEditorSelection();
    if (selection) {
        lastEditorSelection = selection;
    }
    
    clearTimeout(timeout);
    timeout = setTimeout(async () => {
        await window.editor.bridgeReference?.invokeMethodAsync("OnSelectionChange", lastEditorSelection);
    }, 100);
}

export function showFakeSelection() {
    if (!CSS.highlights) return;
    if (!lastEditorSelection || lastEditorSelection.direction === "none") return; // TODO: when selection direction none idk what to do for now.
    
    const anchorElement = findNode(lastEditorSelection.anchorId)!;
    const focusElement = findNode(lastEditorSelection.focusId!)!;
    const anchor = findTextNodeAtOffset(anchorElement, lastEditorSelection.anchorOffset);
    const focus = findTextNodeAtOffset(focusElement, lastEditorSelection.focusOffset!);
    

    const { start, end } = lastEditorSelection.direction === "forward"
        ? { start: anchor, end: focus }
            : { start: focus, end: anchor };
    
    if (!start.node || !end.node) return;

    const range = new Range();
    range.setStart(start.node, start.relativeOffset);
    range.setEnd(end.node, end.relativeOffset);
    
    const highlight = new Highlight(range);
    CSS.highlights.clear();
    CSS.highlights.set("ds-editor-selection", highlight);
}

export function clearFakeSelection() {
    if (!CSS.highlights) return;
    CSS.highlights.clear();
}