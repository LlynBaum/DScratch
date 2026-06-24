using Microsoft.Playwright;

namespace DScratch.Tests.E2E.Framework;

public static class DSelectors
{
    extension(ILocator locator)
    {
        public ILocator Paragraph => locator.Locator("p[data-dnode-id]");
        
        public ILocator TextSpan => locator.Locator("span[data-dnode-id]");
    }

    extension(IPage page)
    {
        public async Task TypeAtCurrentCursorAsync(string text)
        {
            await page.Keyboard.TypeAsync(text, new() { Delay = 100 });
        }

        public async Task BackspaceAsync(bool ctrl = false)
        {
            if (ctrl)
            {
                await page.Keyboard.PressAsync("Control+Backspace");
            }
            else
            {
                await page.Keyboard.PressAsync("Backspace");
            }
        }
        
        public async Task DelAsync(bool ctrl = false)
        {
            if (ctrl)
            {
                await page.Keyboard.PressAsync("Control+Delete");
            }
            else
            {
                await page.Keyboard.PressAsync("Delete");
            }
        }
        
        public async Task SetCursorToOffsetAsync(string dataPathId, int offset)
        {
            await page.EvaluateAsync("""
                 ([id, off]) => {
                     // Find your text span element via its stable CRDT data attribute
                     const element = document.querySelector(`[data-dnode-id='${id}']`);
                     if (!element) throw new Error(`DNode with path ID '${id}' not found in the DOM.`);

                     // Focus the root editable area first so the browser accepts the selection change
                     const editableRoot = element.closest('[contenteditable]');
                     if (editableRoot) editableRoot.focus();

                     const range = document.createRange();
                     const sel = window.getSelection();

                     // Find the raw text node child inside your <span> container
                     const textNode = element.firstChild;

                     if (!textNode || textNode.nodeType !== Node.TEXT_NODE) {
                         // Handle empty paragraphs/spans where no child text node exists yet
                         range.setStart(element, 0);
                     } else {
                         // Cap the offset safely against the actual text length to prevent DOM errors
                         const safeOffset = Math.min(off, textNode.textContent.length);
                         range.setStart(textNode, safeOffset);
                     }

                     range.collapse(true); // collapse(true) means cursor mode, no highlight selection
                     sel.removeAllRanges();
                     sel.addRange(range);
                 }
                 """, 
                new object[] { dataPathId, offset }
            );
        }
    }
}