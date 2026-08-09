using DScratch.Client.Services;
using DScratch.Interactions;
using DScratch.Interactions.EventHandlers;
using DScratch.Interactions.UserStates;
using Microsoft.JSInterop;

namespace DScratch.Client.BrowserInteractions;

public class BrowserEventHelper(
    DJsInvoker jsInvoker, 
    IDScratchService dScratchService,
    IUserStateService userStateService,
    IEditorDebugService editorDebugService,
    IServiceProvider serviceProvider, 
    ILogger<BrowserEventHelper> logger)
{
    [JSInvokable]
    public async Task OnKeyPressCallbackAsync(KeyPressInfo keyPressInfo)
    {
        if (keyPressInfo.Selection is null || keyPressInfo.Selection.AnchorNodeId.IsRoot || keyPressInfo.Selection.FocusNodeId.IsRoot)
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

            editorDebugService.NotifyDocumentChanged(new DebugTransactionInfo(result, keyPressInfo));
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

    [JSInvokable]
    public void OnSelectionChange(SelectionInfo? selectionInfo)
    {
        if (selectionInfo is null)
        {
            userStateService.UpdateState(null);
            editorDebugService.NotifySelectionChange(null);
            return;
        }
        
        var node = dScratchService.Document.FindNode(selectionInfo.AnchorNodeId);
        userStateService.UpdateState(node);

        editorDebugService.NotifySelectionChange(selectionInfo);
    }
}