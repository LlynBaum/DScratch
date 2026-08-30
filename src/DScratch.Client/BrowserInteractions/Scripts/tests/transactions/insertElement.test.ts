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

test('inserts element as first child of given parent', async () => {
    domHelper.createEditorFixture({ pageCount: 1, paragraphsPerPage: 2 });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: transaction.StepType.insertElement,
                parentId: "Root",
                newNodeId: "t-1",
                previousSiblingId: null,
                tagName: "p",
                attributes: null
            } as transaction.InsertElementStep
        ]
    });

    await expect.element(page.DPage()).toHaveLength(1);
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]").nth(0)).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]").nth(0)).toHaveAttribute("data-dnode-id", "t-1");
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]").nth(1)).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]").nth(1)).toHaveAttribute("data-dnode-id", "p-1-1");
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]").nth(2)).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]").nth(2)).toHaveAttribute("data-dnode-id", "p-1-2");

    const el = document.querySelector<HTMLElement>("p[data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([el]);
});

test('inserts element after given sibling in given parent', async () => {
    domHelper.createEditorFixture({ pageCount: 1, paragraphsPerPage: 2 });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: transaction.StepType.insertElement,
                parentId: "Root",
                newNodeId: "t-1",
                previousSiblingId: "p-1-1",
                tagName: "p",
                attributes: null
            } as transaction.InsertElementStep
        ]
    });

    await expect.element(page.DPage()).toHaveLength(1);
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]").nth(0)).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]").nth(0)).toHaveAttribute("data-dnode-id", "p-1-1");
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]").nth(1)).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]").nth(1)).toHaveAttribute("data-dnode-id", "t-1");
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]").nth(2)).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]").nth(2)).toHaveAttribute("data-dnode-id", "p-1-2");

    const el = document.querySelector<HTMLElement>("p[data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([el]);
});

test("insert element after last split part of given sibling", async () => {
    domHelper.createEditorFixture();
    domHelper.createSplittedParagraph(1, "ps-1");

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: transaction.StepType.insertElement,
                parentId: "Root",
                newNodeId: "t-1",
                previousSiblingId: "ps-1",
                tagName: "p",
                attributes: null
            } as transaction.InsertElementStep
        ]
    });
    
    await expect.element(page.DPage()).toHaveLength(2);
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id='t-1']")).not.toBeInTheDocument();
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id='t-1']")).toBeVisible();

    const el = document.querySelector<HTMLElement>("p[data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([el]);
});

test("insert element inside a split part block", async () => {
    domHelper.createEditorFixture();
    domHelper.createSplittedParagraph(1, "ps-1");

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: transaction.StepType.insertElement,
                parentId: "ps-1",
                newNodeId: "t-1",
                previousSiblingId: "ps-1-text",
                tagName: "span",
                attributes: null
            } as transaction.InsertElementStep
        ]
    });

    await expect.element(page.DPage()).toHaveLength(2);
    await expect.element(page.getByPageNumber(1).getByCSS("span[data-dnode-id='t-1']")).not.toBeInTheDocument();
    await expect.element(page.getByPageNumber(2).getByCSS("span[data-dnode-id='t-1']")).toBeInTheDocument();

    const el = document.querySelector<HTMLElement>("[data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([el]);
});