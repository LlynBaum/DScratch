using DScratch.Client.BrowserInteractions.CommandHandlers.Commands;

namespace DScratch.Client.BrowserInteractions.CommandHandlers;

public class EditorCommandDispatcher(IDScratchService dScratchService, DJsInvoker jsInvoker) : IEditorCommandDispatcher
{
    public async Task ChangeBlockTypeAsync(BlockType targetBlockType)
    {
        var transaction = dScratchService.StartTransaction();
        var keyPressInfo = await jsInvoker.GetSelectionAsync();
        
        ChangeBlockTypeHandler.Execute(transaction, keyPressInfo, targetBlockType);

        var result = dScratchService.Apply(transaction);
        await jsInvoker.ApplyTransaction(result);
    }
}