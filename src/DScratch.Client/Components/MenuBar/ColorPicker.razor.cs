using Microsoft.AspNetCore.Components;

namespace DScratch.Client.Components.MenuBar;

public partial class ColorPicker : ComponentBase
{
    [Parameter]
    public string? Value { get; set; }

    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    [Parameter]
    public EventCallback OnClear { get; set; }

    private async Task OnColorInputChanged(ChangeEventArgs e)
    {
        Value = e.Value?.ToString();
        await ValueChanged.InvokeAsync(Value);
    }

    private async Task OnClearColor()
    {
        await OnClear.InvokeAsync();
    }
}
