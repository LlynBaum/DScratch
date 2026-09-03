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

test("adds all marks to all nodes", async () => {

});

test("adds missing marks to all nodes", async () => {

});

test("removes old marks from all nodes", async () => {

});

test("removes all marks to all nodes", async () => {

});
