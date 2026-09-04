// @ts-ignore / rider seems to hate that import, but it is actually the correct import suggested by vitest
import { page } from 'vitest/browser';
import { expect, test, beforeEach, vi } from 'vitest';
import * as domHelper from "../domHelper";
import * as transaction from "../../renderEngine/transaction";
import * as paging from "../../renderEngine/paging";
import {StepType, UpdateAttributes} from "../../renderEngine/transaction";

beforeEach(() => { vi.clearAllMocks(); });
vi.mock('../../renderEngine/paging', () => ({
    update: vi.fn(),
}));

// Check that style and node id stay on node

test("adds all attributes to all nodes", async () => {
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
    el.forEach(e => e.style.fontWeight = "bold");

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.updateAttributes,
                nodeId: "t-1",
                attributes: {
                    "data-test": "123"
                }
            } as UpdateAttributes
        ]
    });
    
    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveAttribute("data-test", "123");
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveAttribute("data-test", "123");

    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveStyle(`font-weight: bold;`);
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveStyle(`font-weight: bold;`);
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([...el]);
});

test("adds missing attributes to all nodes", async () => {
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
    el.forEach(e => {
        e.style.fontWeight = "bold";
        e.setAttribute("data-test-2", "321");
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.updateAttributes,
                nodeId: "t-1",
                attributes: {
                    "data-test": "123",
                    "data-test-2": "321"
                }
            } as UpdateAttributes
        ]
    });

    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveAttribute("data-test", "123");
    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveAttribute("data-test-2", "321");
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveAttribute("data-test", "123");
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveAttribute("data-test-2", "321");

    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveStyle(`font-weight: bold;`);
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveStyle(`font-weight: bold;`);
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([...el]);
});

test("removes old attributes from all nodes", async () => {
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
    el.forEach(e => {
        e.style.fontWeight = "bold";
        e.setAttribute("data-test-2", "321");
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.updateAttributes,
                nodeId: "t-1",
                attributes: {
                    "data-test": "123"
                }
            } as UpdateAttributes
        ]
    });

    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveAttribute("data-test", "123");
    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).not.toHaveAttribute("data-test-2");
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveAttribute("data-test", "123");
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).not.toHaveAttribute("data-test-2");

    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveStyle(`font-weight: bold;`);
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveStyle(`font-weight: bold;`);
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([...el]);
});

test("removes all attributes from all nodes", async () => {
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
    el.forEach(e => {
        e.style.fontWeight = "bold";
        e.setAttribute("data-test-2", "321");
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.updateAttributes,
                nodeId: "t-1",
                attributes: { }
            } as UpdateAttributes
        ]
    });

    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).not.toHaveAttribute("data-test-2");
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).not.toHaveAttribute("data-test-2");

    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveStyle(`font-weight: bold;`);
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveStyle(`font-weight: bold;`);
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([...el]);
});

test("updates all attributes on all nodes", async () => {
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
    el.forEach(e => {
        e.style.fontWeight = "bold";
        e.setAttribute("data-test", "321");
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.updateAttributes,
                nodeId: "t-1",
                attributes: {
                    "data-test": "123"
                }
            } as UpdateAttributes
        ]
    });

    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveAttribute("data-test", "123");
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveAttribute("data-test", "123");

    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveStyle(`font-weight: bold;`);
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveStyle(`font-weight: bold;`);
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([...el]);
});
