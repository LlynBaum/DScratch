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
                    node: Context.AnchorNode,
                    offset: (int)(Context.AnchorNodeId.Value.Clock - Context.AnchorNode.Id.Clock + 1));
            }
            else
            {
                transaction.AddCursorPosition(node: Context.GetParent(), offset: 0);
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
                transaction.AddCursorPosition(nodeInfo.Node, nodeInfo.Offset);
            }
            else
            {
                transaction.AddCursorPosition(parent, 0);
            }
            
            return;
        }
        
        var endNodeId = GetDeleteEnd();
        transaction.DeleteRange(Context.RightAnchorNodeId, endNodeId);
        
        if (Context.HasAnchor)
        {
            transaction.AddCursorPosition(
                node: Context.AnchorNode,
                offset: (int)(Context.AnchorNodeId.Value.Clock - Context.AnchorNode.Id.Clock + 1));
        }
        else
        {
            transaction.AddCursorPosition(Context.RightAnchorNode.Parent!, 0);
        }
    }

    private NodeId GetDeleteEnd()
    {
        // TODO: I mean in theory we always get a TextNode at that point, but you never know, maybe it would be better to check that
        var walker = TreeWalker<TextNode>.StartFrom((TextNode)Context.RightAnchorNode!, Context.RightAnchorNode!.GetNearestBlock());
        return DeleteWord(walker);
    }

    private static NodeId DeleteWord(TreeWalker<TextNode> walker)
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

        if (walker.Node is null)
        {
            return prevNode!.Parent!.LastId;
        }
        
        if (walker.Node.Length == characterOffset)
        {
            prevNode = walker.Node;
            var nextNode = walker.NextNode();
            return nextNode?.Id ?? prevNode.Parent!.LastId;
        }

        return new NodeId(walker.Node.Id.Client, walker.Node.Id.Clock + characterOffset);
    }
}