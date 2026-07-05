using DScratch.LayoutEngine;
using Microsoft.AspNetCore.Components;

namespace DScratch.Client.BrowserInteractions.Rendering;

public partial class DPage : ComponentBase
{
    [Parameter]
    public ElementNode RootElement { get; set; }
}