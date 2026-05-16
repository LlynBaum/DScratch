let bridgeReference: any = null;

export {};
declare global {
    interface Window {
        editor: any;
    }
}

function initEditor() {
    const editor = document.getElementById("editor");
    editor?.addEventListener("click", e => {
        const element = e.target as HTMLElement;
        if(element.hasAttribute("contenteditable")) {
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
    });

    editor?.addEventListener("beforeinput", async event => {
        const handledTypes = [
            "insertText",
            "insertParagraph",
            "deleteContentBackward",
            "deleteContentForward"
        ];

        if (!handledTypes.includes(event.inputType)) {
            return; // Let the browser handle unsupported inputs natively for now
        }

        // Stop the browser from mutating the DOM natively!
        event.preventDefault();

        event.preventDefault();
        const selection = getSelection();
        const path = getPath(selection?.anchorNode as HTMLElement);
        const endPath = getPath(selection?.focusNode as HTMLElement);
        
        const payload = {
            InputType: event.inputType,
            Data: event.data,
            Path: path,
            Selection: {
                Offset: selection?.anchorOffset,
                Direction: selection?.direction,
                End: endPath,
                EndOffset: selection?.focusOffset
            }
        }
        
        await bridgeReference.invokeMethodAsync("OnKeyPressCallback", payload);
    });

    function getPath(element: HTMLElement): string[] {
        const result: string[] = [];
        let current = element.parentElement; // Event is fired on text node within p element
        
        while (current && !current.hasAttribute("contenteditable")) {
            result.push(current.getAttribute("data-path-id")!);
            current = current.parentElement;
        }

        return result;
    }
}

window.editor = {
    initialize: (dotNetRef: any) => {
        bridgeReference = dotNetRef;
        initEditor();
    }
};
