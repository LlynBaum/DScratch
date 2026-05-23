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
        if (keyPressInfo.Selection.Direction is SelectionDirection.None) // TODO: test test test
        {
            var walker = new TreeWalker<TextNode>(parent);
            var node = walker.FirstChild();

            var currentOffset = 0;
            while (node is not null)
            {
                var length = node.Length;
                if (currentOffset + length >= keyPressInfo.Selection.Offset - 1)
                {
                    break;
                }

                currentOffset += length;
                node = walker.NextSibling();
            }

            // if right origin char is not a child of this node, it should be null, since they are not siblings in that case
            origin = node?.ChildAt(keyPressInfo.Selection.Offset - 1 - currentOffset);
            rightOrigin = node?.ChildAt(keyPressInfo.Selection.Offset - currentOffset);
        }
        else
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
}