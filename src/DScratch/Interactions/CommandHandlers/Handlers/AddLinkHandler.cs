using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.CommandHandlers.Handlers;

public class AddLinkHandler(IDScratchService dScratchService) : CommandBase<AddLinkCommand>(dScratchService)
{
    protected override void Handle(ITransaction transaction, SelectionInfo selectionInfo, AddLinkCommand command)
    {
        var nodes = GetSelectedNodes(transaction, selectionInfo);
        var linkNode = transaction.NodeFactory.LinkNode(nodes.Origin.Node, nodes.RightOrigin.Node, command.Href);
        var parent = nodes.Origin.Node?.Parent ?? nodes.RightOrigin.Node?.Parent;
        transaction.Insert(linkNode, parent!);
        transaction.MoveRange(nodes.Origin.Node, nodes.RightOrigin.Node, linkNode, null);
    }
    
    private static DNodeSearchResult GetSelectedNodes(ITransaction transaction, SelectionInfo selectionInfo)
    {
        var (originId, rightOriginId) = selectionInfo.GetConvertedNodeIds();
        
        var start = transaction.Document.FindNode(originId);
        var end = transaction.Document.FindNode(rightOriginId);
        
        if (start is null || end is null)
        {
            throw new ArgumentException($"Node with given id not found: {selectionInfo.AnchorId} or {selectionInfo.FocusId}");
        }

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