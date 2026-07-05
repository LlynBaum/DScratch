using DScratch.Client.Services;
using DScratch.Interactions;
using DScratch.Interactions.CommandHandlers;
using DScratch.Interactions.CommandHandlers.Commands;

namespace DScratch.Client.BrowserInteractions;

public class EditorCommandDispatcher(
    IDScratchService dScratchService, 
    DJsInvoker jsInvoker,
    EditorDebugService editorDebugService) : IEditorCommandDispatcher
{
    public async Task ChangeBlockTypeAsync(BlockNodeType targetBlockNodeType)
    {
        var transaction = dScratchService.StartTransaction();
        var selectionInfo = await jsInvoker.GetSelectionAsync();
        
        if (selectionInfo.AnchorNodeId.IsRoot || selectionInfo.FocusNodeId.IsRoot)
        {
            return;
        }
        
        ChangeBlockTypeHandler.Execute(transaction, selectionInfo, targetBlockNodeType);

        await dScratchService.ApplyAsync(transaction);
        
        editorDebugService.NotifyDocumentChanged(new EditorDebugService.TransactionInfo(result, new KeyPressInfo
        {
            InputType = "ChangeBlockType",
            Data = null,
            Selection = selectionInfo
        }));
    }
}