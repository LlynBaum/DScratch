using Microsoft.AspNetCore.Components;

namespace DScratch.Client.Components;

public partial class IconButton : ComponentBase
{
    [Parameter]
    [EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter]
    public EventCallback OnClick { get; set; }

    [Parameter]
    public bool IsActive { get; set; }

    [Parameter]
    public bool Disabled { get; set; }

    [Parameter]
    public string? Title { get; set; }
}
