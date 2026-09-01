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

test("deletes element with target id", async () => {
    domHelper.createEditorFixture({ pageCount: 1, paragraphsPerPage: 2 });
    
    const deletedElement = document.querySelector<HTMLElement>("p[data-dnode-id='p-1-2']");

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: transaction.StepType.deleteElement,
                targetId: "p-1-2"
            } as transaction.DeleteElementStep
        ]
    });
    
    await expect.element(page.DPage()).toHaveLength(1);
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toHaveLength(1);
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toHaveAttribute("data-dnode-id", "p-1-1");

    expect(paging.update).toHaveBeenCalledExactlyOnceWith([deletedElement]);
});

test("deletes all element with target id", async () => {
    domHelper.createEditorFixture({ pageCount: 1, paragraphsPerPage: 1 });
    domHelper.createSplittedParagraph(1, "t-1");

    const deletedElement = document.querySelectorAll<HTMLElement>("p[data-dnode-id='t-1']");

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: transaction.StepType.deleteElement,
                targetId: "t-1"
            } as transaction.DeleteElementStep
        ]
    });

    await expect.element(page.DPage()).toHaveLength(2);
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toHaveLength(1);
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toHaveAttribute("data-dnode-id", "p-1-1");
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]")).toHaveLength(0);

    expect(paging.update).toHaveBeenCalledExactlyOnceWith([...deletedElement]);
});