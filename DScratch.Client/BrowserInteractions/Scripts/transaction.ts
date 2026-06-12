import {saveSelection, setSelection} from "./selection";
import {findTextNodeAtOffset} from "./nodeHelper";

enum StepType {
    insertText = "insertText",
    deleteText = "deleteText",
    insertElementInline = "insertElementInline",
    insertElementBlock = "insertElementBlock",
    deleteElement = "deleteElement",
    moveInline = "moveInline",
    moveBlock = "moveBlock",
}

interface CursorPosition {
    parentId: string;
    offset: number;
}

export interface TransactionResult {
    steps: Array<Step | null | undefined>;
    cursorPosition: CursorPosition | null;
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

interface InsertElementInlineStep extends Step {
    parentId: string;
    offset: number;
    tagName: string;
    newNodeId: string;
}

interface InsertElementBlockStep extends Step {
    parentId: string;
    previousSiblingId: string | null;
    tagName: string;
    newNodeId: string;
}

interface DeleteElementStep extends Step {
    targetId: string;
}

interface MoveInlineStep extends Step {
    targetNodeId: string;
    targetParentId: string;
    targetOffset: number;
}

interface MoveBlockStep extends Step {
    targetNodeId: string;
    targetParentId: string;
    previousSiblingId: string | null;
}

export function applyTransaction(transaction: TransactionResult){
    saveSelection();
    transaction.steps.map(handle);
    if (transaction.cursorPosition) {
        setSelection(transaction.cursorPosition.parentId, transaction.cursorPosition.offset);
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
    const element = findNode(step.parentId);
    if (!element) return;
    
    // TODO: browser combines multiple spaces into one, can I force it to render all of them?
    
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
}

function handleInsertElementInlineStep(step: InsertElementInlineStep) {
    const parent = findNode(step.parentId);
    if (!parent) return;

    const element = createElement(step.tagName, step.newNodeId);
    insertElementInline(element, parent, step.offset);
}

function handleInsertElementBlockStep(step: InsertElementBlockStep) {
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
   // TODO: this might work out of the box of the browser, but check. Else seek for the a previous text node and set selection there.
}

function handleMoveInlineStep(step: MoveInlineStep) {
    const element = findNode(step.targetNodeId);
    const newParent = findNode(step.targetParentId);
    if (element && newParent) {
        insertElementInline(element, newParent, step.targetOffset);
    }
    // TODO: this might work out of the box of the browser, but check. Else seek for the a previous text node and set selection there.
}

function handleMoveBlockStep(step: MoveBlockStep) {
    const element = findNode(step.targetNodeId);
    const newParent = findNode(step.targetParentId);
    if (element && newParent) {
        const previousSibling = step.previousSiblingId ? findNode(step.previousSiblingId) : null;
        insertElementBlock(element, newParent, previousSibling);
    }
    // TODO: this might work out of the box of the browser, but check. Else seek for the a previous text node and set selection there.
}

function createElement(tagName: string, id: string) {
    const element = document.createElement(tagName);
    element.setAttribute("data-dnode-id", id);
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

function findNode(nodeId: string) : HTMLElement | null {
    const element = document.querySelector<HTMLElement>(`[data-dnode-id='${nodeId}']`);
    if(!element) {
        console.error(`Could not find node '${nodeId}'.`);
    }
    return element;
}
