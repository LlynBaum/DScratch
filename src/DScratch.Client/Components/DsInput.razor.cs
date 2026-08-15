using Microsoft.AspNetCore.Components;

namespace DScratch.Client.Components;

public partial class DsInput
{
    [Parameter]
    public string? CssClass { get; set; }

    [Parameter] 
    public string? Value { get; set; }
    
    [Parameter] 
    public BindEvent BindEvent { get; set; } = BindEvent.Input;
    
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }
    
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object?>? AdditionalAttributes { get; set; }
    
    private string CombinedCssClass => $"ds-input {CssClass}".TrimEnd();

    private async Task HandleInput(ChangeEventArgs e)
    {
        if (BindEvent == BindEvent.Input)
        {
            await ValueChanged.InvokeAsync(e.Value?.ToString());
        }
    }

    private async Task HandleChange(ChangeEventArgs e)
    {
        if (BindEvent == BindEvent.Change)
        {
            await ValueChanged.InvokeAsync(e.Value?.ToString());
        }
    }
}

public enum BindEvent
{
    Change,
    Input
}
