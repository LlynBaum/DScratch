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
        if (string.IsNullOrEmpty(command.DisplayText))
        {
            throw new ArgumentException("Expected to have a DisplayText with SelectionDirection None.");
        }
        
        var target = transaction.Document.FindNode(selectionInfo.AnchorNodeId);
        if (target is null)
        {
            throw new ArgumentException($"Node with given id not found: {selectionInfo.AnchorId}");
        }
            
        
        DNode? textOrigin;
        DNode? textRightOrigin;
        DNode parent;
        
        if (target is TextNode textNode)
        {
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
            parent = textOrigin?.Parent ?? textRightOrigin?.Parent!;
        }
        else
        {
            textOrigin = null;
            textRightOrigin = null;
            parent = target;
        }
        
        var linkNode = transaction.NodeFactory.LinkNode(textOrigin, textRightOrigin, command.Href, command.Target);
        var text = transaction.NodeFactory.String(command.DisplayText, null, null);
        transaction.Insert(linkNode, parent);
        transaction.Insert(text, linkNode);
        transaction.AddCursorPosition(text.Id, text.TextContent.Length);
    }

    private static void HandleSelection(
        ITransaction transaction,
        SelectionInfo selectionInfo,
        AddLinkCommand command,
        DNode origin,
        DNode rightOrigin)
    {
        var (originOffset, rightOriginOffset) = selectionInfo.GetConvertedOffsets();

        var startNode = originOffset > 0
            ? origin is TextNode originText 
                ? transaction.SplitText(originText, originOffset) 
                : throw new ArgumentException($"Expected TextNode at {origin.Id}")
            : origin;
        
        var startLink = transaction.NodeFactory.LinkNode(startNode?.Origin, null, command.Href, command.Target);
        transaction.Insert(startLink, origin.Parent!);
        transaction.MoveRange(startNode, null, startLink, null);
        
        var currentBlock = origin.GetNearestBlock().RightOrigin;
        var endBlock = rightOrigin.GetNearestBlock();

        while (currentBlock is not null && currentBlock.Id != endBlock.Id)
        {
            var link = transaction.NodeFactory.LinkNode(null, null, command.Href, command.Target);
            transaction.Insert(link, currentBlock);
            transaction.MoveRange(currentBlock.FirstChild, null, link, null);
            
            currentBlock = currentBlock.RightOrigin;
        }

        DNode? endNode;
        if (rightOriginOffset > 0)
        {
            if (rightOrigin is not TextNode rightOriginText)
            {
                throw new ArgumentException($"Expected TextNode at {rightOrigin.Id}");
            }
            
            transaction.SplitText(rightOriginText, rightOriginOffset);
            endNode = rightOriginText;
        }
        else
        {
            endNode = rightOrigin.Origin;
        }
        
        var endLink = transaction.NodeFactory.LinkNode(endNode, endNode?.RightOrigin, command.Href, command.Target);
        transaction.Insert(endLink, endBlock);
        transaction.MoveRange(null, endNode, endLink, null);

        if (endNode is null || SelectionHelper.NearestTextNode(endNode) is not { HasFoundNode: true } endTextNode)
        {
            throw new ArgumentException($"Expected TextNode near {endNode?.Id}");
        }
        
        transaction.AddCursorPosition(new SelectionInfo
        {
            AnchorId = endTextNode.Node.Id.Value,
            AnchorOffset = endTextNode.Node.Length,
            FocusId = endTextNode.Node.Id.Value,
            FocusOffset = endTextNode.Node.Length
        });
    }

    private static void HandleSimpleSelection(
        ITransaction transaction,
        SelectionInfo selectionInfo,
        AddLinkCommand command,
        DNode origin,
        DNode rightOrigin)
    {
        var nodes = GetSelectedNodes(transaction, selectionInfo, origin, rightOrigin);
        var linkNode = transaction.NodeFactory.LinkNode(nodes.Origin.Node?.Origin, nodes.RightOrigin.Node?.RightOrigin, command.Href, command.Target);
        var parent = nodes.Origin.Node?.Parent ?? nodes.RightOrigin.Node?.Parent!;
        transaction.Insert(linkNode, parent);
        transaction.MoveRange(nodes.Origin.Node, nodes.RightOrigin.Node, linkNode, null);
        
        transaction.AddCursorPosition(new SelectionInfo
        {
            AnchorId = nodes.RightOrigin.Node!.Id.Value,
            AnchorOffset = nodes.RightOrigin.Offset,
            FocusId = nodes.RightOrigin.Node!.Id.Value,
            FocusOffset = nodes.RightOrigin.Offset
        });
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

        TextNode endTextNode;
        if (walker.Node is not null && rightOriginOffset > 0)
        {
            endTextNode = walker.Node;
            var offset = selectionInfo.AnchorId != selectionInfo.FocusId
                ? rightOriginOffset
                : rightOriginOffset - originOffset;
            transaction.SplitText(walker.Node, offset);
        }
        else
        {
            endTextNode = walker.Node?.Origin is not null && SelectionHelper.NearestTextNode(walker.Node.Origin) is
                { HasFoundNode: true } n
                ? n.Node
                : throw new ArgumentException($"Expected TextNode near {walker.Node?.Origin?.Id}");
        }
        
        return new DNodeSearchResult(DNodeInfo.From(start, 0), DNodeInfo.From(endTextNode, endTextNode.Length));
    }
}