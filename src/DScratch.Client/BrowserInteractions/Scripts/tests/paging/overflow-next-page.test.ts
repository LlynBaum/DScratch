// @ts-ignore / rider seems to hate that import, but it is actually the correct import suggested by vitest
import { page } from 'vitest/browser';
import { expect, test } from 'vitest';
import * as domHelper from "../domHelper";
import * as paging from "../../renderEngine/paging";

const OVERFLOW_TEXT = "fjdjsflksdjlkfjslkdjflkjsdlkfjklsjflksjfkljsdfsfsdfsdf sdf sdfasd lkfjalk jdlfj lajsfljal jflkjl fjlaj lfja lsjdföl kjajsd öljasd";

test("moves overflow block to new page", async () => {
    domHelper.createEditorFixture({ paragraphsPerPage: 30 });
    domHelper.insertPage(2);
    domHelper.insertParagraph("p-1", 2);

    const textElement = document.querySelector<HTMLElement>("[data-dnode-id='p-1-30']")!;
    paging.update([textElement]);

    await expect.element(page.getByPageNumber(1)).toBeVisible();
    await expect.element(page.getByPageNumber(2)).toBeVisible();

    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toHaveLength(29);
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]")).toHaveLength(2);
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]").nth((0))).not.toHaveAttribute("data-split-part");
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]").nth(0)).toHaveAttribute("data-dnode-id", "p-1-30");
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]").nth((1))).not.toHaveAttribute("data-split-part");
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]").nth(1)).toHaveAttribute("data-dnode-id", "p-1");
});

test("moves overflow text to new page", async () => {
    domHelper.createEditorFixture({ paragraphsPerPage: 29 });
    domHelper.insertPage(2);
    domHelper.insertParagraph("p-1", 2);
    
    domHelper.insertText(OVERFLOW_TEXT, {
        parentId: "p-1-29",
        id: "t-1"
    });

    const textElement = document.querySelector<HTMLElement>("[data-dnode-id='t-1']")!;
    paging.update([textElement]);

    await expect.element(page.getByPageNumber(1)).toBeVisible();
    await expect.element(page.getByPageNumber(2)).toBeVisible();

    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toHaveLength(29);
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]")).toHaveLength(2);

    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]").last()).toHaveAttribute("data-split-part", "1");
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]").nth(0)).toHaveAttribute("data-split-part", "2");
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]").nth(0)).toHaveAttribute("data-dnode-id", "p-1-29");
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]").nth(1)).not.toHaveAttribute("data-split-part");
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]").nth(1)).toHaveAttribute("data-dnode-id", "p-1");

    await expect.element(page.getByPageNumber(1).getByCSS("[data-split-part='1']").getByTestId("t-1")).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("[data-split-part='1']").getByTestId("t-1")).not.toHaveTextContent("");
    await expect.element(page.getByPageNumber(2).getByCSS("[data-split-part='2']").getByTestId("t-1")).toBeVisible();
    await expect.element(page.getByPageNumber(2).getByCSS("[data-split-part='2']").getByTestId("t-1")).not.toHaveTextContent("");

    const textPart1 = document.querySelector<HTMLElement>("[data-split-part='1'] [data-dnode-id='t-1']");
    const textPart2 = document.querySelector<HTMLElement>("[data-split-part='2'] [data-dnode-id='t-1']");

    expect(textPart1!.textContent + textPart2!.textContent).toEqual(OVERFLOW_TEXT);
});

test("moves all overflowing blocks to new page", async () => {
    domHelper.createEditorFixture({ paragraphsPerPage: 31 });
    domHelper.insertPage(2);
    domHelper.insertParagraph("p-1", 2);

    const textElement = document.querySelector<HTMLElement>("[data-dnode-id='p-1-31']")!;
    paging.update([textElement]);

    await expect.element(page.getByPageNumber(1)).toBeVisible();
    await expect.element(page.getByPageNumber(2)).toBeVisible();

    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toHaveLength(29);
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]")).toHaveLength(3);
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]").nth(0)).toHaveAttribute("data-dnode-id", "p-1-30");
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]").nth(1)).toHaveAttribute("data-dnode-id", "p-1-31");
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]").nth(2)).toHaveAttribute("data-dnode-id", "p-1");
    
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]").last()).toHaveAttribute("data-split-part", "1");
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]").nth(0)).toHaveAttribute("data-split-part", "2");
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]").nth(1)).not.toHaveAttribute("data-split-part");
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]").nth(2)).not.toHaveAttribute("data-split-part");
});

