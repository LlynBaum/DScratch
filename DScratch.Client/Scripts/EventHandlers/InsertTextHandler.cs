using DScratch.Nodes;
using DScratch.Transactions.Steps;

namespace DScratch.Client.Scripts.EventHandlers;

public class InsertTextHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "insertText";
    
    public TransactionResult Handle(KeyPressInfo keyPressInfo)
    {
        if (keyPressInfo.Data is null)
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
        else  // TODO: test test test
        {
            var rightOriginOffset = keyPressInfo.Selection.Direction is SelectionDirection.Forward
                ? keyPressInfo.Selection.Offset
                : keyPressInfo.Selection.EndOffset;
            
            var originOffset = keyPressInfo.Selection.Direction is SelectionDirection.Forward
                ? keyPressInfo.Selection.EndOffset
                : keyPressInfo.Selection.Offset;
            
            rightOrigin = parent.ChildAt(rightOriginOffset);
            origin = parent.ChildAt(originOffset);
            
            // TODO: slice and then delete new slice
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
            if (currentOffset + length >= keyPressInfo.Selection.Offset - 1)
            {
                break;
            }

            currentOffset += length;
            currentNode = walker.NextSibling();
        }

        return (currentNode, walker.NextSibling());
    }
}