using DScratch.Client.BrowserInteractions.Rendering.Components;
using DScratch.Interactions;
using DScratch.LayoutEngine;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DScratch.Client.BrowserInteractions.Rendering;

public class LayoutRenderer(DJsInvoker jsInvoker, IServiceProvider serviceProvider, ILoggerFactory loggerFactory) : ILayoutRenderer
{
    public async Task RenderAsync(ElementNode rootElement, SelectionInfo? cursorPosition, int pageNumber)
    {
        await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);
        
        var parameterDictionary = new Dictionary<string, object?>
        {
            { nameof(DPage.RootElement), rootElement },
            { nameof(DPage.PageNumber), pageNumber }
        };
        
        var parameters = ParameterView.FromDictionary(parameterDictionary);
        var output = await htmlRenderer.RenderComponentAsync<DPage>(parameters);
        var htmlString = output.ToHtmlString();

        var renderedContent = new RenderedPage
        {
            HtmlString = htmlString,
            PageNumber = pageNumber
        };
        
        await jsInvoker.RenderPageAsync(renderedContent);
    }
}