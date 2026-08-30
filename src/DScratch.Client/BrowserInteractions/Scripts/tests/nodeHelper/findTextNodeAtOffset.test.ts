import { expect, test } from 'vitest';
import * as domHelper from "../domHelper";
import * as nodeHelper from "../../nodeHelper";

test("returns expected node and relativeOffset for given parent and offset", async () => {
    domHelper.createEditorFixture();
    domHelper.insertText("Hello", {
        parentId: "p-1-1",
        id: "t-1"
    });

    const parent = document.querySelector<HTMLElement>("[data-dnode-id='t-1']")!;
    const absolutOffset = nodeHelper.findTextNodeAtOffset(parent, 1);

    expect(absolutOffset.node).toBe(parent.firstChild);
    expect(absolutOffset.relativeOffset).toBe(1);
});

test("returns expected node and relativeOffset for given parent and offset with multiple text nodes", async () => {
    domHelper.createEditorFixture();
    domHelper.insertText("Hello", {
        parentId: "p-1-1",
        id: "t-1"
    });

    const parent = document.querySelector<HTMLElement>("[data-dnode-id='t-1']")!;
    const secondTextNode = document.createTextNode("bye");
    parent.appendChild(secondTextNode);
    const absolutOffset = nodeHelper.findTextNodeAtOffset(parent, 7);

    expect(absolutOffset.node).toBe(secondTextNode);
    expect(absolutOffset.relativeOffset).toBe(2);
});

test.each([0, 1])("returns expected node and relativeOffset in split-part 1 (parentIndex %i)", async (parentIndex: number) => {
    domHelper.createEditorFixture();
    domHelper.createSplittedParagraph(1, "ps-1");
    domHelper.insertText("Hello", {
       parentId: "ps-1",
       id: "t-1",
       splitPart: 1 
    });
    domHelper.insertText(" World!", {
        parentId: "ps-1",
        id: "t-1",
        splitPart: 2
    });

    const parent = document.querySelectorAll<HTMLElement>("[data-dnode-id='t-1']");
    const absolutOffset = nodeHelper.findTextNodeAtOffset(parent[parentIndex], 2);

    expect(absolutOffset.node).toBe(parent[0].firstChild);
    expect(absolutOffset.relativeOffset).toBe(2);
});

test.each([0, 1])("returns expected node and relativeOffset in split-part 2 (parentIndex %i)", async (parentIndex: number) => {
    domHelper.createEditorFixture();
    domHelper.createSplittedParagraph(1, "ps-1");
    domHelper.insertText("Hello", {
        parentId: "ps-1",
        id: "t-1",
        splitPart: 1
    });
    domHelper.insertText(" World!", {
        parentId: "ps-1",
        id: "t-1",
        splitPart: 2
    });

    const parent = document.querySelectorAll<HTMLElement>("[data-dnode-id='t-1']");
    const absolutOffset = nodeHelper.findTextNodeAtOffset(parent[parentIndex], 7);

    expect(absolutOffset.node).toBe(parent[1].firstChild);
    expect(absolutOffset.relativeOffset).toBe(2);
});