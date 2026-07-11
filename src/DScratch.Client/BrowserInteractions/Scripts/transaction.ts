import {saveSelection, SelectionInfo, setSelection} from "./selection";
import {findTextNodeAtOffset} from "./nodeHelper";

enum StepType {
    insertText = "insertText",
    deleteText = "deleteText",
    insertElement = "insertElement",
    deleteElement = "deleteElement",
    move = "move",
    updateMarks = "updateMarks"
}

export interface TransactionResult {
    steps: Array<Step | null | undefined>;
    cursorPosition: SelectionInfo | null;
}

interface Step {
    type: StepType;
}

interface InsertTextStep extends Step {
    parentId: string;
    offset: number;
    text: string;
}

interface DeleteTextStep extends Step {
    parentId: string; 
    offset: number; 
    length: number;
}

interface InsertElementStep extends Step {
    parentId: string;
    previousSiblingId: string | null;
    tagName: string;
    newNodeId: string;
}

interface DeleteElementStep extends Step {
    targetId: string;
}

interface MoveStep extends Step {
    targetNodeId: string;
    targetParentId: string;
    previousSiblingId: string | null;
}

interface UpdateMarksStep extends Step {
    nodeId: string;
    marks: { [key:string] : string; };
}

export function applyTransaction(transaction: TransactionResult){
    saveSelection();
    transaction.steps.map(handle);
    if (transaction.cursorPosition) {
        setSelection(transaction.cursorPosition);
    }

    function handle(step?: Step | null) {
        if(!step) return;
        switch (step.type) {
            case StepType.insertText:
                handleInsertTextStep(step as InsertTextStep);
                break;
            case StepType.deleteText:
                handleDeleteTextStep(step as DeleteTextStep);
                break
            case StepType.insertElement:
                handleInsertElementBlockStep(step as InsertElementStep);
                break;
            case StepType.deleteElement:
                handleDeleteElementStep(step as DeleteElementStep);
                break;
            case StepType.move:
                handleMoveBlockStep(step as MoveStep);
                break;
            case StepType.updateMarks:
                handleUpdateMarksStep(step as UpdateMarksStep);
                break;
        }
    }
}

function handleInsertTextStep(step: InsertTextStep) {
    const element = findNode(step.parentId);
    if (!element) return;
    
    const { node, relativeOffset } = findTextNodeAtOffset(element, step.offset);
    if(node) {
        const text = node.textContent;
        node.textContent = text.slice(0, relativeOffset) + step.text + text.slice(relativeOffset);
    } else {
        const createdNode = document.createTextNode(step.text);
        element.appendChild(createdNode);
    }
}

function handleDeleteTextStep(step: DeleteTextStep) {
    const element = findNode(step.parentId);
    if (!element) return;
    
    const { node, relativeOffset } = findTextNodeAtOffset(element, step.offset);
    if(node) {
        const text = node.textContent;
        node.textContent = text.slice(0, relativeOffset) + text.slice(relativeOffset + step.length);
    }
    
    // TODO: test if that really works. Should clean up any empty spans
    if (element.childNodes.length == 0) {
        element.remove();
    }
}

function handleInsertElementBlockStep(step: InsertElementStep) {
    const parent = findNode(step.parentId);
    if (!parent) return;

    const previousSibling = step.previousSiblingId ? findNode(step.previousSiblingId) : null;

    const element = createElement(step.tagName, step.newNodeId);
    insertElementBlock(element, parent, previousSibling);
}

function handleDeleteElementStep(step: DeleteElementStep) {
    const element = findNode(step.targetId);
    if (!element) return;

    element.remove();
}

function handleMoveBlockStep(step: MoveStep) {
    const element = findNode(step.targetNodeId);
    const newParent = findNode(step.targetParentId);
    if (element && newParent) {
        const previousSibling = step.previousSiblingId ? findNode(step.previousSiblingId) : null;
        insertElementBlock(element, newParent, previousSibling);
    }
}

function handleUpdateMarksStep(step: UpdateMarksStep) {
    const element = findNode(step.nodeId);
    if(!element) return;
    
    element.style = '';
    for (let marksKey in step.marks) {
        // @ts-ignore / we trust C# to send valid CSS properties
        element.style[marksKey] = step.marks[marksKey];
    }
}

function createElement(tagName: string, id: string) {
    const element = document.createElement(tagName);
    element.setAttribute("data-dnode-id", id);
    return element;
}

function insertElementBlock(element: Element, parent: Element, previousSibling: Element | null) {
    const referenceNode = previousSibling ? previousSibling.nextSibling : parent.firstChild;
    parent.insertBefore(element, referenceNode);
}

function findNode(nodeId: string) : HTMLElement | null {
    const element = document.querySelector<HTMLElement>(`[data-dnode-id='${nodeId}']`);
    if(!element) {
        console.error(`Could not find node '${nodeId}'.`);
    }
    return element;
}
