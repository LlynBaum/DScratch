using DScratch.Client.BrowserInteractions;
using DScratch.Interactions;
using DScratch.Interactions.CommandHandlers;
using DScratch.Interactions.CommandHandlers.Commands;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DScratch.Client.Pages.Editor.MenuBar;

public partial class AddLinkPopover(IEditorCommandDispatcher dispatcher, DJsInvoker jsInvoker)
{
    private string? displayText;
    private string linkUrl = string.Empty;
    private bool hasDisplayText;
    private bool isTargetBlank;
    
    private async Task SubmitLinkAsync()
    {
        if (!IsInvalidLinkInput())
        {
            var target = isTargetBlank ? "_blank" : "_self";
            await dispatcher.DispatchAsync(new AddLinkCommand(linkUrl, target, displayText));
        }
    }
    
    private async Task HandleLinkKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await SubmitLinkAsync();
        }
    }

    private bool IsInvalidLinkInput()
    {
        if (string.IsNullOrWhiteSpace(linkUrl)) return true;
        
        if (hasDisplayText)
        {
            return string.IsNullOrWhiteSpace(displayText);
        }

        return false;
    }
    
    private async Task OnPopoverToggleAsync()
    {
        linkUrl = string.Empty;
        displayText = null;
        isTargetBlank = false;
        
        var selection = await jsInvoker.GetEditorSelectionAsync();
        hasDisplayText = selection?.Direction is SelectionDirection.None;
    }
}