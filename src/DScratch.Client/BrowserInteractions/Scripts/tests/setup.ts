// @ts-ignore / rider seems to hate that import, but it is actually the correct import suggested by vitest
import { locators, type Locator } from 'vitest/browser';

import '../../../wwwroot/editor-tokens.css';
import '../../../wwwroot/document-styles.css';
import '../../../Pages/Editor/EditorPage.razor.css';

declare module 'vitest/browser' {
    interface LocatorSelectors {
        getByCSS(css: string): Locator;
        getByPageNumber(pageNumber: number): Locator;
        DPage(): Locator;
    }
}

locators.extend({
    getByCSS(css: string) {
        return `css=${css}`;
    },
    getByPageNumber(pageNumber: number) {
        return `css=[data-page-index="${pageNumber}"]`;
    },
    DPage() {
        return "css=[data-page-index]";
    }
});