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

// Check that style and node id stay on node

test("adds all attributes to all nodes", async () => {

});

test("adds missing attributes to all nodes", async () => {

});

test("removes old attributes from all nodes", async () => {

});

test("removes all attributes to all nodes", async () => {

});
