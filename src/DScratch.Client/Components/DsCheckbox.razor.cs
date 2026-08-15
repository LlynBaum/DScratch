using Microsoft.AspNetCore.Components;

namespace DScratch.Client.Components;

public partial class DsCheckbox : ComponentBase
{
    [Parameter]
    public string? Id { get; set; }

    [Parameter]
    public string? CssClass { get; set; }

    [Parameter]
    public string? InputCssClass { get; set; }

    [Parameter]
    public bool Value { get; set; }

    [Parameter]
    public EventCallback<bool> ValueChanged { get; set; }

    [Parameter]
    public EventCallback<ChangeEventArgs> OnChange { get; set; }

    [Parameter]
    public string? Label { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool Disabled { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object?>? AdditionalAttributes { get; set; }

    private string CombinedWrapperCssClass =>
        $"ds-checkbox-wrapper {(Disabled ? "disabled" : "")} {CssClass}".Trim();

    private string CombinedInputCssClass =>
        $"ds-checkbox-input {InputCssClass} {CssClass}".Trim();

    private async Task HandleChange(ChangeEventArgs e)
    {
        var isChecked = e.Value switch
        {
            bool b => b,
            string s when bool.TryParse(s, out var b) => b,
            _ => false
        };

        Value = isChecked;
        await ValueChanged.InvokeAsync(isChecked);

        if (OnChange.HasDelegate)
        {
            await OnChange.InvokeAsync(e);
        }
    }
}
