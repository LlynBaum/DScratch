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
        if (targetNode is not TextNode targetTextNode)
        {
            throw new ArgumentException($"Expected TextNode at {keyPressInfo.Selection.AnchorId}");
        }

        DNodeInfo nodeInfo;
        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            nodeInfo = HandleNoneSelection(keyPressInfo, transaction, targetTextNode);
        }
        else
        {
            var nodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction);
            
            var cursorPosition = nodeSearchResult.Origin.AbsoluteOffsetIfPresent;
            var cursorTarget = nodeSearchResult.Origin.Node ?? targetNode;
            if (cursorPosition is not null) transaction.AddCursorPosition(cursorTarget.Id, cursorPosition.Value);

            nodeInfo = nodeSearchResult.Origin;
        }
        
        OnAfterSelection(nodeInfo);
        
        return dScratchService.Apply(transaction);
    }

    protected abstract DNodeInfo HandleNoneSelection(
        KeyPressInfo keyPressInfo,
        ITransaction transaction,
        TextNode anchorTextNode);

    protected virtual void OnAfterSelection(DNodeInfo nodeInfo) { }
}