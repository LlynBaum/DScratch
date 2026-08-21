import {saveSelection, SelectionInfo, setSelectionSave} from "../selection";
import * as nodeHelper from "../nodeHelper";
import * as paging from "./paging";

enum StepType {
    insertText = "insertText",
    deleteText = "deleteText",
    insertElement = "insertElement",
    deleteElement = "deleteElement",
    move = "move",
    updateMarks = "updateMarks",
    updateAttributes = "updateAttributes"
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
    attributes: { [key:string] : string; } | null;
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

interface UpdateAttributes extends Step {
    nodeId: string;
    attributes: { [key:string] : string; };
}

export function applyTransaction(transaction: TransactionResult){
    saveSelection();
    const modifiedElements = transaction.steps.map(handle).filter(e => !!e);
    paging.update(modifiedElements);
    if (transaction.cursorPosition) {
        setSelectionSave(transaction.cursorPosition);
    }

    function handle(step?: Step | null) {
        if(!step) return null;
        switch (step.type) {
            case StepType.insertText:
                return handleInsertTextStep(step as InsertTextStep);
            case StepType.deleteText:
                return handleDeleteTextStep(step as DeleteTextStep);
            case StepType.insertElement:
                return handleInsertElementStep(step as InsertElementStep);
            case StepType.deleteElement:
                return handleDeleteElementStep(step as DeleteElementStep);
            case StepType.move:
                return handleMoveStep(step as MoveStep);
            case StepType.updateMarks:
                return handleUpdateMarksStep(step as UpdateMarksStep);
            case StepType.updateAttributes:
                return handleUpdateAttributesStep(step as UpdateAttributes);
            default:
                console.error("Unknown step type.");
                return null;
        }
    }
}

function handleInsertTextStep(step: InsertTextStep) {
    const element = findNode(step.parentId);
    if (!element) return null;
    
    const { node, relativeOffset } = nodeHelper.findTextNodeAtOffset(element, step.offset);
    if(node) {
        const text = node.textContent;
        node.textContent = text!.slice(0, relativeOffset) + step.text + text!.slice(relativeOffset);
    } else {
        const createdNode = document.createTextNode(step.text);
        element.appendChild(createdNode);
    }
    
    return element;
}

function handleDeleteTextStep(step: DeleteTextStep) {
    const element = findNode(step.parentId);
    if (!element) return null;
    
    const { node, relativeOffset } = nodeHelper.findTextNodeAtOffset(element, step.offset);
    if(node) {
        const text = node.textContent;
        node.textContent = text!.slice(0, relativeOffset) + text!.slice(relativeOffset + step.length);
    }
    
    if (element.childNodes.length == 0) {
        element.remove();
    }
    
    return element;
}

function handleInsertElementStep(step: InsertElementStep) {
    const parent = step.previousSiblingId 
        ? findNodeWithChild(step.parentId, step.previousSiblingId) 
        : findNode(step.parentId);
    
    if (!parent) return null;

    const previousSibling = step.previousSiblingId ? findNode(step.previousSiblingId) : null;

    const element = createElement(step.tagName, step.newNodeId, step.attributes);
    insertElement(element, parent, previousSibling);
    return element;
}

function handleDeleteElementStep(step: DeleteElementStep) {
    const element = findNode(step.targetId);
    if (!element) return null;

    element.remove();
    return element;
}

function handleMoveStep(step: MoveStep) {
    const element = findNode(step.targetNodeId);
    const newParent = step.previousSiblingId
        ? findNodeWithChild(step.targetParentId, step.previousSiblingId)
        : findNode(step.targetParentId);
    
    if (element && newParent) {
        const previousSibling = step.previousSiblingId ? findNode(step.previousSiblingId) : null;
        insertElement(element, newParent, previousSibling);
    }
    return element;
}

function handleUpdateMarksStep(step: UpdateMarksStep) {
    const element = findNode(step.nodeId);
    if(!element) return null;
    
    element.style = '';
    for (let marksKey in step.marks) {
        // @ts-ignore / we trust C# to send valid CSS properties
        element.style[marksKey] = step.marks[marksKey];
    }
    return element;
}

function handleUpdateAttributesStep(step: UpdateAttributes) {
    const element = findNode(step.nodeId);
    if(!element) return null;
    
    for (let attr in element.attributes) {
        if (attr === "style" || attr === "data-dnode-id") continue;
        element.removeAttribute(attr);
    }
    
    for (let attr in step.attributes){
        element.setAttribute(attr, step.attributes[attr]);
    }
    
    return element;
}

function createElement(tagName: string, id: string, attributes: { [key:string] : string; } | null) {
    const element = document.createElement(tagName);
    element.setAttribute("data-dnode-id", id);

    for (let atr in attributes) {
        element.setAttribute(atr, attributes[atr])
    }
    
    return element;
}

function insertElement(element: Element, parent: Element, previousSibling: Element | null) {
    const referenceNode = previousSibling ? previousSibling.nextSibling : parent.firstChild;
    parent.insertBefore(element, referenceNode);
}

function findNode(nodeId: string) : HTMLElement | null {
    const element = nodeHelper.findNode(nodeId);
    if(!element) {
        console.error(new Error(`Could not find node '${nodeId}'.`));
    }
    return element;
}

function findNodeWithChild(parentId: string, childId: string) : HTMLElement | null {
    const element = nodeHelper.findNodeWithChild(parentId, childId);
    if(!element) {
        console.error(new Error(`Could not find node '${parentId}' with a child ${childId}.`));
    }
    return element;
}
