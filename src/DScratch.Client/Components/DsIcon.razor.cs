using Microsoft.AspNetCore.Components;

namespace DScratch.Client.Components;

public partial class DsIcon : ComponentBase
{
    [Parameter, EditorRequired]
    public EditorIcon Icon { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string MaskStyle
    {
        get
        {
            var fileName = Icon.GetFileName();
            var iconUrl = $"icons/{fileName}";
            return $"mask-image: url('{iconUrl}'); -webkit-mask-image: url('{iconUrl}');";
        }
    }
}
