using DScratch.Interactions.EventHandlers.Common;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Events;

internal class DeleteWordBackwardHandler(IDScratchService dScratchService) : EventWithSelectionBase(dScratchService)
{
    public const string EventName = "deleteWordBackward";

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
        
        var (startNode, startNodeId) = SimpleDeleteBackwards();
        
        transaction.DeleteRange(startNodeId, Context.RightAnchorNodeId);
        
        if (startNode.Id != startNodeId)
        {
            var targetId = new NodeId(startNodeId.Client, startNodeId.Clock - 1);
            
            transaction.AddCursorPosition(
                nodeId: startNode.Id,
                offset: (int)(targetId.Clock - startNode.Id.Clock + 1));
        }
        else if (startNode.PreviousActiveSibling() is { } previousActiveSibling)
        {
            transaction.AddCursorPosition(
                nodeId: previousActiveSibling.Id,
                offset: (int)(previousActiveSibling.LastId.Clock - previousActiveSibling.Id.Clock + 1));
        }
        else
        {
            transaction.AddCursorPosition(startNode.Parent!.Id, 0);
        }
    }

    private (TextNode Node, NodeId nodeId) SimpleDeleteBackwards()
    {
        // TODO: I mean in theory we always get a TextNode at that point, but you never know, maybe it would be better to check that
        var walker = TreeWalker<TextNode>.StartFrom((TextNode)Context.AnchorNode!, Context.AnchorNode!.GetNearestBlock());
        return DeleteWord(walker);
    }

    private static (TextNode Node, NodeId nodeId) DeleteWord(TreeWalker<TextNode> walker)
    {
        TextNode? prevNode = null;
        var characterOffset = walker.Node!.Length - 1;
        
        while (characterOffset >= 0 && walker.Node is not null && char.IsWhiteSpace(walker.Node.TextContent[characterOffset]))
        {
            if (characterOffset == 0)
            {
                prevNode = walker.Node;
                walker.MovePrevious();
                characterOffset = walker.Node?.Length ?? 0;
            }
            characterOffset--;
        }

        while (characterOffset >= 0 && walker.Node is not null && !char.IsWhiteSpace(walker.Node.TextContent[characterOffset]))
        {
            if (characterOffset == 0)
            {
                prevNode = walker.Node;
                walker.MovePrevious();
                characterOffset = walker.Node?.Length ?? 0;
            }
            characterOffset--;
        }

        return walker.Node is not null
            ? (walker.Node, new NodeId(walker.Node.Id.Client, walker.Node.Id.Clock + characterOffset + 1))
            : (prevNode!, prevNode!.Id);
    }
}