using DScratch.Client.Scripts.EventHandlers.Common;
using DScratch.Nodes;
using DScratch.Transactions.Steps;

namespace DScratch.Client.Scripts.EventHandlers;

public class InsertTextHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "insertText";
    
    public TransactionResult Handle(KeyPressInfo keyPressInfo)
    {
        if (string.IsNullOrEmpty(keyPressInfo.Data))
        {
            return TransactionResult.Empty;
        }
        
        var transaction = dScratchService.StartTransaction();

        var parent = transaction.FindNode(keyPressInfo.GetNodePath());
        if (parent is null)
        {
            throw new ArgumentException($"Parent with given path not found: {keyPressInfo.GetNodePath()}");
        }

        DNode? rightOrigin;
        DNode? origin;
        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            (origin, rightOrigin) = SimpleInsert(keyPressInfo, parent);
        }
        else
        {
            // TODO: deleting over two paragraphs will be more complex. Need to merge them together in that case...
            (origin, _) = DeleteSelection.Handle(keyPressInfo, transaction, parent);
            rightOrigin = origin?.RightOrigin;
        }
        
        var textNode = dScratchService.NodeFactory.String(keyPressInfo.Data, origin, rightOrigin);
        transaction.Insert(textNode, parent);
        return dScratchService.Apply(transaction);
    }

    private static (DNode? origin, DNode? rightOrigin) SimpleInsert(KeyPressInfo keyPressInfo, DNode parent)
    {
        if (keyPressInfo.Selection.Offset <= 0)
        {
            return (null, parent.FirstChild);
        }

        var walker = new TreeWalker<TextNode>(parent);

        var currentOffset = 0;
        var currentNode = walker.FirstChild();
        while (currentNode is not null)
        {
            var length = currentNode.Length;
            if (currentOffset + length >= keyPressInfo.Selection.Offset)
            {
                break;
            }

            currentOffset += length;
            currentNode = walker.NextSibling();
        }

        return (currentNode, walker.NextSibling());
    }
}