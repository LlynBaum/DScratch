import { applyTransaction, TransactionResult } from "./transaction";
import { registerInput } from "./inputs";
import {getSelection, registerSelection, SelectionInfo} from "./selection";
import {registerMenu} from "./editorMenu";

interface Editor {
    bridgeReference: any;
    initialize: (dotNetRef: any) => void;
    applyTransaction: (transaction: TransactionResult) => void;
    getSelection: () => SelectionInfo;
    node: HTMLElement | null;
}

declare global {
    interface Window {
        editor: Editor;
    }
}

function initEditor(dotNetRef: any) {
    const editor = document.getElementById("doc-editor");
    window.editor.node = editor;
    window.editor.bridgeReference = dotNetRef;
    
    if (!editor) {
        console.error("There is no editor node.");
        return;
    }
    
    editor?.addEventListener("click", setCursorToEnd);
    registerInput();
    registerSelection();
    registerMenu();
    
    const rootNode = editor.querySelector<HTMLElement>("[data-dnode-id='Root']");
    rootNode?.setAttribute("contenteditable", '');

    console.info("editor ready!");
}

function setCursorToEnd(event: PointerEvent) {
    const element = event.target as HTMLElement;
    if (!element.hasAttribute("contenteditable")) {
        return;
    }
    
    const lastParagraph = element.querySelector<HTMLElement>("p:last-of-type")!;
    if (!lastParagraph) return;
    
    const selection = window.getSelection();
    const textNode = lastParagraph.firstChild || lastParagraph;
    const offset = textNode.nodeType === Node.TEXT_NODE
        ? (textNode.textContent?.length || 0)
        : 0;
    
    const range = document.createRange();
    range.setStart(textNode, offset);
    range.collapse(true);
    selection?.removeAllRanges();
    selection?.addRange(range);
}

window.editor = {
    bridgeReference: null,
    initialize: initEditor,
    applyTransaction: applyTransaction,
    getSelection: getSelection,
    node: null,
};
