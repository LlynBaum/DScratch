using DScratch.Interactions.EventHandlers.Common;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Events;

internal class DeleteContentForwardHandler(IDScratchService dScratchService) : EventWithSelectionBase(dScratchService)
{
    public const string EventName = "deleteContentForward";

    protected override void HandleEvent(KeyPressInfo keyPressInfo, ITransaction transaction)
    {
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

        if (!Context.HasRightAnchor)
        {
            if (parent.NextActiveSibling() is not { } nextParent) return;
            
            transaction.MoveRange(nextParent.FirstChild?.Id, null, parent, parent.LastChild?.LastId);
            transaction.Delete(nextParent.Id);
            
            if (SelectionHelper.NearestTextNode(parent) is { HasFoundNode: true } nodeInfo)
            {
                transaction.AddCursorPosition(nodeInfo.Node.Id, nodeInfo.Offset);
            }
            else
            {
                transaction.AddCursorPosition(parent.Id, 0);
            }
            
            return;
        }
        
        transaction.Delete(Context.RightAnchorNodeId.Value);

        if (Context.HasAnchor)
        {
            transaction.AddCursorPosition(
                nodeId: Context.AnchorNode.Id,
                offset: (int)(Context.AnchorNodeId.Value.Clock - Context.AnchorNode.Id.Clock + 1));
        }
        else
        {
            transaction.AddCursorPosition(parent.Id, 0);
        }
    }
}