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

const PAGE_LONG_TEXT = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. ".repeat(20);

test("move overflow to next existing page, stabilizes next page too", async () => {
    domHelper.createEditorFixture({ paragraphsPerPage: 0 });
    domHelper.createSplittedParagraph(1, "pt-1", 2);
    
    domHelper.insertText(PAGE_LONG_TEXT, {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 1
    });
    domHelper.insertText(PAGE_LONG_TEXT, {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 2
    });
    domHelper.insertText("Hello", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 3
    });

    const textElement = document.querySelector<HTMLElement>("[data-dnode-id='pt-1']")!;
    paging.update([textElement]);

    await expect.element(page.DPage()).toHaveLength(3);
    await expect.element(page.getByPageNumber(1)).toBeVisible();
    await expect.element(page.getByPageNumber(2)).toBeVisible();
    await expect.element(page.getByPageNumber(3)).toBeVisible();

    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toBeVisible();
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]")).toBeVisible();
    await expect.element(page.getByPageNumber(3).getByCSS("p[data-dnode-id]")).toBeVisible();

    const textPart1 = document.querySelector<HTMLElement>("[data-split-part='1'] [data-dnode-id='t-1']");
    const textPart2 = document.querySelector<HTMLElement>("[data-split-part='2'] [data-dnode-id='t-1']");
    const textPart3 = document.querySelector<HTMLElement>("[data-split-part='3'] [data-dnode-id='t-1']");

    expect(textPart1!.textContent + textPart2!.textContent + textPart3!.textContent).toEqual(PAGE_LONG_TEXT + PAGE_LONG_TEXT + "Hello");
});

test("", async () => {
    domHelper.createEditorFixture({ paragraphsPerPage: 0, pageCount: 3 });
    
    let previousSibling1: string | undefined = undefined;
    let previousSibling3: string | undefined = undefined;
    for (let i = 0; i < 30; i++) {
        domHelper.insertParagraph(`p-1-${i + 1}`, 1, previousSibling1);
        domHelper.insertParagraph(`p-3-${i + 1}`, 3, previousSibling3);

        previousSibling1 = `p-1-${i + 1}`;
        previousSibling3 = `p-3-${i + 1}`;
    }

    const textElement = document.querySelector<HTMLElement>("[data-dnode-id='p-1-30']")!;
    paging.update([textElement]);

    await expect.element(page.DPage()).toHaveLength(3);
    await expect.element(page.getByPageNumber(1)).toBeVisible();
    await expect.element(page.getByPageNumber(2)).toBeVisible();
    await expect.element(page.getByPageNumber(3)).toBeVisible();

    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toHaveLength(29);
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]")).toHaveLength(1);
    await expect.element(page.getByPageNumber(3).getByCSS("p[data-dnode-id]")).toHaveLength(30);
});