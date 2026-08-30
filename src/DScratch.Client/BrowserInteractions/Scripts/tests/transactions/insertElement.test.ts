import { expect, test } from 'vitest';
// @ts-ignore / rider seems to hate that import, but it is actually the correct import suggested by vitest
import { page } from 'vitest/browser';
import * as domHelper from "../domHelper";
import * as transaction from "../../renderEngine/transaction";

test('inserts element after given sibling in given parent', async () => {
    domHelper.createEditorFixture({ pageCount: 1, paragraphsPerPage: 1 });

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
});