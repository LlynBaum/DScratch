using DScratch.Nodes;
using Microsoft.AspNetCore.Components;

namespace DScratch.Client.Components.DNodes;

public partial class DNodeElement
{
    [Parameter]
    [EditorRequired]
    public required DNode Node { get; set; }
}