using Microsoft.AspNetCore.Components;

namespace DScratch.Client.Components;

public partial class DsPopover
{
    [Parameter]
    [EditorRequired]
    public required string Id { get; set; }
    
    [Parameter]
    [EditorRequired]
    public RenderFragment ChildContent { get; set; }
    
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }
}