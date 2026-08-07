using Microsoft.AspNetCore.Components;

namespace DScratch.Client.Components;

public partial class DsIconButton : ComponentBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public EditorIcon? Icon { get; set; }

    [Parameter]
    public EventCallback OnClick { get; set; }

    [Parameter]
    public bool IsActive { get; set; }

    [Parameter]
    public bool Disabled { get; set; }

    [Parameter]
    public string? Title { get; set; }
}
