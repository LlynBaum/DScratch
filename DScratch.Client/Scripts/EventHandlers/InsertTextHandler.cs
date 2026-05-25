using DScratch.Nodes;
using DScratch.Transactions;
using DScratch.Transactions.Steps;
using DScratch.TreeVisualizers;

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
            (origin, rightOrigin) = OverrideInsert(keyPressInfo, transaction, parent);
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

    private static (DNode? origin, DNode? rightOrigin) OverrideInsert(
        KeyPressInfo keyPressInfo,
        ITransaction transaction, 
        DNode parent)
    {
        var (originOffset, rightOriginOffset) = keyPressInfo.Selection.GetConvertedOffsets();
        
        var walker = new TreeWalker<TextNode>(parent);
        var currentOffset = 0;
        var currentNode = walker.FirstChild();
        while (currentNode is not null)
        {
            var length = currentNode.Length;
            if (currentOffset + length >= originOffset)
            {
                break;
            }

            currentOffset += length;
            currentNode = walker.NextSibling();
        }

        var relativeOriginOffset = originOffset - currentOffset;
        var origin = currentNode;
        
        while (currentNode is not null)
        {
            var length = currentNode.Length;
            if (currentOffset + length >= rightOriginOffset)
            {
                break;
            }

            currentOffset += length;
            currentNode = walker.NextSibling();
        }

        var relativeRightOriginOffset = rightOriginOffset - currentOffset;
        var rightOrigin = currentNode;

        var deleteStart = origin is not null ? transaction.SplitText(origin, relativeOriginOffset) : null;

        if (origin is not null && rightOrigin is not null && origin.Id == rightOrigin.Id)
        {
            rightOrigin = deleteStart;
            relativeRightOriginOffset -= origin.Length;
        }
        
        if (rightOrigin != null)
        {
            transaction.SplitText(rightOrigin, relativeRightOriginOffset);
        }
        
        transaction.DeleteRange(deleteStart, rightOrigin);
        return (origin, deleteStart);
    }
}