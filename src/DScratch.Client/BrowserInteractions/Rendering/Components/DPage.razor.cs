using DScratch.LayoutEngine;
using Microsoft.AspNetCore.Components;

namespace DScratch.Client.BrowserInteractions.Rendering.Components;

public partial class DPage : ComponentBase
{
    [Parameter]
    [EditorRequired]
    public required ElementNode RootElement { get; set; }
    
    [Parameter]
    public int PageNumber { get; set; }
}