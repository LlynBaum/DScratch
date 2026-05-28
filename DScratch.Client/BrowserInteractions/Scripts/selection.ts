import {getAbsolutOffset, getElementFromNode} from "./nodeHelper";

interface SelectionSnapshot {
    selection: Selection | null;
    absolutOffset: number;
    path: string[];
    absolutEndOffset: number | null;
    endPath: string[] | null;
}

let snapshot: SelectionSnapshot | null = null;

export function snapshotSelection(offset: number, path: string[], endOffset: number, endPath: string[]) {
    snapshot = {
        selection: window.getSelection(),
        absolutOffset: offset,
        path: path,
        absolutEndOffset: endOffset,
        endPath: endPath
    }
}

export function resetSnapshot() {
    snapshot = null;
}

export function setSelection(parentId: string, offset: number, currentSelection: Selection | null) {
    if (!currentSelection) {
        resetSnapshot();
        return;
    }
    
    const currentParent = getElementFromNode(currentSelection.anchorNode!);
    const currentParentId = currentParent.getAttribute("data-dnode-id");
    const currentOffset = getAbsolutOffset(currentParent, currentSelection.anchorNode!, currentSelection.anchorOffset);

    /*const currentFocusParent = currentSelection.focusNode && getElementFromNode(currentSelection.focusNode!);
    const currentFocusParentId = currentFocusParent && currentFocusParent.getAttribute("data-dnode-id");
    const currentFocusOffset = getAbsolutOffset(currentParent, currentSelection.focusNode!, currentSelection.focusOffset);*/
    
    if(!snapshot) {
        return;
    }
    
    const userMovedNatively =
        currentParentId !== snapshot.path[0] ||
        currentOffset !== snapshot.absolutOffset;
    
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
    const node = document.querySelector<HTMLElement>(`[data-dnode-id="${parentId}"]`) as Node;

    const selection = window.getSelection();
    selection?.removeAllRanges();

    const range = document.createRange();
    range.setStart(node, offset);
    range.collapse(true);
    selection?.addRange(range)
}