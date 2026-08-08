import {getEditorSelection, SelectionInfo, setSelection} from "./selection";

let selectionSnapshot: SelectionInfo | null = null;

export function registerMenu() {
    const editorMenu = document.querySelector<HTMLElement>(".editor > .editor-menu");
    if(!editorMenu){
        console.error("no editor menu found.");
        return;
    }
    
    editorMenu.addEventListener("click", e => {
       if((e.target as HTMLElement).closest("[data-snapshot-selection]")) {
           selectionSnapshot = getEditorSelection();
       }

        if((e.target as HTMLElement).closest("[data-restore-selection]")) {
            selectionSnapshot && setSelection(selectionSnapshot);
        }
    });
}