using Microsoft.AspNetCore.Components;

namespace DScratch.Client.Components;

public partial class MenubarButton : ComponentBase
{
    [Parameter]
    [EditorRequired]
    public RenderFragment ChildContent { get; set; }
    
    [Parameter]
    public EventCallback OnClick { get; set; }
}