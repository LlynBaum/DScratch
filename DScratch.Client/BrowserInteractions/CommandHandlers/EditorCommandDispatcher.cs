using DScratch.Client.BrowserInteractions.CommandHandlers.Commands;

namespace DScratch.Client.BrowserInteractions.CommandHandlers;

public class EditorCommandDispatcher(IDScratchService dScratchService, DJsInvoker jsInvoker) : IEditorCommandDispatcher
{
    public async Task ChangeBlockTypeAsync(BlockType targetBlockType)
    {
        var transaction = dScratchService.StartTransaction();
        var selectionInfo = await jsInvoker.GetSelectionAsync();
        
        if (selectionInfo.AnchorNodeId.IsRoot || selectionInfo.FocusNodeId.IsRoot)
        {
            return;
        }
        
        ChangeBlockTypeHandler.Execute(transaction, selectionInfo, targetBlockType);

        var result = dScratchService.Apply(transaction);
        await jsInvoker.ApplyTransaction(result);
    }
}