using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.CommandHandlers.Handlers;

public class RemoveLinkHandler(IDScratchService dScratchService) : CommandBase<RemoveLinkCommand>(dScratchService)
{
    protected override void Handle(ITransaction transaction, SelectionInfo selectionInfo, RemoveLinkCommand command)
    {
        if (selectionInfo.Direction is not SelectionDirection.None)
        {
            return;
        }
        
        var anchor = transaction.Document.FindNodeOrDefault(selectionInfo.AnchorNodeId);
        var linkNode = FindLinkNode(anchor);
        if (linkNode is null)
        {
            return;
        }
        
        transaction.MoveRange(linkNode.FirstChild?.Id, linkNode.LastChild?.LastId, linkNode.Parent!, linkNode.PreviousSibling()?.LastId);
        transaction.Delete(linkNode.Id);
        transaction.AddCursorPosition(selectionInfo);
    }

    private static LinkNode? FindLinkNode(DNode? node)
    {
        return node switch
        {
            null => null,
            LinkNode linkNode => linkNode,
            _ => FindLinkNode(node.Parent)
        };
    }
}