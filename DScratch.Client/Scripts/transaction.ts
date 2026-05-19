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
    
    const text = element.innerText;
    element.innerText = text.slice(0, step.Offset) + step.Text + text.slice(step.Offset + 1);
}

function handleDeleteTextDiff(step: DeleteTextDiff) {
    const element = findNode(step.Parent);
    if (!element) return;

    const text = element.innerText;
    element.innerText = text.slice(0, step.Offset) + text.slice(step.Offset + step.Length);
}

function handleInsertElementDiff(step: InsertElementDiff) {
    
}

function handleDeleteElementDiff(step: DeleteElementDiff) {

}

function handleMoveDiff(step: MoveDiff) {

}

function findNode(path: string[]) : HTMLElement | null {
    const query = path.map(p => `[${p}]`).join(" ");
    const element = document.querySelector<HTMLElement>(query);
    if(!element) {
        console.error(`Could not find node at path '${query}'.`);
    }
    return element;
}
