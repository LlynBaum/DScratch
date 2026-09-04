// @ts-ignore / rider seems to hate that import, but it is actually the correct import suggested by vitest
import { page } from 'vitest/browser';
import { expect, test } from 'vitest';
import * as domHelper from "../domHelper";
import * as paging from "../../renderEngine/paging";

const FITTING_TEXT = "jlökgfjslkfjasölkj dfölkaj sdlkfj aölksdj flök ajsdfölkj aölskdfj ölaksjdfölk jasöldkfj aölksjdf ölkajsödlfk jasölkdj fölkasj dfölkaj slök fjlköasdj flkasj dlfkjasölkdfj ölkasdjf lökasjdlökfjaslökd fjlökas jflkj öjlöj";
const OVERFLOW_TEXT = " dwww";
const TEXT = FITTING_TEXT + OVERFLOW_TEXT;

test("moves overflow block to new page", async () => {
   domHelper.createEditorFixture({ paragraphsPerPage: 29 });

   const textElement = document.querySelector<HTMLElement>("[data-dnode-id='t-1']")!;
   paging.update([textElement]);

   await expect.element(page.getByPageNumber(1)).toBeVisible();
   await expect.element(page.getByPageNumber(2)).toBeVisible();

   // TODO: test for block 29 to be moved over and deleted from page 1
   
   await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveTextContent(FITTING_TEXT);
   await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveTextContent(OVERFLOW_TEXT);
});

test("moves overflow text to new page", async () => {
   domHelper.createEditorFixture({ paragraphsPerPage: 28 });
   domHelper.insertText(TEXT, {
      parentId: "p-1-28",
      id: "t-1"
   });

   const textElement = document.querySelector<HTMLElement>("[data-dnode-id='t-1']")!;
   paging.update([textElement]);

   await expect.element(page.getByPageNumber(1)).toBeVisible();
   await expect.element(page.getByPageNumber(2)).toBeVisible();

   // TODO: check that block is on both pages there
   
   await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveTextContent(FITTING_TEXT);
   await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveTextContent(OVERFLOW_TEXT);
});