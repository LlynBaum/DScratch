enum StepType {
    insertText = "insertText",
    deleteText = "deleteText",
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
                break
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

    const { node, relativeOffset } = findTextNodeAtOffset(element, step.Offset);
    if(node) {
        const text = node.textContent;
        node.textContent = text.slice(0, relativeOffset) + step.Text + text.slice(relativeOffset);
        
    } else {
        element.appendChild(document.createTextNode(step.Text));
    }
}

function handleDeleteTextStep(step: DeleteTextStep) {
    const element = findNode(step.Parent);
    if (!element) return;

    const { node, relativeOffset } = findTextNodeAtOffset(element, step.Offset);
    if(node) {
        const text = node.textContent;
        node.textContent = text.slice(0, relativeOffset) + text.slice(relativeOffset + step.Length);
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

function insertElement(element: Element, parent: Element, offset: number) {
    if(!parent.hasChildNodes()){
        parent.appendChild(element);
        return;
    }

    const { node, relativeOffset } = findTextNodeAtOffset(parent, offset);
    if (node) {
        const remainingTextNode = node.splitText(relativeOffset);
        node.parentNode?.insertBefore(element, remainingTextNode);
    } else {
        parent.appendChild(element);
    }
}

function findNode(path: string[]) : HTMLElement | null {
    const query = path.map(p => `[data-path-id='${p}']`).join(" ");
    const element = document.querySelector<HTMLElement>(query);
    if(!element) {
        console.error(`Could not find node at path '${query}'.`);
    }
    return element;
}

function findTextNodeAtOffset(parent: Element, offset: number){
    const walker = document.createTreeWalker(parent, NodeFilter.SHOW_TEXT);
    
    let currentOffset = 0;
    let currentNode = walker.nextNode() as Text | null;
    
    while (currentNode) {
        const nodeLength = currentNode.textContent?.length || 0;

        if (currentOffset + nodeLength >= offset) {
            return { node: currentNode, relativeOffset: offset - currentOffset };
        }

        currentOffset += nodeLength;
        currentNode = walker.nextNode() as Text | null;
    }
    return { node: null, relativeOffset: 0 };
}