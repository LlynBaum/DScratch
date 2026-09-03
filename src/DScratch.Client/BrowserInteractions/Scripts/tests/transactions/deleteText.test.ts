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

test("delete all text int Text DNode", async () => {
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