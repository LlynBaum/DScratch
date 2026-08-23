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
    const modifiedElements: HTMLElement[] = [];
    transaction.steps.map(handle);
    paging.update(modifiedElements);
    if (transaction.cursorPosition) {
        setSelectionSave(transaction.cursorPosition);
    }

    function handle(step?: Step | null) {
        if(!step) return null;
        switch (step.type) {
            case StepType.insertText: {
                const element = handleInsertTextStep(step as InsertTextStep);
                element && modifiedElements.push(element);
                break;
            }
            case StepType.deleteText: {
                const element = handleDeleteTextStep(step as DeleteTextStep);
                element && modifiedElements.push(element);
                break;
            }
            case StepType.insertElement: {
                const element = handleInsertElementStep(step as InsertElementStep);
                element && modifiedElements.push(element);
                break;
            }
            case StepType.deleteElement: {
                const elements = handleDeleteElementStep(step as DeleteElementStep);
                modifiedElements.push(...elements);
                break;
            }
            case StepType.move: {
                const elements = handleMoveStep(step as MoveStep);
                modifiedElements.push(...elements);
                break;
            }
            case StepType.updateMarks: {
                const elements = handleUpdateMarksStep(step as UpdateMarksStep);
                modifiedElements.push(...elements);
                break;
            }
            case StepType.updateAttributes: {
                const elements = handleUpdateAttributesStep(step as UpdateAttributes);
                modifiedElements.push(...elements);
                break;
            }
            default:
                console.error("Unknown step type.");
                break;
        }
    }
}

function handleInsertTextStep(step: InsertTextStep) {
    const element = findNode(step.parentId);
    if (!element) return null;
    
    const { node, relativeOffset } = nodeHelper.findTextNodeAtOffset(element, step.offset);
    if (node) {
        const text = node.textContent;
        node.textContent = text!.slice(0, relativeOffset) + step.text + text!.slice(relativeOffset);
        return element;
    }

    const targetParent = nodeHelper.getSplitCounterPart(element) ?? element;
    const createdNode = document.createTextNode(step.text);
    targetParent.appendChild(createdNode);
    return targetParent as HTMLElement;
}

function handleDeleteTextStep(step: DeleteTextStep) {
    const element = findNode(step.parentId);
    if (!element) return null;
    
    // TODO: when deleting selection over different pages, only have to take into account that there might be more text somewhere else
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
    const parent = findNodeLast(step.parentId);
    if (!parent) return null;

    const previousSibling = step.previousSiblingId ? findNodeLast(step.previousSiblingId) : null;

    const element = createElement(step.tagName, step.newNodeId, step.attributes);
    insertElement(element, parent, previousSibling);
    return element;
}

function handleDeleteElementStep(step: DeleteElementStep) {
    const elements = findNodeAll(step.targetId);
    elements.forEach(e => e.remove());
    return elements;
}

function handleMoveStep(step: MoveStep) {
    const elements = findNodeAll(step.targetNodeId);
    const newParent = findNodeLast(step.targetParentId);
    
    if (elements.length > 0 && newParent) {
        let previousSibling = step.previousSiblingId ? findNodeLast(step.previousSiblingId) : null;
        elements.forEach(element => {
            insertElement(element, newParent, previousSibling);
            previousSibling = element;
        });
    }
    return elements;
}

function handleUpdateMarksStep(step: UpdateMarksStep) {
    const elements = findNodeAll(step.nodeId);
    
    elements.forEach(element => {
        element.style = '';
        for (let marksKey in step.marks) {
            // @ts-ignore / we trust C# to send valid CSS properties
            element.style[marksKey] = step.marks[marksKey];
        }
    });
    
    return elements;
}

function handleUpdateAttributesStep(step: UpdateAttributes) {
    const elements = findNodeAll(step.nodeId);
    
    elements.forEach(element => {
        for (let attr in element.attributes) {
            if (attr === "style" || attr === "data-dnode-id") continue;
            element.removeAttribute(attr);
        }

        for (let attr in step.attributes){
            element.setAttribute(attr, step.attributes[attr]);
        }
    });
    
    return elements;
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

function findNodeLast(nodeId: string) : HTMLElement | null {
    const element = nodeHelper.findNodeLast(nodeId);
    if(!element) {
        console.error(new Error(`Could not find node '${nodeId}'.`));
    }
    return element;
}

function findNodeAll(nodeId: string) {
    const element = nodeHelper.findNodeAll(nodeId);
    if(!element) {
        console.error(new Error(`Could not find node '${nodeId}'.`));
    }
    return element;
}
