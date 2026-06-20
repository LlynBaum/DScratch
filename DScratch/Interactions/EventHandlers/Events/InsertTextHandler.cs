using DScratch.Interactions.EventHandlers.Common;
using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Events;

public class InsertTextHandler(IDScratchService dScratchService) : EventWithSelectionBase(dScratchService)
{
    public const string EventName = "insertText";

    protected override DNodeInfo HandleNoneSelection(KeyPressInfo keyPressInfo, ITransaction transaction, TextNode anchorTextNode)
    {
        if (keyPressInfo.Selection.AnchorOffset is 0 || string.IsNullOrEmpty(keyPressInfo.Data))
        {
            return DNodeInfo.NotFound();
        }

        var rightOrigin = transaction.SplitText(anchorTextNode, keyPressInfo.Selection.AnchorOffset);
        var offset = rightOrigin is not null ? 0 : anchorTextNode.Length;
        return new DNodeInfo(rightOrigin ?? anchorTextNode, offset);
    }
    
    protected override void HandleEmptyBlock(KeyPressInfo keyPressInfo, ITransaction transaction, DNode anchorNode)
    {
        if (string.IsNullOrEmpty(keyPressInfo.Data))
        {
            return;
        }
        
        // When we get a block element as anchor, we assume there are no TextNode within the block. So we just insert the text.
        // To prevent any broken Trees we insert it before the FirstChild, in case there are child nodes.
        var textNode = transaction.NodeFactory.String(keyPressInfo.Data, anchorNode.FirstChild, null);
        transaction.Insert(textNode, anchorNode);
        transaction.AddCursorPosition(textNode.Id, textNode.Length);
    }

    protected override void OnAfterSelection(
        KeyPressInfo keyPressInfo,
        ITransaction transaction,
        DNode? anchorNode,
        DNodeInfo nodeInfo)
    {
        if (string.IsNullOrEmpty(keyPressInfo.Data))
        {
            return;
        }

        if (nodeInfo.HasFoundNode)
        {
            var textNode = transaction.NodeFactory.String(keyPressInfo.Data, nodeInfo.Node, nodeInfo.Node.RightOrigin);
            var parent = nodeInfo.Node.Parent;
            transaction.Insert(textNode, parent!);
            transaction.AddCursorPosition(textNode.Id, textNode.Length);
        }
        else if (anchorNode?.Parent is not null)
        {
            var textNode = transaction.NodeFactory.String(keyPressInfo.Data, null, anchorNode.FirstChild);
            var parent = anchorNode.Parent;
            transaction.Insert(textNode, parent);
            transaction.AddCursorPosition(textNode.Id, textNode.Length);
        }
    }
}