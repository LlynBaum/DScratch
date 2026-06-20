using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Common;

public abstract class EventWithSelectionBase(IDScratchService dScratchService) : IEditorEventHandler
{
    public TransactionResult Handle(KeyPressInfo keyPressInfo)
    {
        var transaction = dScratchService.StartTransaction();
        var targetNode = transaction.FindNode(keyPressInfo.Selection.AnchorNodeId);

        DNodeInfo nodeInfo;
        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            if (targetNode is TextNode targetTextNode)
            {
                nodeInfo = HandleNoneSelection(keyPressInfo, transaction, targetTextNode);
            }
            else
            {
                HandleEmptyBlock(keyPressInfo, transaction, targetNode);
                return dScratchService.Apply(transaction);
            }
        }
        else
        {
            var nodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction);
            
            var cursorPosition = nodeSearchResult.Origin.AbsoluteOffsetIfPresent ?? 0;
            var cursorTarget = nodeSearchResult.Origin.Node ?? targetNode;
            if (cursorTarget is not null) transaction.AddCursorPosition(cursorTarget.Id, cursorPosition);

            nodeInfo = nodeSearchResult.Origin;
        }
        
        OnAfterSelection(keyPressInfo, transaction, targetNode, nodeInfo);
        
        return dScratchService.Apply(transaction);
    }

    protected abstract DNodeInfo HandleNoneSelection(
        KeyPressInfo keyPressInfo,
        ITransaction transaction,
        TextNode anchorTextNode);

    protected abstract void HandleEmptyBlock(
        KeyPressInfo keyPressInfo,
        ITransaction transaction,
        DNode? anchorNode);

    protected virtual void OnAfterSelection(
        KeyPressInfo keyPressInfo,
        ITransaction transaction,
        DNode? anchorNode,
        DNodeInfo nodeInfo) { }
}