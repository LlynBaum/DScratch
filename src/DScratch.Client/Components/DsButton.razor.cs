using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DScratch.Client.Components;

public enum DsButtonVariant
{
    Primary,
    Secondary,
    Danger,
    Ghost
}

public partial class DsButton : ComponentBase
{
    [Parameter]
    public DsButtonVariant Variant { get; set; } = DsButtonVariant.Primary;

    [Parameter]
    public string Type { get; set; } = "button";

    [Parameter]
    public bool Disabled { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    [Parameter]
    public EditorIcon? Icon { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string CssClass => $"ds-button ds-button-{Variant.ToString().ToLowerInvariant()} {Class}".Trim();
}
