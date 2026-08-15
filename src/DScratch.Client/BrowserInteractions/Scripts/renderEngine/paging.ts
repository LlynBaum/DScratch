export function update(modifiedNodes: HTMLElement[]) {
    const uniquePages = Array.from(
        new Set(
            modifiedNodes
                .map(n => n.closest<HTMLElement>(".page"))
                .filter((node): node is HTMLElement => node !== null)
        )
    );
    
    const sortedPages = uniquePages.sort((a, b) => {
        const aIdx = Number(a.getAttribute("data-page-index") ?? Infinity);
        const bIdx = Number(b.getAttribute("data-page-index") ?? Infinity);
        return bIdx - aIdx;
    });

    if (sortedPages.length === 0) return;
    greedyFlow(sortedPages);
}

function greedyFlow(modifiedPages: HTMLElement[]) {
    let currentPage;
    while (currentPage = modifiedPages.pop()) {
        const overflow = getBottomOverflowingChildren(currentPage);
        if (!overflow) continue;

        // TODO: move overflow to next page. Never cut in middle of word tho
        
        currentPage.nextElementSibling && modifiedPages.push(currentPage.nextElementSibling as HTMLElement);
    }
}

function getBottomOverflowingChildren(page: HTMLElement) {
    const style = window.getComputedStyle(page);
    const borderBottom = parseFloat(style.borderBottomWidth) || 0; // Take bottom margin into account
    const innerBottom = page.getBoundingClientRect().bottom - borderBottom;

    const child = page.lastElementChild;
    if (!child) return null;
    const isOverflowing = child.getBoundingClientRect().bottom > innerBottom;
    return isOverflowing ? child : null;
}