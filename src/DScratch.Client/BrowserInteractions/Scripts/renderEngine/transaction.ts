import {saveSelection, SelectionInfo, setSelectionSave} from "../selection";
import * as nodeHelper from "../nodeHelper";
import * as paging from "./paging";
import {getSplitPartIndex} from "../nodeHelper";

export enum StepType {
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

export interface Step {
    type: StepType;
}

export interface InsertTextStep extends Step {
    parentId: string;
    offset: number;
    text: string;
}

export interface DeleteTextStep extends Step {
    parentId: string; 
    offset: number; 
    length: number;
}

export interface InsertElementStep extends Step {
    parentId: string;
    previousSiblingId: string | null;
    tagName: string;
    newNodeId: string;
    attributes: { [key:string] : string; } | null;
}

export interface DeleteElementStep extends Step {
    targetId: string;
}

export interface MoveStep extends Step {
    targetNodeId: string;
    targetParentId: string;
    previousSiblingId: string | null;
}

export interface UpdateMarksStep extends Step {
    nodeId: string;
    marks: { [key:string] : string; };
}

export interface UpdateAttributes extends Step {
    nodeId: string;
    attributes: { [key:string] : string; };
}

export function applyTransaction(transaction: TransactionResult){
    saveSelection();
    const modifiedElements: Element[] = [];
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
                element && modifiedElements.push(...element);
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
    
    // TODO: maybe findTextNodeAtOffset should take the NodeId instead of an element. 
    const { node, relativeOffset } = nodeHelper.findTextNodeAtOffset(element, step.offset);
    if (node) {
        const text = node.textContent;
        node.textContent = text!.slice(0, relativeOffset) + step.text + text!.slice(relativeOffset);
        return node.nodeType === Node.ELEMENT_NODE ? node as Element : node.parentElement;
    }

    const targetParent = findLastNode(step.parentId)!;
    const createdNode = document.createTextNode(step.text);
    targetParent.appendChild(createdNode);
    return targetParent as HTMLElement;
}

function handleDeleteTextStep(step: DeleteTextStep): Element[] | null {
    const element = findNode(step.parentId);
    if (!element) return null;

    const start = nodeHelper.findTextNodeAtOffset(element, step.offset);
    const end = nodeHelper.findTextNodeAtOffset(element, step.offset + step.length);
    
    if (!start.node || !end.node) {
        return null;
    }
    
    if (start.node === end.node) {
        const text = start.node.textContent;
        start.node.textContent = text!.slice(0, start.relativeOffset) + text!.slice(start.relativeOffset + step.length);

        const parentElement = start.node.parentElement!;
        if (parentElement.textContent.length === 0) {
            parentElement.remove();
        }
        
        return [parentElement];
    }

    start.node.textContent = start.node.textContent!.slice(0, start.relativeOffset);
    end.node.textContent = end.node.textContent!.slice(start.relativeOffset);
    
    let currentNode = start.node.nextSibling;
    while (currentNode || currentNode === end.node) {
        const n = currentNode;
        currentNode = n.nextSibling;
        n.remove();
    }
    
    const elements = nodeHelper.getAllNodes(element);
    
    if (elements.length > 1) {
        const stratPartIndex = getSplitPartIndex(start.node)!;
        const endPartIndex = getSplitPartIndex(end.node)!;
        elements.filter(n => {
            const index = getSplitPartIndex(n)!;
            return stratPartIndex < index && index < endPartIndex;
        }).forEach(e => e.remove());
    }
    
    const startElement = start.node.parentElement!;
    const endElement = end.node.parentElement!;
    
    if (startElement?.textContent.length === 0) {
        startElement.remove();
    }

    if (startElement === endElement) {
        return [startElement];
    }

    if (endElement?.textContent.length === 0) {
        endElement.remove();
    }
    return [startElement, endElement];
}

function handleInsertElementStep(step: InsertElementStep) {
    const parent = step.previousSiblingId 
        ? findLastNodeWithSibling(step.parentId, step.previousSiblingId) 
        : findLastNode(step.parentId);
     
    if (!parent) return null;

    const previousSibling = step.previousSiblingId 
        ? findNodeIn(parent, step.previousSiblingId) 
        : null;

    const element = createElement(step.tagName, step.newNodeId, step.attributes);
    insertElement(element, parent, previousSibling);
    return element;
}

function handleDeleteElementStep(step: DeleteElementStep) {
    const elements = findAllNode(step.targetId);
    elements.forEach(e => e.remove());
    return elements;
}

function handleMoveStep(step: MoveStep) {
    const elements = findAllNode(step.targetNodeId);
    const newParent = step.previousSiblingId
        ? findLastNodeWithSibling(step.targetParentId, step.previousSiblingId)
        : findLastNode(step.targetParentId);
    
    if (elements.length > 0 && newParent) {
        let previousSibling = step.previousSiblingId ? findNodeIn(newParent, step.previousSiblingId) : null;
        elements.forEach(element => {
            insertElement(element, newParent, previousSibling);
            previousSibling = element;
        });
    }
    return elements;
}

function handleUpdateMarksStep(step: UpdateMarksStep) {
    const elements = findAllNode(step.nodeId);
    
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
    const elements = findAllNode(step.nodeId);
    
    elements.forEach(element => {
        for (let attr of element.attributes) {
            if (attr.name === "style" || attr.name === "data-dnode-id") continue;
            element.removeAttribute(attr.name);
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
    const referenceNode = previousSibling ? previousSibling.nextElementSibling : parent.firstElementChild;
    parent.insertBefore(element, referenceNode);
}

function findNode(nodeId: string) : HTMLElement | null {
    const element = nodeHelper.findNode(nodeId);
    if(!element) {
        console.error(new Error(`Could not find node '${nodeId}'.`));
    }
    return element;
}

function findNodeIn(parent: Element, nodeId: string){
    const element = nodeHelper.findNodeIn(parent, nodeId);
    if(!element) {
        console.error(new Error(`Could not find node '${nodeId}' in element ${parent}.`));
    }
    return element;
}

function findLastNode(nodeId: string) : HTMLElement | null {
    const element = nodeHelper.findLastNode(nodeId);
    if(!element) {
        console.error(new Error(`Could not find node '${nodeId}'.`));
    }
    return element;
}

function findLastNodeWithSibling(nodeId: string, siblingId: string) {
    const element = nodeHelper.findLastNodeWithSibling(nodeId, siblingId);
    if(!element) {
        console.error(new Error(`Could not find node '${nodeId}' with sibling '${siblingId}'.`));
    }
    return element;
}

function findAllNode(nodeId: string) {
    const element = nodeHelper.findAllNode(nodeId);
    if(!element) {
        console.error(new Error(`Could not find node '${nodeId}'.`));
    }
    return element;
}
