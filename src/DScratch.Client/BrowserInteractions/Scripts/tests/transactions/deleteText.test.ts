// @ts-ignore / rider seems to hate that import, but it is actually the correct import suggested by vitest
import { page } from 'vitest/browser';
import { expect, test, beforeEach, vi } from 'vitest';
import * as domHelper from "../domHelper";
import * as transaction from "../../renderEngine/transaction";
import * as paging from "../../renderEngine/paging";

beforeEach(() => { vi.clearAllMocks(); });
vi.mock('../../renderEngine/paging', () => ({
    update: vi.fn(),
}));

test("delete text in middle of Text DNode", async () => {
    domHelper.createEditorFixture();
    domHelper.insertText("hello", {
        parentId: "p-1-1",
        id: "t-1"
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: transaction.StepType.deleteText,
                parentId: "t-1",
                offset: 2,
                length: 2
            } as transaction.DeleteTextStep
        ]
    });
    
    await expect.element(page.DPage()).toHaveLength(1);
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("span[data-dnode-id]")).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("span[data-dnode-id]")).toHaveTextContent("heo");

    const el = document.querySelector<HTMLElement>("[data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([el]);
});

test("delete text at end of Text DNode", async () => {
    domHelper.createEditorFixture();
    domHelper.insertText("hello", {
        parentId: "p-1-1",
        id: "t-1"
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: transaction.StepType.deleteText,
                parentId: "t-1",
                offset: 3,
                length: 2
            } as transaction.DeleteTextStep
        ]
    });

    await expect.element(page.DPage()).toHaveLength(1);
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("span[data-dnode-id]")).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("span[data-dnode-id]")).toHaveTextContent("hel");

    const el = document.querySelector<HTMLElement>("[data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([el]);
});

test("delete text at start of Text DNode", async () => {
    domHelper.createEditorFixture();
    domHelper.insertText("hello", {
        parentId: "p-1-1",
        id: "t-1"
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: transaction.StepType.deleteText,
                parentId: "t-1",
                offset: 0,
                length: 2
            } as transaction.DeleteTextStep
        ]
    });

    await expect.element(page.DPage()).toHaveLength(1);
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("span[data-dnode-id]")).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("span[data-dnode-id]")).toHaveTextContent("llo");

    const el = document.querySelector<HTMLElement>("[data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([el]);
});

test("delete all text in Text DNode", async () => {
    domHelper.createEditorFixture();
    domHelper.insertText("hello", {
        parentId: "p-1-1",
        id: "t-1"
    });
    
    const deletedElement = document.querySelector<HTMLElement>("[data-dnode-id='t-1']");

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: transaction.StepType.deleteText,
                parentId: "t-1",
                offset: 0,
                length: 5
            } as transaction.DeleteTextStep
        ]
    });

    await expect.element(page.DPage()).toHaveLength(1);
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("span[data-dnode-id]")).toHaveLength(0);

    expect(paging.update).toHaveBeenCalledExactlyOnceWith([deletedElement]);
});

test("delete text that spans over two split parts", async () => {
    domHelper.createEditorFixture({ paragraphsPerPage: 0 });
    domHelper.createSplittedParagraph(1, "pt-1");
    domHelper.insertText("Hello", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 1
    });
    domHelper.insertText("World!", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 2
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: transaction.StepType.deleteText,
                parentId: "t-1",
                offset: 3,
                length: 6
            } as transaction.DeleteTextStep
        ]
    });

    await expect.element(page.DPage()).toHaveLength(2);
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toBeVisible();
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]")).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("span[data-dnode-id]")).toHaveTextContent("Hel");
    await expect.element(page.getByPageNumber(2).getByCSS("span[data-dnode-id]")).toHaveTextContent("d!");

    const textElements = document.querySelectorAll<HTMLElement>("[data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([...textElements]);
});