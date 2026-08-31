import { expect, test, beforeEach } from 'vitest';
import * as domHelper from "../domHelper";
import * as selection from "../../selection";

function getDomSelection() {
    const sel = window.getSelection();
    if (!sel || sel.rangeCount === 0) return null;
    return {
        anchorNode: sel.anchorNode,
        anchorOffset: sel.anchorOffset,
        focusNode: sel.focusNode,
        focusOffset: sel.focusOffset,
        isCollapsed: sel.isCollapsed,
    };
}

function setNativeCursor(node: Node, offset: number) {
    const sel = window.getSelection()!;
    const range = document.createRange();
    range.setStart(node, offset);
    range.collapse(true);
    sel.removeAllRanges();
    sel.addRange(range);
}

beforeEach(() => {
    window.getSelection()?.removeAllRanges();
});

test("sets selection to node when no native cursor movement", () => {
    domHelper.createEditorFixture({ paragraphsPerPage: 2 });
    selection.snapshotSelection({
        direction: "none",
        anchorId: "p-1-1",
        anchorOffset: 0,
        focusId: "p-1-1",
        focusOffset: 0
    });

    const node = document.querySelector("[data-dnode-id='p-1-1']")!;
    setNativeCursor(node, 0);
    
    selection.saveSelection();
    selection.setSelectionSave({
        direction: "none",
        anchorId: "p-1-2",
        anchorOffset: 0,
        focusId: "p-1-2",
        focusOffset: 0
    });
    
    const targetNode = document.querySelector("[data-dnode-id='p-1-2']")?.firstChild!;
    const sel = getDomSelection();
    expect(sel).not.toBeNull();
    expect(sel).toEqual({
        isCollapsed: true,
        anchorNode: targetNode,
        anchorOffset: 0,
        focusNode: targetNode,
        focusOffset: 0
    });
});

test("does nothing when cursor was moved natively to other node", () => {
    domHelper.createEditorFixture({ paragraphsPerPage: 3 });
    selection.snapshotSelection({
        direction: "none",
        anchorId: "p-1-1",
        anchorOffset: 0,
        focusId: "p-1-1",
        focusOffset: 0
    });
    
    const node = document.querySelector("[data-dnode-id='p-1-2']")!;
    setNativeCursor(node, 0);
    
    selection.saveSelection();
    selection.setSelectionSave({
        direction: "none",
        anchorId: "p-1-3",
        anchorOffset: 0,
        focusId: "p-1-3",
        focusOffset: 0
    });
    
    const sel = getDomSelection();
    expect(sel).not.toBeNull();
    expect(sel).toEqual({
        isCollapsed: true,
        anchorNode: node,
        anchorOffset: 0,
        focusNode: node,
        focusOffset: 0
    });
});