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

        var targetNode = transaction.FindNode(keyPressInfo.Selection.AnchorNodeId);
        if (targetNode is null)
        {
            throw new ArgumentException($"Parent with given path not found: {keyPressInfo.Selection.AnchorId}");
        }

        DNode? rightOrigin;
        DNode? origin;
        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            if (targetNode is not TextNode targetTextNode)
            {
                throw new ArgumentException("Expected TextNode as target for insert with no selection.", nameof(keyPressInfo));
            }

            var searchResult = SimpleInsert(targetTextNode, keyPressInfo.Selection, transaction);

            origin = searchResult.Origin.Node;
            rightOrigin = searchResult.RightOrigin.Node;
        }
        else
        {
            var dNodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction);
            origin = dNodeSearchResult.Origin.Node;
            rightOrigin = origin?.RightOrigin;
        }

        var textNode = transaction.NodeFactory.String(keyPressInfo.Data, origin, rightOrigin);
        var parent = origin?.Parent ?? rightOrigin?.Parent ?? targetNode.Parent;
        transaction.Insert(textNode, parent!);
        
        transaction.AddCursorPosition(textNode.Id, textNode.Length);
        return dScratchService.Apply(transaction);
    }

    private static DNodeSearchResult SimpleInsert(TextNode targetNode, SelectionInfo selectionInfo, ITransaction transaction)
    {
        if (selectionInfo.AnchorOffset is 0)
        {
            return new DNodeSearchResult(
                Origin: new DNodeInfo(null, -1), 
                RightOrigin: new DNodeInfo(targetNode, 0));
        }

        var rightOrigin = transaction.SplitText(targetNode, selectionInfo.AnchorOffset);
        
        return new DNodeSearchResult(
            Origin: new DNodeInfo(targetNode, selectionInfo.AnchorOffset), 
            RightOrigin: new DNodeInfo(rightOrigin, 0));
    }
}