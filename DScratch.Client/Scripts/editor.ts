import { applyTransaction } from "./transaction";
import { handleInput } from "./inputs";

let bridgeReference: any = null;

declare global {
    interface Window {
        editor: any;
    }
}

function initEditor() {
    const editor = document.getElementById("editor");
    editor?.addEventListener("click", setCursorToEnd);
    editor?.addEventListener("beforeinput", async event => await handleInput(event, bridgeReference));
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
    initialize: (dotNetRef: any) => {
        bridgeReference = dotNetRef;
        initEditor();
    },
    applyTransaction: applyTransaction
};
