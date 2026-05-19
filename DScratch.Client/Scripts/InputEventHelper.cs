using DScratch.Client.Scripts.EventHandlers;
using Microsoft.JSInterop;

namespace DScratch.Client.Scripts;

public class InputEventHelper(IDScratchService dScratchService, IServiceProvider serviceProvider, ILogger<InputEventHelper> logger)
{
    private readonly DScratchDocument document = new DScratchDocument();
    
    [JSInvokable]
    public void OnKeyPressCallback(KeyPressInfo keyPressInfo)
    {
        var handler = serviceProvider.GetKeyedService<IEditorEventHandler>(keyPressInfo.InputType);
        if (handler is not null)
        {
            var transaction = handler.Handle(keyPressInfo, document);

            if (transaction is null)
            {
                return;
            }

            var result = dScratchService.Apply(transaction);
            // TODO: dispatcher result to JS
        }
        else
        {
            logger.LogWarning("No handler registered for input type: {InputType}", keyPressInfo.InputType);
        }
    }
}