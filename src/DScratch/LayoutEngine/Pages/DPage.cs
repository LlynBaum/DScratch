using DScratch.Nodes;

namespace DScratch.LayoutEngine.Pages;

internal sealed class DPage
{
    public DNode LastNode { get; }
    
    public required int PageNumber { get; init; }
}