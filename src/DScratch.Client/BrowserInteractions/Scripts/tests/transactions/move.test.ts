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

test("moves given node into empty target parent", async () => {

});

test("moves given node at start of target parent", async () => {

});

test("moves given node after given sibling into target parent", async () => {

});

test("moves all split nodes into empty target parent", async () => {

});

test("moves all split nodes at start of target parent", async () => {

});

test("moves all split nodes after sibling into target parent", async () => {

});