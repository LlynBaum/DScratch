const PAGE_INDEX_ATTRIBUTE = "data-page-index";
const SPLIT_ATTRIBUTE = "data-split-part";

const pageTemplate =  document.getElementById("page-template") as HTMLTemplateElement;

interface Overflow {
    IsOverflowing: boolean;
    BlockElement: HTMLElement;
    Page: HTMLElement;
    PageBottom: number;
    ElementBottom: number;
}

export function update(modifiedNodes: HTMLElement[]) {
    const uniquePages = Array.from(
        new Set(
            modifiedNodes
                .map(n => n.closest<HTMLElement>(`.page[${PAGE_INDEX_ATTRIBUTE}]`))
                .filter((node): node is HTMLElement => node !== null)
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
        const overflow = getBottomOverflowingChildren(currentPage);
        if (!overflow || !overflow.IsOverflowing) continue;
        
        const pageIndex = Number(currentPage.getAttribute(PAGE_INDEX_ATTRIBUTE));
        const newPage = createPage(pageIndex + 1);
        
        splitOverflow(overflow, newPage);
        
       // currentPage.nextElementSibling && modifiedPages.push(currentPage.nextElementSibling as HTMLElement); TODO only do when next page already existed
    }
}

function splitOverflow(overflow: Overflow, targetPage: HTMLElement) {
    const walker = document.createTreeWalker(overflow.BlockElement, NodeFilter.SHOW_TEXT);
    let lastNode = walker.lastChild() as Text | null;
    let currentNode = walker.previousNode() as Text | null;
    
    // No text, assume whole block must be moved
    if (!lastNode) {
        moveBlock(overflow, targetPage);
        return;
    }
    
    while (currentNode && !isOverflowing(currentNode)){
        lastNode = currentNode;
        currentNode = walker.previousNode() as Text | null;
    }

    const index = findSplitIndex(lastNode, overflow.PageBottom);
    const wordSafeIndex = getWordSafeSplitIndex(lastNode.textContent, index);

    if (wordSafeIndex >= lastNode.textContent.length) return; // TODO: wait in that case it probably is the next node, but that shouldn't be possible
    
    if (index === 0) {
        moveBlock(overflow, targetPage);
        return;
    }
    
    const split = lastNode.splitText(wordSafeIndex);
    const span = split.parentElement;
    const splitElement = span?.cloneNode(false) as HTMLElement;
    splitElement.appendChild(split);

    lastNode.parentElement!.setAttribute(SPLIT_ATTRIBUTE, "1");
    splitElement.setAttribute(SPLIT_ATTRIBUTE, "2");

    let newElem = splitElement;
    if (splitElement.parentElement !== overflow.BlockElement) {
        const wrapper = splitElement.parentElement?.cloneNode(false) as HTMLElement;
        wrapper.appendChild(splitElement);
        newElem = wrapper;
    }

    const parent = targetPage.querySelector<HTMLElement>("div[contenteditable]")!;
    const splitBlock = overflow.BlockElement.cloneNode(false);
    parent.appendChild(splitBlock);

    splitBlock.appendChild(newElem);
    let currentElement = newElem.nextElementSibling;
    while (currentElement) {
        splitBlock.appendChild(currentElement);
        currentElement = currentElement.nextElementSibling;
    }
    
    return;
    
    function isOverflowing(node: Node): boolean {
        const range = document.createRange();
        range.selectNodeContents(node);
        return range.getBoundingClientRect().bottom > overflow.PageBottom;
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

function moveBlock(overflow: Overflow, targetPage: HTMLElement) {
    const parent = targetPage.querySelector<HTMLElement>("div[contenteditable]")!;
    
    let currentElement: Element | null = overflow.BlockElement;
    while (currentElement) {
        parent.appendChild(currentElement);
        currentElement = currentElement.nextElementSibling;
    }
}

function getBottomOverflowingChildren(page: HTMLElement): Overflow | null {
    const style = window.getComputedStyle(page);
    const borderBottom = parseFloat(style.borderBottomWidth) || 0; // Take bottom margin into account
    const innerBottom = page.getBoundingClientRect().bottom - borderBottom;

    const child = page.firstElementChild!.lastElementChild; // TODO: it could also be any previous block that is already overflowing
    if (!child) return null;
    const childBottom = child.getBoundingClientRect().bottom;
    return {
        IsOverflowing: childBottom > innerBottom,
        BlockElement: child as HTMLElement,
        Page: page,
        PageBottom: innerBottom,
        ElementBottom: childBottom
    };
}

function findSplitIndex(textNode: Text, pageBottom: number): number {
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

        if (rect.bottom > pageBottom) {
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