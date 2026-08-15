using DScratch.Interactions.CommandHandlers.Commands;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.CommandHandlers.Handlers;

public class UpdateLinkHandler(IDScratchService dScratchService) : CommandBase<UpdateLinkCommand>(dScratchService)
{
    protected override void Handle(ITransaction transaction, SelectionInfo selectionInfo, UpdateLinkCommand command)
    {
        if (selectionInfo.Direction is not SelectionDirection.None)
        {
            return;
        }
        
        var anchor = transaction.Document.FindNode(selectionInfo.AnchorNodeId);
        var linkNode = FindLinkNode(anchor);
        if (linkNode is null)
        {
            return;
        }

        // TODO: CreateUpdate creates a delegate that the transaction will execute and sends a updateAttribute step to TS.
        //transaction.UpdateAttributes(linkNode, linkNode.CreateUpdate(command.Href, command.Target));
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