using DScratch.Interactions;
using DScratch.LayoutEngine.Pages;

namespace DScratch.LayoutEngine;

public interface ILayoutRenderer
{
    Task RenderAsync(ElementNode rootElement, SelectionInfo? cursorPosition, int pageNumber);
}