using DScratch.Interactions;
using DScratch.Nodes;
using DScratch.Nodes.Marks;

namespace DScratch.Transactions;

public static class CleanUpHelper
{
    public static bool CanMergeWithOrigin(TextNode node, out TextNode origin)
    {
        if (node.Origin is not TextNode originTextNode)
        {
            origin = null!;
            return false;
        }
        
        origin = originTextNode;
        return originTextNode.IsDeleted == node.IsDeleted 
               && originTextNode.LastId.IsContinuesTo(node.Id) 
               && originTextNode.Marks.SequenceEqual(node.Marks, new Mark.DefaultMarkComparer());
    }
    
    public static bool CanMergeWithRightOrigin(TextNode node, out TextNode rightOrigin)
    {
        if (node.RightOrigin is not TextNode rightOriginTextNode)
        {
            rightOrigin = null!;
            return false;
        }
        
        rightOrigin = rightOriginTextNode;
        return rightOriginTextNode.IsDeleted == node.IsDeleted 
               && node.LastId.IsContinuesTo(rightOriginTextNode.Id)
               && rightOriginTextNode.Marks.SequenceEqual(node.Marks, new Mark.DefaultMarkComparer());
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