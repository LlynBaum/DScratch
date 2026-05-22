using DScratch.Client.Scripts.EventHandlers;
using Microsoft.JSInterop;

namespace DScratch.Client.Scripts;

public class InputEventHelper(IJSRuntime jsRuntime, IServiceProvider serviceProvider, ILogger<InputEventHelper> logger)
{
    private const string ApplyTransactionJs = "applyTransaction";
    
    private readonly DScratchDocument document = new DScratchDocument();
    
    [JSInvokable]
    public async Task OnKeyPressCallbackAsync(KeyPressInfo keyPressInfo)
    {
        var handler = serviceProvider.GetKeyedService<IEditorEventHandler>(keyPressInfo.InputType);
        if (handler is not null)
        {
            var result = handler.Handle(keyPressInfo, document);
            if (result.IsEmpty)
            {
                return;
            }

            await jsRuntime.InvokeVoidAsync(ApplyTransactionJs, result.Diffs);
        }
        else
        {
            logger.LogWarning("No handler registered for input type: {InputType}", keyPressInfo.InputType);
        }
    }
}