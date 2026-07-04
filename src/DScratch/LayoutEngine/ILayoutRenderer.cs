using DScratch.Interactions;

namespace DScratch.LayoutEngine;

public interface ILayoutRenderer
{
    Task RenderAsync(IReadOnlyList<ElementNode> nodes, SelectionInfo? cursorPosition);
}