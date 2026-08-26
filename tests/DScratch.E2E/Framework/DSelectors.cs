using DScratch.Interactions;
using Microsoft.Playwright;

namespace DScratch.E2E.Framework;

public static class DSelectors
{
    extension(ILocator locator)
    {
        public ILocator EditorPage => locator.Locator(".page[data-page-index]");
        
        public ILocator AllChildren => locator.Locator("> *");
        
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
        
        public ILocator Link => locator.Locator("a[data-dnode-id]");
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

        public async Task ArrowUp()
        {
            await page.Keyboard.PressAsync("ArrowUp");
        }
        
        public async Task SetCursorAsync(string dataPathId, int offset)
        {
            await page.EvaluateAsync("""
                 ([id, off]) => {
                     window.__dscratch_test__?.setSelection({
                         direction: 'none',
                         anchorId: id,
                         anchorOffset: off,
                         focusId: id,
                         focusOffset: off
                     });
                 }
                 """, 
                new object[] { dataPathId, offset }
            );
        }
        
        public async Task SetSelectionAsync(SelectionInfo selectionInfo)
        {
            var payload = new object?[] 
            { 
                selectionInfo.Direction.ToString().ToLowerInvariant(),
                selectionInfo.AnchorId, 
                selectionInfo.AnchorOffset, 
                selectionInfo.FocusId, 
                selectionInfo.FocusOffset 
            };

            await page.EvaluateAsync("""
                (args) => {
                    const [direction, anchorId, anchorOffset, focusId, focusOffset] = args;
                    window.__dscratch_test__?.setSelection({
                        direction: direction,
                        anchorId: anchorId,
                        anchorOffset: anchorOffset,
                        focusId: focusId,
                        focusOffset: focusOffset
                    });
                }
                """, payload);
        }
    }
}