using Microsoft.AspNetCore.Components;

namespace DScratch.Client.Components;

public partial class DsDialog : ComponentBase
{
    [Parameter]
    public bool IsOpen { get; set; }

    [Parameter]
    public EventCallback<bool> IsOpenChanged { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public bool ShowCloseButton { get; set; } = true;

    [Parameter]
    public bool CloseOnBackdropClick { get; set; } = true;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public RenderFragment? FooterContent { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    public async Task OpenAsync()
    {
        IsOpen = true;
        await IsOpenChanged.InvokeAsync(true);
    }

    public async Task CloseAsync()
    {
        IsOpen = false;
        await IsOpenChanged.InvokeAsync(false);
        await OnClose.InvokeAsync();
    }

    private async Task HandleBackdropClick()
    {
        if (CloseOnBackdropClick)
        {
            await CloseAsync();
        }
    }
}
