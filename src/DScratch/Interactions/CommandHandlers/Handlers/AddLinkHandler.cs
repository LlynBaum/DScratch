using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.CommandHandlers.Handlers;

public class AddLinkHandler(IDScratchService dScratchService) : CommandBase<AddLinkCommand>(dScratchService)
{
    protected override void Handle(ITransaction transaction, SelectionInfo selectionInfo, AddLinkCommand command)
    {
        if (selectionInfo.Direction is SelectionDirection.None)
        {
            InsertWithDisplayText(transaction, selectionInfo, command);
            return;
        }
        
        var (originId, rightOriginId) = selectionInfo.GetConvertedNodeIds();
        var origin = transaction.Document.FindNode(originId);
        var rightOrigin = transaction.Document.FindNode(rightOriginId);
        
        if (origin is null || rightOrigin is null)
        {
            throw new ArgumentException($"Node with given id not found: {selectionInfo.AnchorId} or {selectionInfo.FocusId}");
        }

        if (origin.GetNearestBlock().Id == rightOrigin.GetNearestBlock().Id)
        {
            HandleSimpleSelection(transaction, selectionInfo, command, origin, rightOrigin);
        }
        else
        {
            HandleSelection(transaction, selectionInfo, command, origin, rightOrigin);
        }
    }

    private static void InsertWithDisplayText(ITransaction transaction, SelectionInfo selectionInfo, AddLinkCommand command)
    {
        if (command.DisplayText is null)
        {
            throw new ArgumentException("Expected to have a DisplayText with SelectionDirection None.");
        }
            
        var target = transaction.Document.FindNode(selectionInfo.AnchorNodeId);
        if (target is null)
        {
            throw new ArgumentException($"Node with given id not found: {selectionInfo.AnchorId}");
        }
            
        if (target is not TextNode textNode)
        {
            throw new ArgumentException("Expected TextNode at selection.");
        }

        DNode? textOrigin;
        DNode? textRightOrigin;
            
        if (selectionInfo.AnchorOffset > 0)
        {
            textOrigin = textNode;
            textRightOrigin = transaction.SplitText(textNode, selectionInfo.AnchorOffset) ?? textNode.RightOrigin;
        }
        else
        {
            textOrigin = textNode.Origin;
            textRightOrigin = textNode;
        }

        var linkNode = transaction.NodeFactory.LinkNode(textOrigin, textRightOrigin, command.Href);
        var text = transaction.NodeFactory.String(command.DisplayText, null, null);
        transaction.Insert(text, linkNode);
        transaction.Insert(linkNode, textOrigin?.Parent ?? textRightOrigin?.Parent!);
        transaction.AddCursorPosition(text.Id, text.TextContent.Length);
    }

    private static void HandleSelection(
        ITransaction transaction,
        SelectionInfo selectionInfo,
        AddLinkCommand command,
        DNode origin,
        DNode rightOrigin)
    {
        
    }

    private static void HandleSimpleSelection(
        ITransaction transaction,
        SelectionInfo selectionInfo,
        AddLinkCommand command,
        DNode origin,
        DNode rightOrigin)
    {
        var nodes = GetSelectedNodes(transaction, selectionInfo, origin, rightOrigin);
        var linkNode = transaction.NodeFactory.LinkNode(nodes.Origin.Node, nodes.RightOrigin.Node, command.Href);
        var parent = nodes.Origin.Node?.Parent ?? nodes.RightOrigin.Node?.Parent;
        transaction.Insert(linkNode, parent!);
        transaction.MoveRange(nodes.Origin.Node, nodes.RightOrigin.Node, linkNode, null);
    }
    
    private static DNodeSearchResult GetSelectedNodes(
        ITransaction transaction,
        SelectionInfo selectionInfo,
        DNode? start,
        DNode end)
    {
        if (start is not TextNode originText || end is not TextNode)
        {
            throw new ArgumentException("Expected TextNode at selection.");
        }

        var walker = TreeWalker<TextNode>.StartFrom(originText, transaction.Root);
        
        var (originOffset, rightOriginOffset) = selectionInfo.GetConvertedOffsets();
        if (originOffset > 0)
        {
            transaction.SplitText(originText, originOffset);
            start = walker.NextNode();
        }

        if (selectionInfo.AnchorId != selectionInfo.FocusId)
        {
            while (walker.Node is not null && walker.Node.Id != end.Id)
            {
                walker.NextNode();
            }
        }

        if (walker.Node is not null && rightOriginOffset > 0)
        {
            var offset = selectionInfo.AnchorId != selectionInfo.FocusId
                ? rightOriginOffset
                : rightOriginOffset - originOffset;
            transaction.SplitText(walker.Node, offset);
        }
        
        return new DNodeSearchResult(DNodeInfo.From(start, 0), DNodeInfo.From(end, 0));
    }
}