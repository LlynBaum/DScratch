using DScratch.Client.BrowserInteractions.EventHandlers.Common;
using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers.Events;

public class DeleteContentBackwardHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "deleteContentBackward";
    
    public TransactionResult Handle(KeyPressInfo keyPressInfo)
    {
        var transaction = dScratchService.StartTransaction();

        var targetNode = transaction.FindNode(keyPressInfo.Selection.AnchorNodeId);
        if (targetNode is not TextNode targetTextNode)
        {
            throw new ArgumentException($"Expected TextNode at {keyPressInfo.Selection.AnchorId}");
        }
        
        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            var deletedNodeInfo = SimpleDeleteBackwards(keyPressInfo, transaction, targetTextNode);

            if (!deletedNodeInfo.HasFoundNode && targetNode.GetNearestBlock() is { Origin: not null } parent)
            {
                if (parent.LastChild is TextNode textNode) // TODO: take inline element into account
                {
                    transaction.AddCursorPosition(textNode.Id, 0); 
                }
                
                transaction.MoveRange(parent.FirstChild, null, parent.Origin, parent.Origin.LastChild);
                transaction.Delete(parent);
            }
            else if (deletedNodeInfo.HasFoundNode)
            {
                transaction.AddCursorPosition(deletedNodeInfo.Node.Origin?.Id ?? deletedNodeInfo.Node.Id, deletedNodeInfo.Offset);
            }
        }
        else
        {
            var nodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction);
            
            var cursorPosition = nodeSearchResult.Origin.AbsoluteOffsetIfPresent;
            var cursorTarget = nodeSearchResult.Origin.Node ?? targetNode;
            if (cursorPosition is not null) transaction.AddCursorPosition(cursorTarget.Id, cursorPosition.Value);
        }
        
        return dScratchService.Apply(transaction);
    }

    private static NodeInfo<TextNode> SimpleDeleteBackwards(KeyPressInfo keyPressInfo, ITransaction transaction, TextNode targetTextNode)
    {
        if (keyPressInfo.Selection.AnchorOffset is 0)
        {
            return new NodeInfo<TextNode>(null, 0);
        }
        
        transaction.SplitText(targetTextNode, keyPressInfo.Selection.AnchorOffset);
        var nodeToDelete = transaction.SplitText(targetTextNode, 1) ?? targetTextNode;
        transaction.Delete(nodeToDelete);
        
        return new NodeInfo<TextNode>(nodeToDelete, keyPressInfo.Selection.AnchorOffset - 1);
    }
}