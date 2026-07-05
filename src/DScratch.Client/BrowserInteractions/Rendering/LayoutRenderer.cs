using DScratch.Interactions;
using DScratch.LayoutEngine;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DScratch.Client.BrowserInteractions.Rendering;

public class LayoutRenderer(IServiceProvider serviceProvider, ILoggerFactory loggerFactory) : ILayoutRenderer
{
    public async Task RenderAsync(ElementNode rootElement, SelectionInfo? cursorPosition)
    {
        await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);
        
        var parameterDictionary = new Dictionary<string, object?>
        {
            { nameof(DPage.RootElement), rootElement }
        };
        
        var parameters = ParameterView.FromDictionary(parameterDictionary);
        var output = await htmlRenderer.RenderComponentAsync<DPage>(parameters);
        var htmlString = output.ToHtmlString();
        
        // TODO: send via JsInvoker to JS and swap DOM
    }
}