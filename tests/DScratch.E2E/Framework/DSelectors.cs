using DScratch.Interactions;
using Microsoft.Playwright;

namespace DScratch.E2E.Framework;

public static class DSelectors
{
    extension(ILocator locator)
    {
        public ILocator Paragraph => locator.Locator("p[data-dnode-id]");
        
        public ILocator Heading1 => locator.Locator("h1[data-dnode-id]");
        public ILocator Heading2 => locator.Locator("h2[data-dnode-id]");
        public ILocator Heading3 => locator.Locator("h3[data-dnode-id]");
        public ILocator Heading4 => locator.Locator("h4[data-dnode-id]");
        public ILocator Heading5 => locator.Locator("h5[data-dnode-id]");
        public ILocator Heading6 => locator.Locator("h6[data-dnode-id]");

        public ILocator Heading(HeadingLevel level)
        {
            return locator.Locator($"h{(int)level}[data-dnode-id]");
        }

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

        public async Task EnterAsync()
        {
            await page.Keyboard.PressAsync("Enter");
        }
        
        public async Task SetCursorAsync(string dataPathId, int offset)
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
        
        public async Task SetSelectionAsync(SelectionInfo selectionInfo)
        {
            var payload = new object[] 
            { 
                selectionInfo.AnchorId, 
                selectionInfo.AnchorOffset, 
                selectionInfo.FocusId, 
                selectionInfo.FocusOffset 
            };

            await page.EvaluateAsync("""
                (args) => {
                    const [anchorId, anchorOffset, focusId, focusOffset] = args;

                    // 1. Locate both elements via your stable CRDT data attributes
                    const anchorElement = document.querySelector(`[data-dnode-id='${anchorId}']`);
                    const focusElement = document.querySelector(`[data-dnode-id='${focusId}']`);

                    if (!anchorElement) throw new Error(`Anchor node with ID '${anchorId}' not found in DOM.`);
                    if (!focusElement) throw new Error(`Focus node with ID '${focusId}' not found in DOM.`);

                    // 2. Safely claim browser focus inside the contenteditable root boundary
                    const editableRoot = anchorElement.closest('[contenteditable]');
                    if (editableRoot) editableRoot.focus();

                    // Helper to resolve whether to target a text node or a structural empty block container
                    const resolveTarget = (element, offset) => {
                        const textNode = element.firstChild;
                        if (!textNode || textNode.nodeType !== Node.TEXT_NODE) {
                            return { node: element, offset: 0 };
                        }
                        const safeOffset = Math.min(offset, textNode.textContent.length);
                        return { node: textNode, offset: safeOffset };
                    };

                    const anchorTarget = resolveTarget(anchorElement, anchorOffset);
                    const focusTarget = resolveTarget(focusElement, focusOffset);

                    // 3. Apply the selection directly to the window view layer
                    const sel = window.getSelection();
                    sel.removeAllRanges();
                    
                    // setBaseAndExtent natively establishes forward vs backward directions!
                    sel.setBaseAndExtent(
                        anchorTarget.node, 
                        anchorTarget.offset, 
                        focusTarget.node, 
                        focusTarget.offset
                    );
                }
                """, payload);
        }
    }
}