using DScratch.Interactions.EventHandlers.Common;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Events;

internal class DeleteContentBackwardHandler(IDScratchService dScratchService) : EventWithSelectionBase(dScratchService)
{
    public const string EventName = "deleteContentBackward";
    
    protected override void HandleEvent(KeyPressInfo keyPressInfo, ITransaction transaction)
    {
        // Already handled by DeleteSelection
        if (keyPressInfo.Selection!.Direction is not SelectionDirection.None)
        {
            if (Context.HasAnchor)
            {
                transaction.AddCursorPosition(
                    nodeId: Context.AnchorNode.Id,
                    offset: (int)(Context.AnchorNodeId.Value.Clock - Context.AnchorNode.Id.Clock + 1));
            }
            else
            {
                transaction.AddCursorPosition(nodeId: Context.GetParent().Id, offset: 0);
            }
            
            return;
        }
        
        var parent = Context.GetParent();
        if (parent.Id.IsRoot) return; // Protect against inputs where Anchor Nodes are already a Block Element
        
        if (!Context.HasAnchor)
        {
            if (parent.PreviousActiveSibling() is not { } prevParent) return;
            
            transaction.MoveRange(parent.FirstChild?.Id, null, prevParent, prevParent.LastChild?.LastId);
            transaction.Delete(parent.Id);
            
            if (SelectionHelper.NearestTextNode(prevParent) is { HasFoundNode: true } nodeInfo)
            {
                transaction.AddCursorPosition(nodeInfo.Node.Id, nodeInfo.Offset);
            }
            else
            {
                transaction.AddCursorPosition(prevParent.Id, 0);
            }
            
            return;
        }
        
        transaction.Delete(Context.AnchorNodeId.Value);

        if (Context.AnchorNode is TextNode textNode && textNode.Id != Context.AnchorNodeId)
        {
            var targetId = new NodeId(Context.AnchorNodeId.Value.Client, Context.AnchorNodeId.Value.Clock - 1);
            
            transaction.AddCursorPosition(
                nodeId: textNode.Id,
                offset: (int)(targetId.Clock - textNode.Id.Clock + 1));
        }
        else if (Context.AnchorNode.PreviousActiveSibling() is { } previousActiveSibling)
        {
            transaction.AddCursorPosition(
                nodeId: previousActiveSibling.Id,
                offset: (int)(previousActiveSibling.LastId.Clock - previousActiveSibling.Id.Clock + 1));
        }
        else
        {
            transaction.AddCursorPosition(parent.Id, 0);
        }
    }
}