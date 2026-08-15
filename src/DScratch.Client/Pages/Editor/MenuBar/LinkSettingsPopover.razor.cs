using DScratch.Interactions.CommandHandlers;
using DScratch.Interactions.CommandHandlers.Commands;

namespace DScratch.Client.Pages.Editor.MenuBar;

public partial class LinkSettingsPopover(IEditorCommandDispatcher dispatcher)
{
    private string? editLink;
    private bool isEditTargetBlank;
    
    private async Task UrlChangedAsync(string? url)
    {
        editLink = url;
        if (string.IsNullOrWhiteSpace(url)) return;
        await dispatcher.DispatchAsync(new UpdateLinkCommand(url, null));
    }

    private async Task OnEditTargetChangeAsync(bool isBlank)
    {
        isEditTargetBlank = isBlank;
        var target = isBlank ? "_blank" : "_self";
        await dispatcher.DispatchAsync(new UpdateLinkCommand(null, target));
    }
    
    private async Task RemoveLinkAsync()
    {
        await dispatcher.DispatchAsync(new RemoveLinkCommand());
    }
}