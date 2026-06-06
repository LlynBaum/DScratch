using DScratch.Client.BrowserInteractions.EventHandlers.Common;
using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers;

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

        NodeSearchResult nodeSearchResult;
        DNode? rightOrigin;
        DNode? origin;
        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            nodeSearchResult = SimpleInsert(keyPressInfo, parent);
            origin = nodeSearchResult.Origin.Node;
            rightOrigin = nodeSearchResult.RightOrigin.Node;
        }
        else
        {
            nodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction, parent);
            origin = nodeSearchResult.Origin.Node;
            rightOrigin = origin?.RightOrigin;
        }
        
        var textNode = dScratchService.NodeFactory.String(keyPressInfo.Data, origin, rightOrigin);
        var targetParent = origin?.Parent ?? rightOrigin?.Parent ?? parent;
        transaction.Insert(textNode, targetParent);
        
        var cursorPosition = nodeSearchResult.Origin.AbsolutOffset + textNode.Length;
        transaction.AddCursorPosition(targetParent.Id, cursorPosition);
        return dScratchService.Apply(transaction);
    }

    private static NodeSearchResult SimpleInsert(KeyPressInfo keyPressInfo, DNode parent)
    {
        if (keyPressInfo.Selection.AnchorOffset <= 0)
        {
            return new NodeSearchResult(
                Origin: new NodeInfo(null, 0),
                RightOrigin: new NodeInfo(parent.FirstChild, 0));
        }

        var walker = new TreeWalker<TextNode>(parent);

        var currentOffset = 0;
        var currentNode = walker.FirstChild();
        while (currentNode is not null)
        {
            var length = currentNode.Length;
            if (currentOffset + length >= keyPressInfo.Selection.AnchorOffset)
            {
                break;
            }

            currentOffset += length;
            currentNode = walker.NextSibling();
        }

        var relativeOffset = keyPressInfo.Selection.AnchorOffset - currentOffset;
        return new NodeSearchResult(
            Origin: new NodeInfo(currentNode, keyPressInfo.Selection.AnchorOffset), 
            RightOrigin: new NodeInfo(walker.NextSibling(), currentOffset + currentNode?.Length ?? 0));
    }
}