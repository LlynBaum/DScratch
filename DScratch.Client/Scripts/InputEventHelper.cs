using DScratch.Client.Scripts.EventHandlers;
using Microsoft.JSInterop;

namespace DScratch.Client.Scripts;

public class InputEventHelper(IServiceProvider serviceProvider, ILogger<InputEventHelper> logger)
{
    private readonly DScratchDocument document = new DScratchDocument();
    
    [JSInvokable]
    public void OnKeyPressCallback(KeyPressInfo keyPressInfo)
    {
        var handler = serviceProvider.GetKeyedService<IEditorEventHandler>(keyPressInfo.InputType);
        if (handler is not null)
        {
            var result = handler.Handle(keyPressInfo, document);
            if (result.IsEmpty)
            {
                return;
            }

            // TODO: dispatcher result to JS
        }
        else
        {
            logger.LogWarning("No handler registered for input type: {InputType}", keyPressInfo.InputType);
        }
    }
}