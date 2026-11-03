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
        
        var linkOrigin = GetLinkAnchor(target, selectionInfo.AnchorOffset - 1);
        var linkRightOrigin = GetLinkRightAnchor(target, selectionInfo.AnchorOffset);
        CreateLink(linkOrigin, linkRightOrigin, target.Parent!);
        return;
        
        static NodeId? GetLinkAnchor(DNode node, int offset)
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
        
        static NodeId? GetLinkRightAnchor(DNode node, int offset)
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
        
        var startLink = transaction.NodeFactory.LinkNode(leftOriginId, null, command.Href, command.Target);
        transaction.Insert(startLink, origin.Parent!);
        transaction.MoveRange(anchor.nodeId, startLink.Id, startLink, null);
        
        var currentBlock = origin.GetNearestBlock().NextSibling();
        var endBlock = rightOrigin.GetNearestBlock();

        while (currentBlock is not null && currentBlock.Id != endBlock.Id)
        {
            var link = transaction.NodeFactory.LinkNode(null, null, command.Href, command.Target);
            transaction.Insert(link, currentBlock);
            transaction.MoveRange(currentBlock.FirstChild?.Id, null, link, null);
            
            currentBlock = currentBlock.NextSibling();
        }
        
        var endLink = transaction.NodeFactory.LinkNode(null, rightAnchor.nodeId, command.Href, command.Target);
        transaction.Insert(endLink, endBlock);
        transaction.MoveRange(null, rightAnchor.nodeId, endLink, null);

        transaction.AddCursorPosition(() =>
        {
            if (endLink.LastChild is null)
            {
                return new SelectionInfo
                {
                    AnchorId = "",
                    AnchorOffset = 0,
                    FocusId = "",
                    FocusOffset = 0
                };
            }
            var offset = (int)(endLink.LastChild!.LastId.Clock - endLink.LastChild.Id.Clock + 1);
            return new SelectionInfo
            {
                AnchorId = endLink.LastChild!.Id.Value,
                AnchorOffset = offset,
                FocusId = endLink.LastChild!.Id.Value,
                FocusOffset = offset
            };
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
        transaction.MoveRange(anchor.nodeId, rightAnchor.nodeId, linkNode, null);
        
        transaction.AddCursorPosition(() =>
        {
            var offset = (int)(linkNode.LastChild!.LastId.Clock - linkNode.LastChild.Id.Clock + 1);
            return new SelectionInfo
            {
                AnchorId = linkNode.LastChild!.Id.Value,
                AnchorOffset = offset,
                FocusId = linkNode.LastChild!.Id.Value,
                FocusOffset = offset
            };
        });
    }
    
    private static (DNode node, NodeId nodeId) GetAnchor(DNode node, int offset)
    {
        return (node, new NodeId(node.Id.Client, node.Id.Clock + offset));
    }
        
    private static (DNode node, NodeId nodeId) GetRightAnchor(DNode node, int offset)
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