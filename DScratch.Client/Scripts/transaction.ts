enum StepType {
    insertText = "insertText",
    deleteText = "deleteText",
    insertElement = "insertElement",
    deleteElement = "deleteElement",
    move = "move"
}

interface StepDiff {
    Type: StepType;
}

interface InsertTextDiff extends StepDiff {
    Parent: string[];
    Offset: number;
    Text: string;
}

interface DeleteTextDiff extends StepDiff {
    Parent: string[]; 
    Offset: number; 
    Length: number;
}

interface InsertElementDiff extends StepDiff {
    Parent: string[];
    Offset: number;
    TagName: string;
    NewNodeId: string;
}

interface DeleteElementDiff extends StepDiff {
    Path: string[];
}

interface MoveDiff extends StepDiff {
    TargetNodePath: string[];
    TargetParentPath: string[];
    TargetOffset: number;
}

export function applyTransaction(transaction: StepDiff[]){
    transaction.map(handle);
    
    function handle(step: StepDiff) {
        switch (step.Type) {
            case StepType.insertText:
                handleInsertTextDiff(step as InsertTextDiff);
                break;
            case StepType.deleteText:
                handleDeleteTextDiff(step as DeleteTextDiff);
                break;
            case StepType.insertElement:
                handleInsertElementDiff(step as InsertElementDiff);
                break;
            case StepType.deleteElement:
                handleDeleteElementDiff(step as DeleteElementDiff);
                break;
            case StepType.move:
                handleMoveDiff(step as MoveDiff);
                break;
        }
    }
}

function handleInsertTextDiff(step: InsertTextDiff) {
    const element = findNode(step.Parent);
    if (!element) return;

    // TODO: <p> abc <b> def </b> ghi </p>     insert text here does not work currently - maybe unify with insert elements
    const text = element.innerText;
    element.innerText = text.slice(0, step.Offset) + step.Text + text.slice(step.Offset);
}

function handleDeleteTextDiff(step: DeleteTextDiff) {
    const element = findNode(step.Parent);
    if (!element) return;

    // TODO: <p> abc <b> def </b> ghi </p>     removing text here does not work currently - maybe unify with delete elements
    const text = element.innerText;
    element.innerText = text.slice(0, step.Offset) + text.slice(step.Offset + step.Length);
}

function handleInsertElementDiff(step: InsertElementDiff) {
    const parent = findNode(step.Parent);
    if (!parent) return;
    
    const element = document.createElement(step.TagName);
    element.setAttribute("data-path-id", step.NewNodeId);
    insertElement(element, parent, step.Offset);
}

function handleDeleteElementDiff(step: DeleteElementDiff) {
    const element = findNode(step.Path);
    if (!element) return;
    
    element.remove();
}

function handleMoveDiff(step: MoveDiff) {
    const element = findNode(step.TargetNodePath);
    if (!element) return;
    
    const newParent = findNode(step.TargetParentPath);
    if (!newParent) return;
    
    element.remove();
    insertElement(element, newParent, step.TargetOffset);
}

function insertElement(element: Element, parent: Element, offset: number) {
    if(!parent.hasChildNodes()){
        parent.appendChild(element);
        return;
    }

    let currentOffset = 0;
    let targetTextNode: Text | null = null;
    let relativeOffset = 0;

    const walker = document.createTreeWalker(parent, NodeFilter.SHOW_TEXT);
    let currentNode = walker.nextNode() as Text | null;

    while (currentNode) {
        const nodeLength = currentNode.textContent?.length || 0;

        if (currentOffset + nodeLength >= offset) {
            targetTextNode = currentNode;
            relativeOffset = offset - currentOffset;
            break;
        }

        currentOffset += nodeLength;
        currentNode = walker.nextNode() as Text | null;
    }

    if (targetTextNode) {
        const remainingTextNode = targetTextNode.splitText(relativeOffset);
        targetTextNode.parentNode?.insertBefore(element, remainingTextNode);
    } else {
        parent.appendChild(element);
    }
}

function findNode(path: string[]) : HTMLElement | null {
    const query = path.map(p => `[${p}]`).join(" ");
    const element = document.querySelector<HTMLElement>(query);
    if(!element) {
        console.error(`Could not find node at path '${query}'.`);
    }
    return element;
}
