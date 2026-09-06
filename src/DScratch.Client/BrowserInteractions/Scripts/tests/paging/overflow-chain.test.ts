// @ts-ignore / rider seems to hate that import, but it is actually the correct import suggested by vitest
import { page } from 'vitest/browser';
import { expect, test } from 'vitest';
import * as domHelper from "../domHelper";
import * as paging from "../../renderEngine/paging";

test("move overflow to new page, stabilizes new page too", async () => {
    domHelper.createEditorFixture({ paragraphsPerPage: 30 * 2 });

    const textElement = document.querySelector<HTMLElement>("[data-dnode-id='p-1-30']")!;
    paging.update([textElement]);

    await expect.element(page.DPage()).toHaveLength(3);
    await expect.element(page.getByPageNumber(1)).toBeVisible();
    await expect.element(page.getByPageNumber(2)).toBeVisible();
    await expect.element(page.getByPageNumber(3)).toBeVisible();

    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toHaveLength(29);
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]")).toHaveLength(29);
    await expect.element(page.getByPageNumber(3).getByCSS("p[data-dnode-id]")).toHaveLength(2);
});

test("move overflow to next existing page, stabilizes next page too", async () => {
    domHelper.createEditorFixture({ pageCount: 2, paragraphsPerPage: 30 });

    const textElement = document.querySelector<HTMLElement>("[data-dnode-id='p-1-30']")!;
    paging.update([textElement]);

    await expect.element(page.DPage()).toHaveLength(3);
    await expect.element(page.getByPageNumber(1)).toBeVisible();
    await expect.element(page.getByPageNumber(2)).toBeVisible();
    await expect.element(page.getByPageNumber(3)).toBeVisible();

    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toHaveLength(29);
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]")).toHaveLength(29);
    await expect.element(page.getByPageNumber(3).getByCSS("p[data-dnode-id]")).toHaveLength(2);
});