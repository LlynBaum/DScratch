// @ts-ignore / rider seems to hate that import, but it is actually the correct import suggested by vitest
import { page } from 'vitest/browser';
import {beforeEach, expect, test, vi} from 'vitest';
import * as domHelper from "../domHelper";
import * as transaction from "../../renderEngine/transaction";
import {InsertTextStep, StepType} from "../../renderEngine/transaction";
import {insertText} from "../domHelper";
import * as paging from "../../renderEngine/paging";

beforeEach(() => { vi.clearAllMocks(); });
vi.mock('../../renderEngine/paging', () => ({
    update: vi.fn(),
}));

test("inserts text into empty parent", async () => {
    domHelper.createEditorFixture();
    insertText("", {
        parentId: "p-1-1",
        id: "t-1"
    });
    
    transaction.applyTransaction({ 
        cursorPosition: null, 
        steps: [
            {
                type: StepType.insertText,
                parentId: "t-1",
                offset: 0,
                text: "Hello World!"
            } as InsertTextStep
        ]
    });

    await expect.element(page.getByTestId("t-1")).toHaveTextContent("Hello World!");

    const el = document.querySelector<HTMLElement>("[data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([el]);
});

test("inserts text before existing text", async () => {
    domHelper.createEditorFixture();
    insertText("World!", {
        parentId: "p-1-1",
        id: "t-1"
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.insertText,
                parentId: "t-1",
                offset: 0,
                text: "Hello "
            } as InsertTextStep
        ]
    });

    await expect.element(page.getByTestId("t-1")).toHaveTextContent("Hello World!");

    const el = document.querySelector<HTMLElement>("[data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([el]);
});

test("inserts text after existing text", async () => {
    domHelper.createEditorFixture();
    insertText("Hello", {
        parentId: "p-1-1",
        id: "t-1"
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.insertText,
                parentId: "t-1",
                offset: 5,
                text: " World!"
            } as InsertTextStep
        ]
    });

    await expect.element(page.getByTestId("t-1")).toHaveTextContent("Hello World!");

    const el = document.querySelector<HTMLElement>("[data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([el]);
});

test("inserts text at given offset", async () => {
    domHelper.createEditorFixture();
    insertText("Hello!", {
        parentId: "p-1-1",
        id: "t-1"
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.insertText,
                parentId: "t-1",
                offset: 5,
                text: " World"
            } as InsertTextStep
        ]
    });

    await expect.element(page.getByTestId("t-1")).toHaveTextContent("Hello World!");

    const el = document.querySelector<HTMLElement>("[data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([el]);
});

test("inserts text at given offset in first split part", async () => {
    domHelper.createEditorFixture();
    domHelper.createSplittedParagraph(1, "pt-1");
    
    insertText("Heo", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 1
    });
    
    insertText("World!", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 2
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.insertText,
                parentId: "t-1",
                offset: 2,
                text: "ll"
            } as InsertTextStep
        ]
    });

    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveTextContent("Hello");
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveTextContent("World!");

    const el = document.querySelector<HTMLElement>("[data-split-part='1'] [data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([el]);
});

test("inserts text at end of first split part", async () => {
    domHelper.createEditorFixture();
    domHelper.createSplittedParagraph(1, "pt-1");

    insertText("He", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 1
    });

    insertText("World!", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 2
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.insertText,
                parentId: "t-1",
                offset: 2,
                text: "llo"
            } as InsertTextStep
        ]
    });

    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveTextContent("Hello");
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveTextContent("World!");

    const el = document.querySelector<HTMLElement>("[data-split-part='1'] [data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([el]);
});

test("inserts text at given offset in second split part", async () => {
    domHelper.createEditorFixture();
    domHelper.createSplittedParagraph(1, "pt-1");

    insertText("Hello", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 1
    });

    insertText("Wd!", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 2
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.insertText,
                parentId: "t-1",
                offset: 6,
                text: "orl"
            } as InsertTextStep
        ]
    });

    await expect.element(page.getByPageNumber(1).getByTestId("t-1")).toHaveTextContent("Hello");
    await expect.element(page.getByPageNumber(2).getByTestId("t-1")).toHaveTextContent("World!");

    const el = document.querySelector<HTMLElement>("[data-split-part='2'] [data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([el]);
});