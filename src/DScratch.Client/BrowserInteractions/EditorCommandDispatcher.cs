using DScratch.Client.Services;
using DScratch.Interactions;
using DScratch.Interactions.CommandHandlers;

namespace DScratch.Client.BrowserInteractions;

public class EditorCommandDispatcher(
    IServiceProvider serviceProvider,
    DJsInvoker jsInvoker,
    IDScratchService dScratchService,
    IEditorDebugService editorDebugService,
    ILogger<EditorCommandDispatcher> logger) : IEditorCommandDispatcher
{
    public async Task DispatchAsync<TCommand>(TCommand command) where TCommand : IEditorCommand
    {
        var selectionInfo = await jsInvoker.GetSelectionAsync();
        if (selectionInfo.AnchorNodeId.IsRoot || selectionInfo.FocusNodeId.IsRoot)
        {
            return;
        }
        
        var handler = serviceProvider.GetService<IEditorCommandHandler<TCommand>>();
        if (handler is null)
        {
            logger.LogWarning("No handler registered for command: {Command}", command.GetType().Name);
            return;
        }
        
        var result = handler.Execute(selectionInfo, command);
        await jsInvoker.ApplyTransaction(result);
        
        editorDebugService.NotifySelectionChange(selectionInfo);
        editorDebugService.NotifyDocumentChanged(new DebugTransactionInfo(result, new KeyPressInfo
        {
            InputType = "Command",
            Data = null,
            Selection = selectionInfo
        }));

        if (editorDebugService.IsDebugEnabled)
        {
            var visualizer = new TreeVisualizers.DocumentVisualizer(dScratchService.Document);
            visualizer.Print();
        }
    }
}