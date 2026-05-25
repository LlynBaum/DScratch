using DScratch.Client.Scripts.EventHandlers.Common;
using DScratch.Nodes;
using DScratch.Transactions.Steps;

namespace DScratch.Client.Scripts.EventHandlers;

public class InsertParagraphHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "insertParagraph";
    
    public TransactionResult Handle(KeyPressInfo keyPressInfo)
    {
        var transaction = dScratchService.StartTransaction();
        
        var sibling = transaction.FindNode(keyPressInfo.GetNodePath());
        if (sibling is null)
        {
            throw new ArgumentException($"Sibling node with given path not found: {keyPressInfo.GetNodePath()}");
        }

        if (sibling.Parent is null)
        {
            throw new ArgumentException($"Sibling node with given path has no parent: {keyPressInfo.GetNodePath()}");
        }
        
        DNode? rightOriginTextNode;
        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            rightOriginTextNode = SimpleInsert(keyPressInfo, sibling);
        }
        else
        {
            // TODO: deleting over two paragraphs will be more complex. Need to merge them together in that case...
            var (originTextNode, _) = DeleteSelection.Handle(keyPressInfo, transaction, sibling.Parent);
            rightOriginTextNode = originTextNode?.RightOrigin;
        }

        var (origin, rightOrigin) = GetOrigins(keyPressInfo, sibling);
        var paragraph = dScratchService.NodeFactory.Paragraph(origin, rightOrigin);
        transaction.Insert(paragraph, sibling.Parent);
        transaction.MoveRange(rightOriginTextNode, null, paragraph, null);
        return dScratchService.Apply(transaction);
    }

    private static DNode? SimpleInsert(KeyPressInfo keyPressInfo, DNode parent)
    {
        if (keyPressInfo.Selection.Offset <= 0)
        {
            return parent.FirstChild;
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

        return walker.NextSibling();
    }

    private static (DNode? origin, DNode? rightOrigin) GetOrigins(KeyPressInfo keyPressInfo, DNode sibling)
    {
        return keyPressInfo.Selection.Offset <= 0 ? (sibling.Origin, sibling) : (sibling, sibling.RightOrigin);
    }
}