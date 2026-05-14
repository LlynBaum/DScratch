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

    editor?.addEventListener("keydown", async e => {
        const selection = getSelection();
        const path = getPath(selection?.anchorNode as HTMLElement);
        const endPath = getPath(selection?.focusNode as HTMLElement);
        
        const payload = {
            Key: {
                Value: e.key,
                Alt: e.altKey,
                Ctrl: e.ctrlKey,
                Shift: e.shiftKey
            },
            Path: path,
            Selection: {
                Offset: selection?.anchorOffset,
                Direction: selection?.direction,
                End: endPath,
                EndOffset: selection?.focusOffset
            }
        }
        
        // @ts-ignore ts does not know what DotNet will be here when WASM has loaded
        await DotNet.invokeMethodAsync("DScratch.Client", "OnKeyPressCallback", payload);
    });

    function getPath(element: HTMLElement): string[] {
        const result: string[] = [];
        let current = element.parentElement; // Event is fired on text node within p element
        
        while (current && !current.classList.contains("paper")) {
            result.push(current.getAttribute("data-path-id")!);
            current = current.parentElement;
        }

        return result;
    }
}

window.addEventListener("DOMContentLoaded", initEditor);
