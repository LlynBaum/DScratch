export interface RenderedPage {
    htmlString: string;
    pageNumber: number;
}

export function renderPage(page: RenderedPage) {
    const pageElement = getPage(page.pageNumber);
    
    if (!pageElement) {
        appendPage(page);
    } else {
        swapPageContent(pageElement, page.htmlString);
    }

    const rootNodes = window.editor.node!.querySelectorAll<HTMLElement>("[data-dnode-id='Root']:not([contenteditable])");
    rootNodes.forEach(n => n.setAttribute("contenteditable", ''));
}

function swapPageContent(pageElement: HTMLElement, content: string) {
    const root = parsePageHtmlString(content)?.firstElementChild; // 
    if (!root) {
        console.error("RenderedPage did not contain a Root node! Skip DOM Swap.");
        return;
    }

    pageElement.innerHTML = root.innerHTML;
}

function appendPage(page: RenderedPage) {
    const doc = window.editor.node?.querySelector<HTMLElement>(".doc-editor")!;

    const pageElement = parsePageHtmlString(page.htmlString);

    if (!pageElement) {
        console.error("RenderedPage did not contain a page node! Skip appending page.");
        return;
    }

    doc.appendChild(pageElement)
}

function parsePageHtmlString(pageHtml: string) {
    const parser = new DOMParser();
    return parser
        .parseFromString(pageHtml, "text/html")
        .querySelector<HTMLElement>(".paper");
}

function getPage(pageNumber: number) {
    return document.querySelector<HTMLElement>(`[data-page-number="${pageNumber}"]`);
}