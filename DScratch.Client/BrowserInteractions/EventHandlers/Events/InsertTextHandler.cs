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

        int offset;
        DNode? rightOrigin;
        DNode? origin;
        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            if (targetNode is not TextNode targetTextNode)
            {
                throw new ArgumentException("Expected TextNode as target for insert with no selection.", nameof(keyPressInfo));
            }

            offset = keyPressInfo.Selection.AnchorOffset;
            origin = targetTextNode;
            rightOrigin = targetTextNode.RightOrigin;
        }
        else
        {
            var dNodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction, targetNode.GetNearestBlock());
            origin = dNodeSearchResult.Origin.Node;
            rightOrigin = origin?.RightOrigin;
            offset = dNodeSearchResult.Origin.AbsolutOffset;
        }

        var textNode = transaction.NodeFactory.String(keyPressInfo.Data, origin, rightOrigin);
        var targetParent = origin?.Parent ?? rightOrigin?.Parent ?? targetNode;
        transaction.Insert(textNode, targetParent);
        
        var cursorPosition = offset + textNode.Length;
        transaction.AddCursorPosition(targetParent.Id, cursorPosition);
        return dScratchService.Apply(transaction);
    }
}