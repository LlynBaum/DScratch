import '../../../wwwroot/editor-tokens.css';
import '../../../wwwroot/document-styles.css';
import { locators, type Locator } from 'vitest/browser';

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