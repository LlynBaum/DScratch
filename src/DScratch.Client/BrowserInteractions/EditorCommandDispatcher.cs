using DScratch.Client.Services;
using DScratch.Interactions;
using DScratch.Interactions.CommandHandlers;
using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Interactions.UserStates;
using DScratch.Nodes.Marks;

namespace DScratch.Client.BrowserInteractions;

public class EditorCommandDispatcher(
    IDScratchService dScratchService, 
    IUserStateService userStateService,
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
        var selectionInfo = await jsInvoker.GetSelectionAsync();

        if (selectionInfo.Direction is SelectionDirection.None)
        {
            if (action is UpdateMarkAction.Remove)
            {
                userStateService.RemovePendingMark(mark);
            }
            else
            {
                userStateService.AddPendingMark(mark);
            }
            return;
        }
        
        if (selectionInfo.AnchorNodeId.IsRoot || selectionInfo.FocusNodeId.IsRoot)
        {
            return;
        }
        
        var transaction = dScratchService.StartTransaction();
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