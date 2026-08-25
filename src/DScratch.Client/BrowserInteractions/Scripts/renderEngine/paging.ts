const PAGE_INDEX_ATTRIBUTE = "data-page-index";
const SPLIT_ATTRIBUTE = "data-split-part";

const pageTemplate =  document.getElementById("page-template") as HTMLTemplateElement;

interface Overflow {
    IsOverflowing: boolean;
    BlockElement: HTMLElement;
    Page: HTMLElement;
    PageBottom: number;
    ElementBottom: number;
    Margin: number;
}

export function update(modifiedNodes: HTMLElement[]) {
    const uniquePages = Array.from(
        new Set(
            modifiedNodes
                .map(n => n.closest<HTMLElement>(`.page[${PAGE_INDEX_ATTRIBUTE}]`))
                .filter((node) => node !== null)
        )
    );
    
    const sortedPages = uniquePages.sort((a, b) => {
        const aIdx = Number(a.getAttribute(PAGE_INDEX_ATTRIBUTE) ?? Infinity);
        const bIdx = Number(b.getAttribute(PAGE_INDEX_ATTRIBUTE) ?? Infinity);
        return bIdx - aIdx;
    });

    if (sortedPages.length === 0) return;
    greedyFlow(sortedPages);
}

function greedyFlow(modifiedPages: HTMLElement[]) {
    let currentPage;
    while (currentPage = modifiedPages.pop()) {
        // If the page was removed from the DOM, assume it underflowed and was empty, so we can safely skip it.
        if (!currentPage.isConnected) {
            continue;
        }
        
        const overflow = getBottomOverflowingChildren(currentPage);
        if (!overflow || !overflow.IsOverflowing) continue;
        
        const pageIndex = Number(currentPage.getAttribute(PAGE_INDEX_ATTRIBUTE));
        const newPage = createPage(pageIndex + 1); // TODO: if there is a next page, move text over instead of create page

        stabilize(overflow, newPage);
        
       // currentPage.nextElementSibling && modifiedPages.push(currentPage.nextElementSibling as HTMLElement);
    }
}

function stabilize(overflow: Overflow, targetPage: HTMLElement) {
    const walker = document.createTreeWalker(overflow.BlockElement, NodeFilter.SHOW_TEXT);
    let lastNode = walker.lastChild() as Text | null;
    let currentNode = walker.previousNode() as Text | null;
    
    // No text, assume whole block must be moved
    if (!lastNode) {
        moveBlock(overflow, targetPage);
        return;
    }

    while (currentNode && isOverflowing(currentNode)){
        lastNode = currentNode;
        currentNode = walker.previousNode() as Text | null;
    }
    
    splitText(lastNode, overflow, targetPage);
    return;

    function isOverflowing(node: Node): boolean {
        const range = document.createRange();
        range.selectNodeContents(node);
        const bottom = range.getBoundingClientRect().bottom + overflow.Margin;
        return bottom > overflow.PageBottom;
    }
}

function splitText(textNode: Text, overflow: Overflow, targetPage: HTMLElement) {
    const index = findSplitIndex(textNode, overflow);
    const wordSafeIndex = getWordSafeSplitIndex(textNode.textContent, index);

    if (index === 0) {
        moveBlock(overflow, targetPage);
        return;
    }

    const range = new Range();
    range.setStart(textNode, wordSafeIndex);
    range.setEndAfter(overflow.Page.lastElementChild!);
    
    const overflowContent = range.extractContents();
    
    let content;
    if (overflowContent && overflowContent.firstElementChild?.matches("[data-dnode-id='Root']")) {
        content = overflowContent?.querySelectorAll("[data-dnode-id='Root'] > *");
    } else {
        content = overflowContent.childNodes;
    }

    const referenceNode = targetPage.firstElementChild!.firstElementChild ?? null;
    if (referenceNode) {
        referenceNode.before(...content);
    } else {
        targetPage.firstElementChild!.append(...content);
    }

    overflow.BlockElement.setAttribute(SPLIT_ATTRIBUTE, "1");
    
    for (const node of content) {
        if (node.nodeType === Node.ELEMENT_NODE) {
            (node as HTMLElement).setAttribute(SPLIT_ATTRIBUTE, "2");
            break;
        }
    }
}

function moveBlock(overflow: Overflow, targetPage: HTMLElement) {
    const parent = targetPage.querySelector<HTMLElement>("div[contenteditable]")!;

    let currentElement: Element | null = overflow.BlockElement;
    while (currentElement) {
        parent.appendChild(currentElement);
        currentElement = currentElement.nextElementSibling;
    }
}

function createPage(index: number) {
    const page = document.importNode(pageTemplate.content, true).firstElementChild as HTMLElement;
    page.setAttribute(PAGE_INDEX_ATTRIBUTE, index.toString());
    const nextPage = document.querySelector<HTMLElement>(`[${PAGE_INDEX_ATTRIBUTE}='${index - 1}']`)?.nextElementSibling;
    
    if (nextPage) {
        window.editor.node?.insertBefore(page, nextPage);
    } else {
        window.editor.node?.appendChild(page);
    }
    
    return page;
}

function getBottomOverflowingChildren(page: HTMLElement): Overflow | null {
    const pageContent = page.firstElementChild as HTMLElement;
    if (!pageContent) return null;

    const pageContentRect = pageContent.getBoundingClientRect();
    const pageContentStyle = window.getComputedStyle(pageContent);
    const paddingBottom = parseFloat(pageContentStyle.paddingBottom) || 0;
    const pageBottom = pageContentRect.bottom - paddingBottom;

    const lastBlockElement = pageContent.lastElementChild as HTMLElement; // TODO: it could also be any previous block that is already overflowing
    if (!lastBlockElement) return null;
    
    const blockStyle = window.getComputedStyle(lastBlockElement);
    const marginBottom = parseFloat(blockStyle.marginBottom) || 0;
    const childBottom = lastBlockElement.getBoundingClientRect().bottom + marginBottom;
    
    return {
        IsOverflowing: childBottom > pageBottom,
        BlockElement: lastBlockElement,
        Page: page,
        PageBottom: pageBottom,
        ElementBottom: childBottom,
        Margin: marginBottom
    };
}

function findSplitIndex(textNode: Text, overflow: Overflow): number {
    const range = document.createRange();
    const fullText = textNode.textContent ?? "";

    let low = 0;
    let high = fullText.length;
    let splitIndex = fullText.length;

    while (low <= high) {
        const mid = Math.floor((low + high) / 2);

        // Measure from the start of the node to the mid-point
        range.setStart(textNode, 0);
        range.setEnd(textNode, mid);

        const rect = range.getBoundingClientRect();
        const bottom = rect.bottom + overflow.Margin

        if (bottom > overflow.PageBottom) {
            // This segment overflows, search left to find the earliest overflow point
            splitIndex = mid;
            high = mid - 1;
        } else {
            // Fits within bounds, search right
            low = mid + 1;
        }
    }

    return splitIndex;
}

function getWordSafeSplitIndex(textContent: string, index: number) {
    if (index <= 0 || index >= textContent.length) return index;
    const lastSpace = textContent.lastIndexOf(" ", index);
    return lastSpace > 0 ? lastSpace : index;
}