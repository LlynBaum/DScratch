using DScratch.Interactions;

namespace DScratch.LayoutEngine;

public interface ILayoutRenderer
{
    Task RenderAsync(ElementNode rootElement, SelectionInfo? cursorPosition);
}