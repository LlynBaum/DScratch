enum StepType {
    insertText = "insertText",
    deleteText = "deleteText",
    splitText = "splitText", // TODO: maybe useful as well?
    insertElement = "insertElement",
    deleteElement = "deleteElement",
    move = "move",
}

interface Step {
    Type: StepType;
}

interface InsertTextStep extends Step {
    Parent: string[];
    Offset: number;
    Text: string;
}

interface DeleteTextStep extends Step {
    Parent: string[]; 
    Offset: number; 
    Length: number;
}

interface SplitTextStep extends Step {
    TargetNodePath: string[];
    Offset: number;
}

interface InsertElementStep extends Step {
    Parent: string[];
    Offset: number;
    TagName: string;
    NewNodeId: string;
}

interface DeleteElementStep extends Step {
    Path: string[];
}

interface MoveStep extends Step {
    TargetNodePath: string[];
    TargetParentPath: string[];
    TargetOffset: number;
}

export function applyTransaction(transaction: Step[]){
    transaction.map(handle);
    
    function handle(step: Step) {
        switch (step.Type) {
            case StepType.insertText:
                handleInsertTextStep(step as InsertTextStep);
                break;
            case StepType.deleteText:
                handleDeleteTextStep(step as DeleteTextStep);
                break;
            case StepType.splitText:
                handleSplitTextStep(step as SplitTextStep);
                break;
            case StepType.insertElement:
                handleInsertElementStep(step as InsertElementStep);
                break;
            case StepType.deleteElement:
                handleDeleteElementStep(step as DeleteElementStep);
                break;
            case StepType.move:
                handleMoveStep(step as MoveStep);
                break;
        }
    }
}

function handleInsertTextStep(step: InsertTextStep) {
    const element = findNode(step.Parent);
    if (!element) return;

    // TODO: <p> abc <b> def </b> ghi </p>     insert text here does not work currently - maybe unify with insert elements
    const text = element.innerText;
    element.innerText = text.slice(0, step.Offset) + step.Text + text.slice(step.Offset);
}

function handleDeleteTextStep(step: DeleteTextStep) {
    const element = findNode(step.Parent);
    if (!element) return;

    // TODO: <p> abc <b> def </b> ghi </p>     removing text here does not work currently - maybe unify with delete elements
    const text = element.innerText;
    element.innerText = text.slice(0, step.Offset) + text.slice(step.Offset + step.Length);
}

function handleSplitTextStep(step: SplitTextStep) {
    const element = findNode(step.TargetNodePath);
    if (!element) return;

    // TODO: split only allows to split text, everything else can be done with move. But does it do that xD
    // TODO: maybe do it fancy like insertElement...?
    // TODO: maybe instead of split, move can take a length and only moves the length of the text... or maybe just a moveText that moves the text
    if(element instanceof Text) {
        element.splitText(step.Offset);
    }
}

function handleInsertElementStep(step: InsertElementStep) {
    const parent = findNode(step.Parent);
    if (!parent) return;
    
    const element = document.createElement(step.TagName);
    element.setAttribute("data-path-id", step.NewNodeId);
    insertElement(element, parent, step.Offset);
}

function handleDeleteElementStep(step: DeleteElementStep) {
    const element = findNode(step.Path);
    if (!element) return;
    
    element.remove();
}

function handleMoveStep(step: MoveStep) {
    const element = findNode(step.TargetNodePath);
    if (!element) return;
    
    const newParent = findNode(step.TargetParentPath);
    if (!newParent) return;
    
    element.remove();
    insertElement(element, newParent, step.TargetOffset);
}

// TODO: really that fancy. I mean it HAS to be the given parent. Everything else might break my C# stuff
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
