import {findNode} from "../nodeHelper";

type PageNumberByNode = Array<{ nodeId: string, pageNumber: number }>;

export type GetPageNumbersFunc = (nodeIds: string[]) => PageNumberByNode;


export function getPageNumbers(nodeIds: string[]): PageNumberByNode {
    const result: PageNumberByNode = [];
    
    for (let nodeId in nodeIds) {
        const page = findNode(nodeId)?.closest(".page");
        if (!page) {
            console.error(`Node ${nodeId} is not part of a page.`);
            continue;
        }
        
        const pageNumber = page.getAttribute("data-page-index");
        result.push({ nodeId, pageNumber: Number(pageNumber) });
    }
    
    return result;
}