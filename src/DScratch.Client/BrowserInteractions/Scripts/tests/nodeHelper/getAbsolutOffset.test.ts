import { expect, test } from 'vitest';
import * as domHelper from "../domHelper";
import * as nodeHelper from "../../nodeHelper";

test("returns absolute offset relative to parent", async () => {
    domHelper.createEditorFixture();
    domHelper.insertText("p-1-1", "t-1", "hello");
    
    const parent = document.querySelector<HTMLElement>("[data-dnode-id='t-1']")!;
    const absolutOffset = nodeHelper.getAbsolutOffset(parent, parent.firstChild!, 1);
    
    expect(absolutOffset).toBe(1);
});

test("returns absolute offset relative to parent with text multiple text nodes", async () => {
    domHelper.createEditorFixture();
    domHelper.insertText("p-1-1", "t-1", "hello");

    const parent = document.querySelector<HTMLElement>("[data-dnode-id='t-1']")!;
    const secondTextNode = document.createTextNode("bye");
    parent.appendChild(secondTextNode);
    const absolutOffset = nodeHelper.getAbsolutOffset(parent, secondTextNode, 2);

    expect(absolutOffset).toBe(7);
});

test("returns absolute offset relative to split parent in split-part 1", async () => {
    domHelper.createEditorFixture();
    domHelper.createSplittedParagraph(1, "ps-1");

    const parent = document.querySelectorAll<HTMLElement>("[data-dnode-id='ps-1-text']")[0];
    const absolutOffset = nodeHelper.getAbsolutOffset(parent, parent.firstChild!, 2);

    expect(absolutOffset).toBe(2);
});

test("returns absolute offset relative to split parent in split-part 2", async () => {
    domHelper.createEditorFixture();
    domHelper.createSplittedParagraph(1, "ps-1");

    const parent = document.querySelectorAll<HTMLElement>("[data-dnode-id='ps-1-text']")[1];
    const absolutOffset = nodeHelper.getAbsolutOffset(parent, parent.firstChild!, 2);

    expect(absolutOffset).toBe(219);
});