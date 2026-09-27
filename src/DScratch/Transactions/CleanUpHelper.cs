using DScratch.Interactions;
using DScratch.Nodes;

namespace DScratch.Transactions;

public static class CleanUpHelper // TODO: test the merge also for the new RightOrigin and Origin check
{
    public static bool CanMergeWithPrevious(TextNode node, out TextNode origin)
    {
        if (node.PreviousSibling() is not TextNode previousTextNode)
        {
            origin = null!;
            return false;
        }
        
        origin = previousTextNode;
        return previousTextNode.IsDeleted == node.IsDeleted
               && previousTextNode.LastId == node.Origin
               && node.RightOrigin == previousTextNode.RightOrigin
               && previousTextNode.LastId.IsContinuesTo(node.Id)
               && previousTextNode.Marks.SequenceEqual(node.Marks); // TODO: key only comparer
    }
    
    public static bool CanMergeWithNext(TextNode node, out TextNode rightOrigin)
    {
        if (node.NextSibling() is not TextNode nextTextNode)
        {
            rightOrigin = null!;
            return false;
        }
        
        rightOrigin = nextTextNode;
        return nextTextNode.IsDeleted == node.IsDeleted 
               && node.LastId == nextTextNode.Origin
               && node.RightOrigin == nextTextNode.RightOrigin
               && node.LastId.IsContinuesTo(nextTextNode.Id)
               && nextTextNode.Marks.SequenceEqual(node.Marks);
    }
    
    public static SelectionInfo? AdjustSelection(SelectionInfo? selectionInfo, TextNode oldNode, TextNode targetNode)
    {
        if (selectionInfo is null) return null;
        
        if (selectionInfo.AnchorId == oldNode.Id.Value && selectionInfo.FocusId == oldNode.Id.Value)
        {
            return new SelectionInfo
            {
                AnchorId = targetNode.Id.Value,
                AnchorOffset = targetNode.Length + selectionInfo.AnchorOffset,
                FocusId = targetNode.Id.Value,
                FocusOffset = targetNode.Length + selectionInfo.AnchorOffset
            };
        }
        
        if (selectionInfo.AnchorId == oldNode.Id.Value && selectionInfo.FocusId != oldNode.Id.Value)
        {
            return new SelectionInfo
            {
                AnchorId = targetNode.Id.Value,
                AnchorOffset = targetNode.Length + selectionInfo.AnchorOffset,
                FocusId = selectionInfo.FocusId,
                FocusOffset = selectionInfo.FocusOffset
            };
        }
        
        if (selectionInfo.AnchorId != oldNode.Id.Value && selectionInfo.FocusId == oldNode.Id.Value)
        {
            return new SelectionInfo
            {
                AnchorId = selectionInfo.AnchorId,
                AnchorOffset = selectionInfo.AnchorOffset,
                FocusId = targetNode.Id.Value,
                FocusOffset = targetNode.Length + selectionInfo.AnchorOffset
            };
        }

        return selectionInfo;
    }
}