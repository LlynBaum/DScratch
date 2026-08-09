using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DScratch.Client.Components;

public partial class DsInput : InputBase<string>
{
    private string CombinedCssClass => $"ds-input {CssClass}".Trim();

    protected override bool TryParseValueFromString(string? value, [NotNullWhen(true)] out string? result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        result = value ?? string.Empty;
        validationErrorMessage = null;
        return true;
    }

    private void OnChange(ChangeEventArgs e)
    {
        CurrentValueAsString = e.Value?.ToString();
    }

    private void OnInput(ChangeEventArgs e)
    {
        CurrentValueAsString = e.Value?.ToString();
    }
}
