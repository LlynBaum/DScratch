using DScratch.Client.Components.Icons;
using Microsoft.AspNetCore.Components;

namespace DScratch.Client.Components;

public partial class IconButton : ComponentBase
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

    private string GetIconSvg(EditorIcon icon)
    {
        return IconProvider.GetSvg(icon);
    }
}
