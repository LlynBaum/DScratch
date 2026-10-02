using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Common;

internal abstract class EventWithSelectionBase(IDScratchService dScratchService) : IEditorEventHandler
{
    protected readonly EventHandlerContext Context = new EventHandlerContext();

    protected abstract void HandleEvent(KeyPressInfo keyPressInfo, ITransaction transaction);
    
    public TransactionResult Handle(KeyPressInfo keyPressInfo)
    {
        var transaction = dScratchService.StartTransaction();
        var targetNode = transaction.Document.FindNode(keyPressInfo.Selection!.AnchorNodeId);
        if (targetNode is null)
        {
            throw new ArgumentException($"Node not found: {keyPressInfo.Selection.AnchorId}");
        }

        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            if (targetNode is TextNode targetTextNode)
            {
                if (keyPressInfo.Selection.FocusOffset > 0)
                {
                    var anchorId = new NodeId(
                        client: targetTextNode.Id.Client, 
                        clock: targetTextNode.Id.Clock + keyPressInfo.Selection.FocusOffset);
                    Context.SetAnchors(targetTextNode, anchorId);
                }
                else
                {
                    Context.SetRightAnchor(targetTextNode);
                }
            }
            else if (SearchTextNode(targetNode, keyPressInfo.Selection) is { Node: not null } result)
            {
                if (keyPressInfo.Selection.FocusOffset > 0)
                {
                    var anchorId = new NodeId(
                        client: result.Node.Id.Client, 
                        clock: result.Node.Id.Clock + keyPressInfo.Selection.FocusOffset - result.Offset);
                    Context.SetAnchors(result.Node, anchorId);
                }
                else
                {
                    Context.SetRightAnchor(result.Node);
                }
            }
            else
            {
                // In case FirstChild is null, we need to set the parent manually
                Context.SetCustomParent(targetNode);
                Context.SetRightAnchor(targetNode.FirstChild);
            }
        }
        else
        {
            DeleteSelection.Handle(keyPressInfo, transaction, Context);
        }
        
        HandleEvent(keyPressInfo, transaction);
        return dScratchService.Apply(transaction);
    }

    private static (TextNode? Node, int Offset) SearchTextNode(DNode targetNode, SelectionInfo selection)
    {
        var walker = new TreeWalker<TextNode>(targetNode);

        var node = walker.NextNode();
        var offset = 0;
        while (walker.Node is not null)
        {
            if (walker.Node.TextContent.Length + offset >= selection.AnchorOffset)
            {
                node = walker.Node;
                break;
            }

            offset += walker.Node.TextContent.Length;
            walker.NextNode();
        }

        return (node, offset);
    }
}