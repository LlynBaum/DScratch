using DScratch.Interactions.EventHandlers.Common;
using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Events;

public class InsertParagraphHandler(IDScratchService dScratchService) : EventWithSelectionBase(dScratchService)
{
    public const string EventName = "insertParagraph";

    protected override void OnAfterSelection(
        KeyPressInfo keyPressInfo,
        ITransaction transaction,
        TextNode anchorTextNode,
        DNodeInfo nodeInfo)
    {
        var siblingBlock = nodeInfo.Node?.GetNearestBlock() ?? anchorTextNode.GetNearestBlock();
        if (siblingBlock.Parent is null)
        {
            // Even blocks at least have to have root as a parent.
            throw new ArgumentException($"Sibling node with given path has no parent: {keyPressInfo.Selection.AnchorId}");
        }
        
        var (origin, rightOrigin) = GetOrigins(keyPressInfo, siblingBlock);
        var paragraph = transaction.NodeFactory.Paragraph(origin, rightOrigin);

        transaction.Insert(paragraph, siblingBlock.Parent!);
        
        if (keyPressInfo.Selection.AnchorOffset > 0)
        {
            transaction.MoveRange(nodeInfo.Node?.RightOrigin, null, paragraph, null);
        }
        
        var cursorTarget = keyPressInfo.Selection.AnchorOffset > 0 ? paragraph : rightOrigin!;
        transaction.AddCursorPosition(cursorTarget.Id, 0);
    }

    protected override DNodeInfo HandleNoneSelection(KeyPressInfo keyPressInfo, ITransaction transaction, TextNode anchorTextNode)
    {
        if (keyPressInfo.Selection.AnchorOffset <= 0)
        {
            var parent = anchorTextNode.GetNearestBlock();
            return DNodeInfo.From(parent.FirstChild, 0);
        }

        transaction.SplitText(anchorTextNode, keyPressInfo.Selection.AnchorOffset);
        return DNodeInfo.From(anchorTextNode, 0);
    }

    private static (DNode? origin, DNode? rightOrigin) GetOrigins(KeyPressInfo keyPressInfo, DNode sibling)
    {
        return keyPressInfo.Selection.AnchorOffset <= 0 ? (sibling.Origin, sibling) : (sibling, sibling.RightOrigin);
    }
}