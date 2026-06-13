using DScratch.Client.BrowserInteractions.EventHandlers.Common;
using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers.Events;

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

        var parent = transaction.FindNode(keyPressInfo.Selection.AnchorNodeId);
        if (parent is null)
        {
            throw new ArgumentException($"Parent with given path not found: {keyPressInfo.Selection.AnchorId}");
        }

        DNodeSearchResult dNodeSearchResult;
        DNode? rightOrigin;
        DNode? origin;
        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            dNodeSearchResult = SimpleInsert(keyPressInfo, parent);
            origin = dNodeSearchResult.Origin.Node;
            rightOrigin = dNodeSearchResult.RightOrigin.Node;
        }
        else
        {
            dNodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction, parent);
            origin = dNodeSearchResult.Origin.Node;
            rightOrigin = origin?.RightOrigin;
        }
        
        var textNode = transaction.NodeFactory.String(keyPressInfo.Data, origin, rightOrigin);
        var targetParent = origin?.Parent ?? rightOrigin?.Parent ?? parent;
        transaction.Insert(textNode, targetParent);
        
        var cursorPosition = dNodeSearchResult.Origin.AbsolutOffset + textNode.Length;
        transaction.AddCursorPosition(targetParent.Id, cursorPosition);
        return dScratchService.Apply(transaction);
    }

    private static DNodeSearchResult SimpleInsert(KeyPressInfo keyPressInfo, DNode parent)
    {
        if (keyPressInfo.Selection.AnchorOffset <= 0)
        {
            return new DNodeSearchResult(
                Origin: new DNodeInfo(null, 0),
                RightOrigin: new DNodeInfo(parent.FirstChild, 0));
        }

        var walker = new TreeWalker<TextNode>(parent);

        var currentOffset = 0;
        var currentNode = walker.FirstChild();
        while (currentNode is not null)
        {
            if (currentOffset + currentNode.Length >= keyPressInfo.Selection.AnchorOffset)
            {
                break;
            }

            currentOffset += currentNode.Length;
            currentNode = walker.NextSibling();
        }

        return new DNodeSearchResult(
            Origin: new DNodeInfo(currentNode, keyPressInfo.Selection.AnchorOffset), 
            RightOrigin: new DNodeInfo(walker.NextSibling(), currentOffset + currentNode?.Length ?? 0));
    }
}