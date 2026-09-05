// @ts-ignore / rider seems to hate that import, but it is actually the correct import suggested by vitest
import { page, utils } from 'vitest/browser';
import { expect, test } from 'vitest';
import * as domHelper from "../domHelper";
import * as paging from "../../renderEngine/paging";

const FITTING_TEXT = "jlökgfjslkfjasölkj dfölkaj sdlkfj aölksdj flök ajsdfölkj aölskdfj ölaksjdfölk jasöldkfj aölksjdf ölkajsödlfk jasölkdj fölkasj dfölkaj slök fjlköasdj flkasj dlfkjasölkdfj ölkasdjf lökasjdlökfjaslökd fjlökas jflkj öjlöj";
const OVERFLOW_TEXT = " dwww";
const TEXT = FITTING_TEXT + OVERFLOW_TEXT;

test("moves overflow block to new page", async () => {
   domHelper.createEditorFixture({ paragraphsPerPage: 30 });

   const textElement = document.querySelector<HTMLElement>("[data-dnode-id='p-1-30']")!;
   paging.update([textElement]);

   await expect.element(page.getByPageNumber(1)).toBeVisible();
   await expect.element(page.getByPageNumber(2)).toBeVisible();
   
   await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toHaveLength(29);
   await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]")).toHaveLength(1);
   await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]")).toHaveAttribute("data-dnode-id", "p-1-30");
});

test("moves overflow text to new page", async () => {
   domHelper.createEditorFixture({ paragraphsPerPage: 29, includeText: true });
   document.querySelector<HTMLElement>("[data-dnode-id='p-1-29-text']")!.remove();
   domHelper.insertText(TEXT, {
      parentId: "p-1-29",
      id: "t-1"
   });

   const textElement = document.querySelector<HTMLElement>("[data-dnode-id='t-1']")!;
   paging.update([textElement]);

   await expect.element(page.getByPageNumber(1)).toBeVisible();
   await expect.element(page.getByPageNumber(2)).toBeVisible();

   await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]")).toHaveLength(29);
   await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]")).toHaveLength(1);

   await expect.element(page.getByPageNumber(1).getByCSS("p[data-dnode-id]").last()).toHaveAttribute("data-split-part", "1");
   await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]")).toHaveAttribute("data-split-part", "2");
   await expect.element(page.getByPageNumber(2).getByCSS("p[data-dnode-id]")).toHaveAttribute("data-dnode-id", "p-1-29");
   
   await expect.element(page.getByPageNumber(1).getByCSS("[data-split-part='1']").getByTestId("t-1")).toHaveTextContent(FITTING_TEXT);
   await expect.element(page.getByPageNumber(2).getByCSS("[data-split-part='2']").getByTestId("t-1")).toHaveTextContent(OVERFLOW_TEXT);
});