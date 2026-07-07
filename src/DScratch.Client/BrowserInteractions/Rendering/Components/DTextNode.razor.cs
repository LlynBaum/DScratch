using DScratch.LayoutEngine;
using Microsoft.AspNetCore.Components;

namespace DScratch.Client.BrowserInteractions.Rendering.Components;

public partial class DTextNode : ComponentBase
{
    [Parameter]
    [EditorRequired]
    public required ElementNode TextElement { get; set; }
}