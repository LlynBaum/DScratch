using DScratch.Interactions.EventHandlers.Common;
using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Events;

public class InsertParagraphHandler(IDScratchService dScratchService) : EventWithSelectionBase(dScratchService)
{
    public const string EventName = "insertParagraph";

    protected override DNodeSearchResult HandleNoneSelection(
        KeyPressInfo keyPressInfo,
        ITransaction transaction,
        TextNode anchorTextNode)
    {
        if (keyPressInfo.Selection.AnchorOffset <= 0)
        {
            var parent = anchorTextNode.GetNearestBlock();
            return new DNodeSearchResult(
                Origin: DNodeInfo.NotFound(), 
                RightOrigin: DNodeInfo.From(parent.FirstChild, 0));
        }

        var rightOrigin = transaction.SplitText(anchorTextNode, keyPressInfo.Selection.AnchorOffset);
        return new DNodeSearchResult(
            Origin: new DNodeInfo(anchorTextNode, anchorTextNode.Length), 
            RightOrigin: DNodeInfo.From(rightOrigin, 0));
    }
    
    protected override void HandleEmptyBlock(KeyPressInfo keyPressInfo, ITransaction transaction, DNode anchorNode)
    {
        if (anchorNode.Parent is null)
        {
            throw new ArgumentException("Expected node to have a parent.");
        }
        
        var paragraph = transaction.NodeFactory.Paragraph(anchorNode, anchorNode.RightOrigin);
        transaction.Insert(paragraph, anchorNode.Parent);
        transaction.AddCursorPosition(paragraph.Id, 0);
    }

    protected override void OnAfterSelection(KeyPressInfo keyPressInfo,
        ITransaction transaction,
        DNode anchorNode,
        DNodeSearchResult nodeSearchResult)
    {
        var siblingBlock = nodeSearchResult.Origin.Node?.GetNearestBlock()
                           ?? nodeSearchResult.RightOrigin.Node?.GetNearestBlock() 
                           ?? anchorNode.GetNearestBlock();
        
        if (siblingBlock.Parent is null)
        {
            // Even blocks at least have to have root as a parent.
            throw new ArgumentException($"Expected an block at {keyPressInfo.Selection.AnchorId} with a parent node.");
        }
        
        var (origin, rightOrigin) = GetOrigins(keyPressInfo, siblingBlock);
        var paragraph = transaction.NodeFactory.Paragraph(origin, rightOrigin);

        transaction.Insert(paragraph, siblingBlock.Parent!);
        
        if (keyPressInfo.Selection.AnchorOffset > 0 && nodeSearchResult.Origin.HasFoundNode)
        {
            transaction.MoveRange(nodeSearchResult.Origin.Node.RightOrigin, null, paragraph, null);
        }
        
        var cursorTarget = keyPressInfo.Selection.AnchorOffset > 0 ? paragraph : rightOrigin!;
        transaction.AddCursorPosition(cursorTarget.Id, 0);
    }
    
    private static (DNode? origin, DNode? rightOrigin) GetOrigins(KeyPressInfo keyPressInfo, DNode sibling)
    {
        return keyPressInfo.Selection.AnchorOffset <= 0 ? (sibling.Origin, sibling) : (sibling, sibling.RightOrigin);
    }
}