import {applyTransaction, TransactionResult} from "./transaction";
import { handleInput } from "./inputs";

let bridgeReference: any = null;

interface Editor {
    initialize: (dotNetRef: any) => void;
    applyTransaction: (transaction: TransactionResult) => void;
    node: HTMLElement | null;
}

declare global {
    interface Window {
        editor: Editor;
    }
}

function initEditor(dotNetRef: any) {
    const editor = document.getElementById("editor");
    
    if (!editor) {
        console.error("There is no editor node.");
        return;
    }
    
    editor?.addEventListener("click", setCursorToEnd);
    editor?.addEventListener("beforeinput", async event => await handleInput(event, bridgeReference));
    
    const rootNode = editor.querySelector<HTMLElement>("[data-dnode-id='Root']");
    rootNode?.setAttribute("contenteditable", '');
    
    window.editor.node = editor;
    bridgeReference = dotNetRef;
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
    initialize: initEditor,
    applyTransaction: applyTransaction,
    node: null,
};
