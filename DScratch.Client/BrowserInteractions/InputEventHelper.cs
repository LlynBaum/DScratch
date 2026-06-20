using DScratch.Interactions;
using DScratch.Interactions.EventHandlers;
using Microsoft.JSInterop;

namespace DScratch.Client.BrowserInteractions;

public class InputEventHelper(DJsInvoker jsInvoker, IServiceProvider serviceProvider, ILogger<InputEventHelper> logger)
{
    [JSInvokable]
    public async Task OnKeyPressCallbackAsync(KeyPressInfo keyPressInfo)
    {
        if (keyPressInfo.Selection.AnchorNodeId.IsRoot || keyPressInfo.Selection.FocusNodeId.IsRoot)
        {
            return;
        }
        
        var handler = serviceProvider.GetKeyedService<IEditorEventHandler>(keyPressInfo.InputType);
        if (handler is not null)
        {
            var result = handler.Handle(keyPressInfo);
            if (result.IsEmpty)
            {
                return;
            }

            await jsInvoker.ApplyTransaction(result);
        }
        else
        {
            logger.LogWarning("No handler registered for input type: {InputType}", keyPressInfo.InputType);
        }
    }
}