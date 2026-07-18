using DScratch.Client.BrowserInteractions.Metadata;
using DScratch.Client.Services;
using DScratch.Interactions;
using DScratch.Interactions.CommandHandlers;
using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Nodes.Marks;
using DScratch.Nodes.NodeTypes;

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

        var result = dScratchService.Apply(transaction);
        await jsInvoker.ApplyTransaction(result);
        
        editorDebugService.NotifyDocumentChanged(new EditorDebugService.TransactionInfo(result, new KeyPressInfo
        {
            InputType = "ChangeBlockType",
            Data = null,
            Selection = selectionInfo
        }));
    }

    public async Task UpdateMarkAsync(Mark mark, UpdateMarkAction action)
    {
        var transaction = dScratchService.StartTransaction();
        var selectionInfo = await jsInvoker.GetSelectionAsync();

        if (selectionInfo.AnchorNodeId.IsRoot || selectionInfo.FocusNodeId.IsRoot)
        {
            return;
        }

        if (selectionInfo.Direction is SelectionDirection.None)
        {
            var node = transaction.FindNode(selectionInfo.AnchorNodeId);
            var metadataAction = action is UpdateMarkAction.Remove ? "remove" : "add";
            var data = node is IBlockElement
                ? new MetadataEntry(new BlockMetadata(node.Id, mark))
                : new MetadataEntry(new PositionMetadata(mark));
            
            await jsInvoker.UpdateMetadataAsync(data, metadataAction);
            return;
        }

        UpdateMarkHandler.Execute(transaction, selectionInfo, mark, action);

        var result = dScratchService.Apply(transaction);
        await jsInvoker.ApplyTransaction(result);

        editorDebugService.NotifyDocumentChanged(new EditorDebugService.TransactionInfo(result, new KeyPressInfo
        {
            InputType = "UpdateMark",
            Data = null,
            Selection = selectionInfo
        }));
    }
}