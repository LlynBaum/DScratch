// @ts-ignore / rider seems to hate that import, but it is actually the correct import suggested by vitest
import { page } from 'vitest/browser';
import { expect, test, beforeEach, vi } from 'vitest';
import * as domHelper from "../domHelper";
import * as transaction from "../../renderEngine/transaction";
import * as paging from "../../renderEngine/paging";
import {StepType, UpdateMarksStep} from "../../renderEngine/transaction";

beforeEach(() => { vi.clearAllMocks(); });
vi.mock('../../renderEngine/paging', () => ({
    update: vi.fn(),
}));

test("adds all marks to all nodes", async () => {
    domHelper.createEditorFixture();
    domHelper.createSplittedParagraph(1, "pt-1");
    
    domHelper.insertText("", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 1
    });
    domHelper.insertText("", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 2
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.updateMarks,
                nodeId: "t-1",
                marks: {
                    "font-weight": "bold",
                    "font-style": "italic"
                }
            } as UpdateMarksStep
        ]
    });

    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveStyle(`
        font-weight: bold;
        font-style: italic;
    `);
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveStyle(`
        font-weight: bold;
        font-style: italic;
    `);
    
    const el = document.querySelectorAll<HTMLElement>("[data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([...el]);
});

test("adds missing marks to all nodes", async () => {
    domHelper.createEditorFixture();
    domHelper.createSplittedParagraph(1, "pt-1");

    domHelper.insertText("", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 1
    });
    domHelper.insertText("", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 2
    });
    
    const el = document.querySelectorAll<HTMLElement>("[data-dnode-id='t-1']");
    el.forEach(e => e.style.fontStyle = "italic");

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.updateMarks,
                nodeId: "t-1",
                marks: {
                    "font-weight": "bold",
                    "font-style": "italic"
                }
            } as UpdateMarksStep
        ]
    });

    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveStyle(`
        font-weight: bold;
        font-style: italic;
    `);
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveStyle(`
        font-weight: bold;
        font-style: italic;
    `);

    expect(paging.update).toHaveBeenCalledExactlyOnceWith([...el]);
});

test("removes old marks from all nodes", async () => {
    domHelper.createEditorFixture();
    domHelper.createSplittedParagraph(1, "pt-1");

    domHelper.insertText("", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 1
    });
    domHelper.insertText("", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 2
    });

    const el = document.querySelectorAll<HTMLElement>("[data-dnode-id='t-1']");
    el.forEach(e => e.style.fontStyle = "italic");

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.updateMarks,
                nodeId: "t-1",
                marks: {
                    "font-weight": "bold"
                }
            } as UpdateMarksStep
        ]
    });

    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveStyle(`
        font-weight: bold;
    `);
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveStyle(`
        font-weight: bold;
    `);

    expect(paging.update).toHaveBeenCalledExactlyOnceWith([...el]);
});

test("removes all marks to all nodes", async () => {
    domHelper.createEditorFixture();
    domHelper.createSplittedParagraph(1, "pt-1");

    domHelper.insertText("", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 1
    });
    domHelper.insertText("", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 2
    });

    const el = document.querySelectorAll<HTMLElement>("[data-dnode-id='t-1']");
    el.forEach(e => e.style.fontStyle = "italic");

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.updateMarks,
                nodeId: "t-1",
                marks: { }
            } as UpdateMarksStep
        ]
    });

    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveAttribute("style", "");
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveAttribute("style", "");

    expect(paging.update).toHaveBeenCalledExactlyOnceWith([...el]);
});

test("updates all marks on all nodes", async () => {
    domHelper.createEditorFixture();
    domHelper.createSplittedParagraph(1, "pt-1");

    domHelper.insertText("", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 1
    });
    domHelper.insertText("", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 2
    });

    const el = document.querySelectorAll<HTMLElement>("[data-dnode-id='t-1']");
    el.forEach(e => e.style.fontStyle = "italic");

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.updateMarks,
                nodeId: "t-1",
                marks: { 
                    "font-style": "normal"
                }
            } as UpdateMarksStep
        ]
    });

    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveStyle(`
        font-style: normal;
    `);
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveStyle(`
        font-style: normal;
    `);

    expect(paging.update).toHaveBeenCalledExactlyOnceWith([...el]);
});
