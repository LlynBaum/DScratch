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
    const result = nodeHelper.findTextNodeAtOffset(parent, 1);

    expect(result.node).toBe(parent.firstChild);
    expect(result.relativeOffset).toBe(1);
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
    const result = nodeHelper.findTextNodeAtOffset(parent, 7);

    expect(result.node).toBe(secondTextNode);
    expect(result.relativeOffset).toBe(2);
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
    const result = nodeHelper.findTextNodeAtOffset(parent[parentIndex], 2);

    expect(result.node).toBe(parent[0].firstChild);
    expect(result.relativeOffset).toBe(2);
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
    const result = nodeHelper.findTextNodeAtOffset(parent[parentIndex], 7);

    expect(result.node).toBe(parent[1].firstChild);
    expect(result.relativeOffset).toBe(2);
});

test.each([0, 1, 2])("returns expected node and relativeOffset in split-part 3 (parentIndex %i)", async (parentIndex: number) => {
    domHelper.createEditorFixture();
    domHelper.createSplittedParagraph(1, "ps-1", 2);
    domHelper.insertText("Hello", {
        parentId: "ps-1",
        id: "t-1",
        splitPart: 1
    });
    domHelper.insertText("new", {
        parentId: "ps-1",
        id: "t-1",
        splitPart: 2
    });
    domHelper.insertText("World!", {
        parentId: "ps-1",
        id: "t-1",
        splitPart: 3
    });

    const parent = document.querySelectorAll<HTMLElement>("[data-dnode-id='t-1']");
    const result = nodeHelper.findTextNodeAtOffset(parent[parentIndex], 10);

    expect(result.node).toBe(parent[2].firstChild);
    expect(result.relativeOffset).toBe(2);
});