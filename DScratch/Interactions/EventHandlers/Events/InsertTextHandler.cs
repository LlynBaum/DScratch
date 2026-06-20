using DScratch.Interactions.EventHandlers.Common;
using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Events;

public class InsertTextHandler(IDScratchService dScratchService) : EventWithSelectionBase(dScratchService)
{
    public const string EventName = "insertText";

    protected override void OnAfterSelection(
        KeyPressInfo keyPressInfo,
        ITransaction transaction,
        TextNode anchorTextNode,
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
        else
        {
            var textNode = transaction.NodeFactory.String(keyPressInfo.Data, null, anchorTextNode.FirstChild);
            var parent = anchorTextNode.Parent;
            transaction.Insert(textNode, parent!);
            transaction.AddCursorPosition(textNode.Id, textNode.Length);
        }
    }

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
}