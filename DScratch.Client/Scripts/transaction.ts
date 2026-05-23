enum StepType {
    insertText = "insertText",
    deleteText = "deleteText",
    insertElementInline = "insertElementInline",
    insertElementBlock = "insertElementBlock",
    deleteElement = "deleteElement",
    moveInline = "moveInline",
    moveBlock = "moveBlock",
}

interface Step {
    type: StepType;
}

interface InsertTextStep extends Step {
    parent: string[];
    offset: number;
    text: string;
}

interface DeleteTextStep extends Step {
    parent: string[]; 
    offset: number; 
    length: number;
}

interface InsertElementInlineStep extends Step {
    parent: string[];
    offset: number;
    tagName: string;
    newNodeId: string;
}

interface InsertElementBlockStep extends Step {
    parent: string[];
    previousSibling: string[] | null;
    tagName: string;
    newNodeId: string;
}

interface DeleteElementStep extends Step {
    path: string[];
}

interface MoveInlineStep extends Step {
    targetNodePath: string[];
    targetParentPath: string[];
    targetOffset: number;
}

interface MoveBlockStep extends Step {
    targetNodePath: string[];
    targetParentPath: string[];
    previousSibling: string[] | null;
}

export function applyTransaction(transaction: Step[]){
    transaction.map(handle);
    
    function handle(step: Step) {
        switch (step.type) {
            case StepType.insertText:
                handleInsertTextStep(step as InsertTextStep);
                break;
            case StepType.deleteText:
                handleDeleteTextStep(step as DeleteTextStep);
                break
            case StepType.insertElementInline:
                handleInsertElementInlineStep(step as InsertElementInlineStep);
                break;
            case StepType.insertElementBlock:
                handleInsertElementBlockStep(step as InsertElementBlockStep);
                break;
            case StepType.deleteElement:
                handleDeleteElementStep(step as DeleteElementStep);
                break;
            case StepType.moveInline:
                handleMoveInlineStep(step as MoveInlineStep);
                break;
            case StepType.moveBlock:
                handleMoveBlockStep(step as MoveBlockStep);
                break;
        }
    }
}

function handleInsertTextStep(step: InsertTextStep) {
    const element = findNode(step.parent);
    if (!element) return;

    const { node, relativeOffset } = findTextNodeAtOffset(element, step.offset);
    if(node) {
        const text = node.textContent;
        node.textContent = text.slice(0, relativeOffset) + step.text + text.slice(relativeOffset);
        setSelection(node, relativeOffset + 1);
        
    } else {
        const createdNode = document.createTextNode(step.text);
        element.appendChild(createdNode);
        setSelection(createdNode, step.text.length);
    }
}

function handleDeleteTextStep(step: DeleteTextStep) {
    const element = findNode(step.parent);
    if (!element) return;

    const { node, relativeOffset } = findTextNodeAtOffset(element, step.offset);
    if(node) {
        const text = node.textContent;
        node.textContent = text.slice(0, relativeOffset) + text.slice(relativeOffset + step.length);
        setSelection(node, relativeOffset);
    }
}

function handleInsertElementInlineStep(step: InsertElementInlineStep) {
    const parent = findNode(step.parent);
    if (!parent) return;

    const element = createElement(step.tagName, step.newNodeId);
    insertElementInline(element, parent, step.offset);
    setSelection(element, 0);
}

function handleInsertElementBlockStep(step: InsertElementBlockStep) {
    const parent = findNode(step.parent);
    if (!parent) return;

    const previousSibling = step.previousSibling && findNode(step.previousSibling);

    const element = createElement(step.tagName, step.newNodeId);
    insertElementBlock(element, parent, previousSibling);
}

function handleDeleteElementStep(step: DeleteElementStep) {
    const element = findNode(step.path);
    if (!element) return;
    
    element.remove();
   // TODO: this might work out of the box of the browser, but check. Else seek for the a previous text node and set selection there. 
}

function handleMoveInlineStep(step: MoveInlineStep) {
    const element = findNode(step.targetNodePath);
    const newParent = findNode(step.targetParentPath);
    if (element && newParent) {
        insertElementInline(element, newParent, step.targetOffset);
    }
    // TODO: this might work out of the box of the browser, but check. Else seek for the a previous text node and set selection there.
}

function handleMoveBlockStep(step: MoveBlockStep) {
    const element = findNode(step.targetNodePath);
    const newParent = findNode(step.targetParentPath);
    if (element && newParent) {
        const previousSibling = step.previousSibling && findNode(step.previousSibling);
        insertElementBlock(element, newParent, previousSibling);
    }
    // TODO: this might work out of the box of the browser, but check. Else seek for the a previous text node and set selection there.
}

function createElement(tagName: string, id: string) {
    const element = document.createElement(tagName);
    element.setAttribute("data-path-id", id);
    return element;
}

function insertElementInline(element: Element, parent: Element, offset: number) {
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

function insertElementBlock(element: Element, parent: Element, previousSibling: Element | null) {
    const referenceNode = previousSibling ? previousSibling.nextSibling : parent.firstChild;
    parent.insertBefore(element, referenceNode);
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

function setSelection(node: Node, offset: number) {
    const selection = window.getSelection();
    selection?.removeAllRanges();

    const range = document.createRange();
    range.setStart(node, offset);
    range.collapse(true);
    selection?.addRange(range)
}