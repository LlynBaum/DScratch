using DScratch.Interactions.EventHandlers.Common;
using DScratch.Interactions.EventHandlers.Models;
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
        
        var endNode = SimpleDeleteBackwards();
        var endNodeId = new NodeId(endNode.Node!.Id.Client, endNode.Node.Id.Clock + endNode.Offset);
        
        transaction.DeleteRange(endNodeId, Context.AnchorNodeId);
        
        if (endNode.Node is TextNode textNode && textNode.Id != endNodeId)
        {
            var targetId = new NodeId(endNodeId.Client, endNodeId.Clock - 1);
            
            transaction.AddCursorPosition(
                nodeId: textNode.Id,
                offset: (int)(targetId.Clock - textNode.Id.Clock + 1));
        }
        else if (endNode.Node.PreviousActiveSibling() is { } previousActiveSibling)
        {
            transaction.AddCursorPosition(
                nodeId: previousActiveSibling.Id,
                offset: (int)(previousActiveSibling.LastId.Clock - previousActiveSibling.Id.Clock + 1));
        }
        else
        {
            transaction.AddCursorPosition(endNode.Node.Parent!.Id, 0);
        }
    }

    private DNodeInfo SimpleDeleteBackwards()
    {
        // TODO: I mean in theory we always get a TextNode at that point, but you never know, maybe it would be better to check that
        var walker = TreeWalker<TextNode>.StartFrom((TextNode)Context.AnchorNode!, Context.AnchorNode!.GetNearestBlock());
        return DeleteWord(walker);
    }

    private static DNodeInfo DeleteWord(TreeWalker<TextNode> walker)
    {
        var index = walker.Node!.Length - 1;
        
        while (index >= 0 && walker.Node is not null && char.IsWhiteSpace(walker.Node.TextContent[index]))
        {
            if (index == 0)
            {
                walker.MovePrevious();
                index = walker.Node?.Length ?? 0;
            }
            index--;
        }

        while (index >= 0 && walker.Node is not null && !char.IsWhiteSpace(walker.Node.TextContent[index]))
        {
            if (index == 0)
            {
                walker.MovePrevious();
                index = walker.Node?.Length ?? 0;
            }
            index--;
        }

        var prevWord = walker.Node?.PreviousSibling();
        return prevWord is not null
            ? SelectionHelper.NearestTextNode(prevWord)
            : new DNodeInfo(walker.Node?.Parent ?? walker.Parent, 0);
    }
}