test("moves overflow block and overflow text to new page", async () => {
    domHelper.createEditorFixture({ paragraphsPerPage: 30 });
    domHelper.insertPage(2);
    domHelper.insertParagraph("p-1", 2);
    
    domHelper.insertText(OVERFLOW_TEXT, {
        parentId: "p-1-29",
        id: "t-1"
    });

    const textElement = document.querySelector<HTMLElement>("[data-dnode-id='t-1']")!;
    paging.update([textElement]);

    await expect.element(page.getByPageNumber(1)).toBeVisible();
    await expect.element(page.getByPageNumber(2)).toBeVisible();

    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toHaveLength(29);
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]")).toHaveLength(3);

    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]").last()).toHaveAttribute("data-split-part", "1");
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]").nth(0)).toHaveAttribute("data-split-part", "2");
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]").nth(0)).toHaveAttribute("data-dnode-id", "p-1-29");
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]").nth(1)).toHaveAttribute("data-dnode-id", "p-1-30");
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]").nth(2)).toHaveAttribute("data-dnode-id", "p-1");

    await expect.element(page.getByPageNumber(1).getByCSS("[data-split-part='1']").getByTestId("t-1")).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("[data-split-part='1']").getByTestId("t-1")).not.toHaveTextContent("");
    await expect.element(page.getByPageNumber(2).getByCSS("[data-split-part='2']").getByTestId("t-1")).toBeVisible();
    await expect.element(page.getByPageNumber(2).getByCSS("[data-split-part='2']").getByTestId("t-1")).not.toHaveTextContent("");

    const textPart1 = document.querySelector<HTMLElement>("[data-split-part='1'] [data-dnode-id='t-1']");
    const textPart2 = document.querySelector<HTMLElement>("[data-split-part='2'] [data-dnode-id='t-1']");

    expect(textPart1!.textContent + textPart2!.textContent).toEqual(OVERFLOW_TEXT);
});

test("Typing in overflowing paragraph in first page, moves newly overflown text to next page and puts it into the same element as the already overflown text", async () => {
    domHelper.createEditorFixture({ paragraphsPerPage: 28 });
    domHelper.createSplittedParagraph(1, "pt-1");

    domHelper.insertText(OVERFLOW_TEXT, {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 1
    });
    domHelper.insertText("Hello", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 2
    });

    const textElement = document.querySelector<HTMLElement>("[data-dnode-id='t-1']")!;
    paging.update([textElement]);

    await expect.element(page.getByPageNumber(1)).toBeVisible();
    await expect.element(page.getByPageNumber(2)).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toHaveLength(29);
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]")).toHaveLength(2);

    await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]").last()).toHaveAttribute("data-split-part", "1");
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]")).toHaveAttribute("data-split-part", "2");
    await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]")).toHaveAttribute("data-dnode-id", "pt-1");

    await expect.element(page.getByPageNumber(1).getByCSS("[data-split-part='1']").getByTestId("t-1")).toBeVisible();
    await expect.element(page.getByPageNumber(1).getByCSS("[data-split-part='1']").getByTestId("t-1")).not.toHaveTextContent("");
    await expect.element(page.getByPageNumber(2).getByCSS("[data-split-part='2']").getByTestId("t-1")).toBeVisible();
    await expect.element(page.getByPageNumber(2).getByCSS("[data-split-part='2']").getByTestId("t-1")).not.toHaveTextContent("");

    const textPart1 = document.querySelector<HTMLElement>("[data-split-part='1'] [data-dnode-id='t-1']");
    const textPart2 = document.querySelector<HTMLElement>("[data-split-part='2'] [data-dnode-id='t-1']");

    expect(textPart1!.textContent + textPart2!.textContent).toEqual(OVERFLOW_TEXT + "Hello");
});