using DScratch.LayoutEngine;
using Microsoft.AspNetCore.Components;

namespace DScratch.Client.BrowserInteractions.Rendering.Components;

public partial class DElementNode : ComponentBase
{
    [Parameter]
    [EditorRequired]
    public required ElementNode ElementNode { get; set; }
}