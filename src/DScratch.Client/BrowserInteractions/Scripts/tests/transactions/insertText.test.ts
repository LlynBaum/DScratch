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

test("inserts text into empty parent", async () => {
    
});

test("inserts text before existing text", async () => {

});

test("inserts text after existing text", async () => {

});

test("inserts text at given offset", async () => {

});

test("inserts text at given offset in first split part", async () => {

});

test("inserts text at given offset in second split part", async () => {

});