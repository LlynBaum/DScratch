// @ts-ignore / rider seems to hate that import, but it is actually the correct import suggested by vitest
import { page } from 'vitest/browser';
import {beforeEach, expect, test, vi} from 'vitest';
import * as domHelper from "../domHelper";
import * as transaction from "../../renderEngine/transaction";
import {MoveStep, StepType} from "../../renderEngine/transaction";
import * as paging from "../../renderEngine/paging";

beforeEach(() => { vi.clearAllMocks(); });
vi.mock('../../renderEngine/paging', () => ({
    update: vi.fn(),
}));

test("moves given node into empty target parent", async () => {
    domHelper.createEditorFixture({ paragraphsPerPage: 2 });
    domHelper.insertText("test", {
        parentId: "p-1-1",
        id: "t-1"
    });
    
    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.move,
                targetNodeId: "t-1",
                previousSiblingId: null,
                targetParentId: "p-1-2"
            } as MoveStep
        ]
    });
    
    await expect.element(page.getByTestId("p-1-1").getByTestId("t-1")).toHaveLength(0);
    await expect.element(page.getByTestId("p-1-2").getByTestId("t-1")).toBeVisible();

    const el = document.querySelector<HTMLElement>("[data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([el]);
});

test("moves given node at start of target parent", async () => {
    domHelper.createEditorFixture({ paragraphsPerPage: 2 });
    domHelper.insertText("test", {
        parentId: "p-1-1",
        id: "t-1"
    });
    domHelper.insertText("test", {
        parentId: "p-1-2",
        id: "t-2"
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.move,
                targetNodeId: "t-1",
                previousSiblingId: null,
                targetParentId: "p-1-2"
            } as MoveStep
        ]
    });

    await expect.element(page.getByTestId("p-1-1").getByCSS("span")).toHaveLength(0);
    await expect.element(page.getByTestId("p-1-2").getByCSS("span").nth(0)).toHaveAttribute("data-dnode-id", "t-1");
    await expect.element(page.getByTestId("p-1-2").getByCSS("span").nth(1)).toHaveAttribute("data-dnode-id", "t-2");

    const el = document.querySelector<HTMLElement>("[data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([el]);
});

test("moves given node after given sibling into target parent", async () => {
    domHelper.createEditorFixture({ paragraphsPerPage: 2 });
    domHelper.insertText("test", {
        parentId: "p-1-1",
        id: "t-1"
    });
    domHelper.insertText("test", {
        parentId: "p-1-2",
        id: "t-2"
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.move,
                targetNodeId: "t-1",
                previousSiblingId: "t-2",
                targetParentId: "p-1-2"
            } as MoveStep
        ]
    });

    await expect.element(page.getByTestId("p-1-1").getByCSS("span")).toHaveLength(0);
    await expect.element(page.getByTestId("p-1-2").getByCSS("span").nth(0)).toHaveAttribute("data-dnode-id", "t-2");
    await expect.element(page.getByTestId("p-1-2").getByCSS("span").nth(1)).toHaveAttribute("data-dnode-id", "t-1");

    const el = document.querySelector<HTMLElement>("[data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([el]);
});

test("moves all split nodes into empty target parent", async () => {
    domHelper.createEditorFixture({ paragraphsPerPage: 1 });
    domHelper.createSplittedParagraph(1, "pt-1");
    
    domHelper.insertText("test", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 1
    });
    domHelper.insertText("test", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 2
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.move,
                targetNodeId: "t-1",
                previousSiblingId: null,
                targetParentId: "p-1-1"
            } as MoveStep
        ]
    });

    await expect.element(page.getByPageNumber(1).getByTestId("pt-1").getByCSS("span")).toHaveLength(0);
    await expect.element(page.getByPageNumber(2).getByTestId("pt-1").getByCSS("span")).toHaveLength(0);
    
    await expect.element(page.getByTestId("p-1-1").getByCSS("span").nth(0)).toHaveAttribute("data-dnode-id", "t-1");
    await expect.element(page.getByTestId("p-1-1").getByCSS("span").nth(1)).toHaveAttribute("data-dnode-id", "t-1");

    const el = document.querySelectorAll<HTMLElement>("[data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([...el]);
});

test("moves all split nodes at start of target parent", async () => {
    domHelper.createEditorFixture({ paragraphsPerPage: 1 });
    domHelper.insertText("test", {
        parentId: "p-1-1",
        id: "b-1"
    });

    domHelper.createSplittedParagraph(1, "pt-1");
    domHelper.insertText("test", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 1
    });
    domHelper.insertText("test", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 2
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.move,
                targetNodeId: "t-1",
                previousSiblingId: null,
                targetParentId: "p-1-1"
            } as MoveStep
        ]
    });

    await expect.element(page.getByPageNumber(1).getByTestId("pt-1").getByCSS("span")).toHaveLength(0);
    await expect.element(page.getByPageNumber(2).getByTestId("pt-1").getByCSS("span")).toHaveLength(0);

    await expect.element(page.getByTestId("p-1-1").getByCSS("span").nth(0)).toHaveAttribute("data-dnode-id", "t-1");
    await expect.element(page.getByTestId("p-1-1").getByCSS("span").nth(1)).toHaveAttribute("data-dnode-id", "t-1");
    await expect.element(page.getByTestId("p-1-1").getByCSS("span").nth(2)).toHaveAttribute("data-dnode-id", "b-1");

    const el = document.querySelectorAll<HTMLElement>("[data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([...el]);
});

test("moves all split nodes after sibling into target parent", async () => {
    domHelper.createEditorFixture({ paragraphsPerPage: 1 });
    domHelper.insertText("test", {
        parentId: "p-1-1",
        id: "b-1"
    });

    domHelper.createSplittedParagraph(1, "pt-1");
    domHelper.insertText("test", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 1
    });
    domHelper.insertText("test", {
        parentId: "pt-1",
        id: "t-1",
        splitPart: 2
    });

    transaction.applyTransaction({
        cursorPosition: null,
        steps: [
            {
                type: StepType.move,
                targetNodeId: "t-1",
                previousSiblingId: "b-1",
                targetParentId: "p-1-1"
            } as MoveStep
        ]
    });

    await expect.element(page.getByPageNumber(1).getByTestId("pt-1").getByCSS("span")).toHaveLength(0);
    await expect.element(page.getByPageNumber(2).getByTestId("pt-1").getByCSS("span")).toHaveLength(0);

    await expect.element(page.getByTestId("p-1-1").getByCSS("span").nth(0)).toHaveAttribute("data-dnode-id", "b-1");
    await expect.element(page.getByTestId("p-1-1").getByCSS("span").nth(1)).toHaveAttribute("data-dnode-id", "t-1");
    await expect.element(page.getByTestId("p-1-1").getByCSS("span").nth(2)).toHaveAttribute("data-dnode-id", "t-1");

    const el = document.querySelectorAll<HTMLElement>("[data-dnode-id='t-1']");
    expect(paging.update).toHaveBeenCalledExactlyOnceWith([...el]);
});