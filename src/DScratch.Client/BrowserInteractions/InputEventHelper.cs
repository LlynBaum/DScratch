using DScratch.Client.Services;
using DScratch.Interactions;
using DScratch.Interactions.EventHandlers;
using Microsoft.JSInterop;

namespace DScratch.Client.BrowserInteractions;

public class InputEventHelper(
    IDScratchService dScratchService,
    EditorDebugService editorDebugService,
    IServiceProvider serviceProvider, 
    ILogger<InputEventHelper> logger)
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
            var transaction = dScratchService.StartTransaction();
            handler.Handle(keyPressInfo, transaction);
            await dScratchService.ApplyAsync(transaction);

            editorDebugService.NotifyKeyPress(keyPressInfo);

            if (editorDebugService.IsDebugEnabled)
            {
                var visualizer = new TreeVisualizers.DocumentVisualizer(dScratchService.Document);
                visualizer.Print();
            }
        }
        else
        {
            logger.LogWarning("No handler registered for input type: {InputType}", keyPressInfo.InputType);
        }
    }
}