import { expect, test, beforeEach } from 'vitest';
import * as domHelper from "../domHelper";
import * as selection from "../../selection";

function setNativeCursor(node: Node, offset: number) {
    const sel = window.getSelection()!;
    const range = document.createRange();
    range.setStart(node, offset);
    range.collapse(true);
    sel.removeAllRanges();
    sel.addRange(range);
}

function setNativeRange(anchorNode: Node, anchorOffset: number, focusNode: Node, focusOffset: number) {
    const sel = window.getSelection()!;
    sel.removeAllRanges();
    sel.setBaseAndExtent(anchorNode, anchorOffset, focusNode, focusOffset);
}

beforeEach(() => {
    window.getSelection()?.removeAllRanges();
});

test('returns collapsed selection info for cursor inside paragraph', () => {
    domHelper.createEditorFixture();

    const p = document.querySelector<HTMLElement>("[data-dnode-id='p-1-1']")!;

    setNativeCursor(p, 0);

    const sel = selection.getEditorSelection();
    expect(sel).toEqual({
        direction: "none",
        anchorId: "p-1-1",
        anchorOffset: 0,
        focusId: "p-1-1",
        focusOffset: 0,
    });
});

test('returns collapsed selection info for cursor inside text', () => {
    domHelper.createEditorFixture();
    domHelper.insertText("hello", {
        parentId: "p-1-1", 
        id: "t-1"
    });
    
    const spanElem = document.querySelector<HTMLElement>("[data-dnode-id='t-1']")!;
    const textNode = spanElem.firstChild!;

    setNativeCursor(textNode, 4);

    const sel = selection.getEditorSelection();
    expect(sel).toEqual({
        direction: "none",
        anchorId: "t-1",
        anchorOffset: 4,
        focusId: "t-1",
        focusOffset: 4,
    });
});

test('returns forward selection info when selecting text within paragraph', () => {
    domHelper.createEditorFixture();
    domHelper.insertText("hello", {
        parentId: "p-1-1",
        id: "t-1"
    });

    const spanElem = document.querySelector<HTMLElement>("[data-dnode-id='t-1']")!;
    const textNode = spanElem.firstChild!;

    setNativeRange(textNode, 2, textNode, 4);

    const sel = selection.getEditorSelection();
    expect(sel).toEqual({
        direction: "forward",
        anchorId: "t-1",
        anchorOffset: 2,
        focusId: "t-1",
        focusOffset: 4,
    });
});

test('returns forward selection info when selecting text over two paragraphs', () => {
    domHelper.createEditorFixture({ pageCount: 1, paragraphsPerPage: 2 });
    domHelper.insertText("hello", {
        parentId: "p-1-1",
        id: "t-1"
    });
    domHelper.insertText("bye", {
        parentId: "p-1-2",
        id: "t-2"
    });

    const spanElem1 = document.querySelector<HTMLElement>("[data-dnode-id='t-1']")!;
    const textNode1 = spanElem1.firstChild!;

    const spanElem2 = document.querySelector<HTMLElement>("[data-dnode-id='t-2']")!;
    const textNode2 = spanElem2.firstChild!;

    setNativeRange(textNode1, 2, textNode2, 2);

    const sel = selection.getEditorSelection();
    expect(sel).toEqual({
        direction: "forward",
        anchorId: "t-1",
        anchorOffset: 2,
        focusId: "t-2",
        focusOffset: 2,
    });
});

test('calculates absolute offset across split parts when cursor is on page 2 (split part 2)', () => {
    domHelper.createEditorFixture();
    domHelper.createSplittedParagraph(1, "ps-1");
    domHelper.insertText("Hello ", {
        parentId: "ps-1",
        id: "t-1",
        splitPart: 1
    });
    domHelper.insertText("World!", {
        parentId: "ps-1",
        id: "t-1",
        splitPart: 2
    });

    const part2 = document.querySelector<HTMLElement>("[data-split-part='2'] [data-dnode-id='t-1']")!;
    const textNodePart2 = part2.firstChild!;

    setNativeCursor(textNodePart2, 3);

    const sel = selection.getEditorSelection();
    expect(sel).toEqual({
        direction: "none",
        anchorId: "t-1",
        anchorOffset: 9,
        focusId: "t-1",
        focusOffset: 9,
    });
});