using DScratch.Interactions.EventHandlers.Common;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Events;

internal class DeleteWordForwardHandler(IDScratchService dScratchService) : EventWithSelectionBase(dScratchService)
{
    public const string EventName = "deleteWordForward";

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
        
        var (endNode, endNodeId) = SimpleDeleteForwards();
        
        transaction.DeleteRange(Context.RightAnchorNodeId, endNodeId);
        
        if (endNode.Id != endNodeId)
        {
            var targetId = new NodeId(endNodeId.Client, endNodeId.Clock - 1);
            
            transaction.AddCursorPosition(
                nodeId: endNode.Id,
                offset: (int)(targetId.Clock - endNode.Id.Clock + 1));
        }
        else if (endNode.PreviousActiveSibling() is { } previousActiveSibling)
        {
            transaction.AddCursorPosition(
                nodeId: previousActiveSibling.Id,
                offset: (int)(previousActiveSibling.LastId.Clock - previousActiveSibling.Id.Clock + 1));
        }
        else
        {
            transaction.AddCursorPosition(endNode.Parent!.Id, 0);
        }
    }

    private (TextNode Node, NodeId nodeId) SimpleDeleteForwards()
    {
        // TODO: I mean in theory we always get a TextNode at that point, but you never know, maybe it would be better to check that
        var walker = TreeWalker<TextNode>.StartFrom((TextNode)Context.RightAnchorNode!, Context.RightAnchorNode!.GetNearestBlock());
        return DeleteWord(walker);
    }

    private static (TextNode Node, NodeId nodeId) DeleteWord(TreeWalker<TextNode> walker)
    {
        TextNode? prevNode = null;
        var characterOffset = 0;
        
        while (walker.Node is not null && characterOffset < walker.Node.Length && char.IsWhiteSpace(walker.Node.TextContent[characterOffset]))
        {
            if (characterOffset == walker.Node.Length - 1)
            {
                prevNode = walker.Node;
                walker.NextNode();
                characterOffset = 0;
            }
            characterOffset++;
        }

        while (walker.Node is not null && characterOffset < walker.Node.Length && !char.IsWhiteSpace(walker.Node.TextContent[characterOffset]))
        {
            if (characterOffset == walker.Node.Length - 1)
            {
                prevNode = walker.Node;
                walker.NextNode();
                characterOffset = 0;
            }
            characterOffset++;
        }

        return walker.Node is not null
            ? (walker.Node, new NodeId(walker.Node.Id.Client, walker.Node.Id.Clock + characterOffset + 1))
            : (prevNode!, prevNode!.LastId);
    }
}