using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;
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
        if (target is IBlockElement)
        {
            CreateLink(null, null, target);
            return;
        }
        
        var linkOrigin = GetAnchor(target, selectionInfo.AnchorOffset - 1);
        var linkRightOrigin = GetRightAnchor(target, selectionInfo.AnchorOffset);
        CreateLink(linkOrigin, linkRightOrigin, target.Parent!);
        return;
        
        static NodeId? GetAnchor(DNode node, int offset)
        {
            var targetId = new NodeId(node.Id.Client, node.Id.Clock + offset);
            if (targetId.Clock >= node.Id.Clock)
            {
                return targetId;
            }
            
            return node.PreviousSibling() is { } previousSibling
                ? previousSibling.LastId
                : null;
        }
        
        static NodeId? GetRightAnchor(DNode node, int offset)
        {
            var targetId = new NodeId(node.Id.Client, node.Id.Clock + offset);
            if (targetId.Clock <= node.LastId.Clock)
            {
                return targetId;
            }

            return node.NextSibling() is { } nextSibling
                ? nextSibling.Id
                : null;
        }

        void CreateLink(NodeId? origin, NodeId? rightOrigin, DNode parent)
        {
            var linkNode = transaction.NodeFactory.LinkNode(origin, rightOrigin, command.Href, command.Target);
            var text = transaction.NodeFactory.String(command.DisplayText, null, null);
            transaction.Insert(linkNode, parent);
            transaction.Insert(text, linkNode);
            transaction.AddCursorPosition(text, text.TextContent.Length);
        }
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
        
        var startLink = transaction.NodeFactory.LinkNode(startNode?.PreviousSibling()?.LastId, null, command.Href, command.Target);
        transaction.Insert(startLink, origin.Parent!);
        transaction.MoveRange(startNode?.Id, null, startLink, null);
        
        var currentBlock = origin.GetNearestBlock().NextSibling();
        var endBlock = rightOrigin.GetNearestBlock();

        while (currentBlock is not null && currentBlock.Id != endBlock.Id)
        {
            var link = transaction.NodeFactory.LinkNode(null, null, command.Href, command.Target);
            transaction.Insert(link, currentBlock);
            transaction.MoveRange(currentBlock.FirstChild?.Id, null, link, null);
            
            currentBlock = currentBlock.NextSibling();
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
            endNode = rightOrigin.PreviousSibling();
        }
        
        var endLink = transaction.NodeFactory.LinkNode(endNode?.LastId, endNode?.NextSibling()?.Id, command.Href, command.Target);
        transaction.Insert(endLink, endBlock);
        transaction.MoveRange(null, endNode?.Id, endLink, null);

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
        var (originOffset, rightOriginOffset) = selectionInfo.GetConvertedOffsets();
        var anchor = GetAnchor(origin, originOffset);
        var rightAnchor = GetRightAnchor(rightOrigin, rightOriginOffset);

        NodeId? leftOriginId;
        if (anchor.node.Id.Clock != anchor.nodeId.Clock)
        {
            leftOriginId = new NodeId(anchor.nodeId.Client, anchor.nodeId.Clock - 1);
        }
        else
        {
            leftOriginId = anchor.node.PreviousSibling()?.LastId;
        }
        
        var linkNode = transaction.NodeFactory.LinkNode(leftOriginId, rightAnchor.nodeId, command.Href, command.Target);
        var parent = anchor.node.Parent ?? rightAnchor.node.Parent ?? throw new InvalidOperationException("Expected at least one node to be not null and have a parent");
        transaction.Insert(linkNode, parent);
        
        // The LinkNode wil get inserted before the rightAnchor, so we must set as end node the link itself, so it doesn't get moved
        transaction.MoveRange(anchor.nodeId, linkNode.Id, linkNode, null);
        
        transaction.AddCursorPosition(() => new SelectionInfo
        {
            AnchorId = linkNode.FirstChild!.Id.Value,
            AnchorOffset = 0,
            FocusId = linkNode.LastChild!.Id.Value,
            FocusOffset = (int)(linkNode.LastChild.LastId.Clock - linkNode.LastChild.Id.Clock + 1)
        });
        return;
        
        static (DNode node, NodeId nodeId) GetAnchor(DNode node, int offset)
        {
            return (node, new NodeId(node.Id.Client, node.Id.Clock + offset));
        }
        
        static (DNode node, NodeId nodeId) GetRightAnchor(DNode node, int offset)
        {
            var targetId = new NodeId(node.Id.Client, node.Id.Clock + offset);
            if (targetId.Clock <= node.LastId.Clock)
            {
                return (node, targetId);
            }

            return node.NextSibling() is { } nextSibling
                ? (nextSibling, nextSibling.Id)
                : (node.Parent!, node.Parent!.Id);
        }
    }
}