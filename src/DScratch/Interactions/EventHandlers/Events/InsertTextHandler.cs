using DScratch.Interactions.EventHandlers.Common;
using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Nodes.Marks;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Events;

public class InsertTextHandler(IDScratchService dScratchService) : EventWithSelectionBase(dScratchService)
{
    public const string EventName = "insertText";

    protected override DNodeSearchResult HandleNoneSelection(
        KeyPressInfo keyPressInfo,
        ITransaction transaction,
        TextNode anchorTextNode)
    {
        if (string.IsNullOrEmpty(keyPressInfo.Data))
        {
            return DNodeSearchResult.Empty;
        }

        if (keyPressInfo.Selection.AnchorOffset is 0)
        {
            return new DNodeSearchResult(DNodeInfo.NotFound(), DNodeInfo.From(anchorTextNode, 0));
        }
        
        var rightOrigin = transaction.SplitText(anchorTextNode, keyPressInfo.Selection.AnchorOffset);
        return new DNodeSearchResult(
            Origin: new DNodeInfo(anchorTextNode, anchorTextNode.Length), 
            RightOrigin: DNodeInfo.From(rightOrigin, 0));
    }

    protected override void HandleEmptyBlock(KeyPressInfo keyPressInfo, ITransaction transaction, DNode anchorNode)
    {
        if (string.IsNullOrEmpty(keyPressInfo.Data))
        {
            return;
        }

        var marks = transaction.PopPendingMarks();
        // When we get a block element as anchor, we assume there are no TextNode within the block. So we just insert the text.
        // To prevent any broken Trees we insert it before the FirstChild, in case there are child nodes.
        var textNode = transaction.NodeFactory.String(keyPressInfo.Data, anchorNode.FirstChild, null, marks);
        transaction.Insert(textNode, anchorNode);
        transaction.AddCursorPosition(textNode.Id, textNode.Length);
    }

    protected override void OnAfterSelection(KeyPressInfo keyPressInfo,
        ITransaction transaction,
        DNode anchorNode,
        DNodeSearchResult nodeSearchResult)
    {
        if (string.IsNullOrEmpty(keyPressInfo.Data))
        {
            return;
        }

        var pendingMarks = transaction.PopPendingMarks();
        
        if (nodeSearchResult.Origin.HasFoundNode)
        {
            var marks = nodeSearchResult.Origin.Node is TextNode t 
                ? pendingMarks.Concat(t.Marks).ToHashSet(new Mark.MarkTable())
                : pendingMarks;

            var textNode = transaction.NodeFactory.String(
                value: keyPressInfo.Data,
                origin: nodeSearchResult.Origin.Node,
                rightOrigin: nodeSearchResult.Origin.Node.RightOrigin,
                initMarks: marks);

            var parent = nodeSearchResult.Origin.Node.Parent;
            transaction.Insert(textNode, parent!);
            transaction.AddCursorPosition(textNode.Id, textNode.Length);
        }
        else if (nodeSearchResult.RightOrigin.HasFoundNode)
        {
            var marks = nodeSearchResult.RightOrigin.Node.Origin is TextNode t 
                ? pendingMarks.Concat(t.Marks).ToHashSet(new Mark.MarkTable())
                : pendingMarks;
            
            var textNode = transaction.NodeFactory.String(
                value: keyPressInfo.Data,
                origin: nodeSearchResult.RightOrigin.Node.Origin,
                rightOrigin: nodeSearchResult.RightOrigin.Node,
                initMarks: marks);

            var parent = nodeSearchResult.RightOrigin.Node.Parent;
            transaction.Insert(textNode, parent!);
            transaction.AddCursorPosition(textNode.Id, textNode.Length);
        }
        else if (anchorNode.Parent is not null)
        {
            var textNode = transaction.NodeFactory.String(
                value: keyPressInfo.Data, 
                origin: null, 
                rightOrigin: anchorNode.FirstChild,
                initMarks: pendingMarks);

            var parent = anchorNode.Parent;
            transaction.Insert(textNode, parent);
            transaction.AddCursorPosition(textNode.Id, textNode.Length);
        }
    }
}