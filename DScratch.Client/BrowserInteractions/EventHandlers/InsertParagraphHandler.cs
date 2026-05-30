using DScratch.Client.BrowserInteractions.EventHandlers.Common;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers;

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

        DNode? firstNodeToMove;
        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            firstNodeToMove = SimpleInsert(keyPressInfo, transaction, sibling);
        }
        else
        {
            var nodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction, sibling);
            firstNodeToMove = nodeSearchResult.RightOrigin.Node;
        }

        var (origin, rightOrigin) = GetOrigins(keyPressInfo, sibling);
        var paragraph = dScratchService.NodeFactory.Paragraph(origin, rightOrigin);
        transaction.Insert(paragraph, sibling.Parent);
        
        if (keyPressInfo.Selection.Offset > 0)
        {
            transaction.MoveRange(firstNodeToMove, null, paragraph, null);
        }

        var cursorTarget = keyPressInfo.Selection.Offset > 0 ? paragraph : rightOrigin!;
        transaction.AddCursorPosition(cursorTarget.Id, 0);
        return dScratchService.Apply(transaction);
    }

    private static DNode? SimpleInsert(KeyPressInfo keyPressInfo, ITransaction transaction, DNode parent)
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

        var relativeOffset = keyPressInfo.Selection.Offset - currentOffset;
        return currentNode is null || relativeOffset <= 0 
            ? currentNode 
            : transaction.SplitText(currentNode, relativeOffset) ?? currentNode.RightOrigin;
    }

    private static (DNode? origin, DNode? rightOrigin) GetOrigins(KeyPressInfo keyPressInfo, DNode sibling)
    {
        return keyPressInfo.Selection.Offset <= 0 ? (sibling.Origin, sibling) : (sibling, sibling.RightOrigin);
    }
}