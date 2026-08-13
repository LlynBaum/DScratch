using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Common;

public static class DeleteSelection
{
    public static DNodeSearchResult Handle(KeyPressInfo keyPressInfo, ITransaction transaction)
    {
        var result = SearchSelectedNodes(keyPressInfo, transaction);
        return result.Origin.Node?.Parent == result.RightOrigin.Node?.Parent 
            ? DeleteContentInParent(result, transaction) 
            : DeleteAndMerge(result, transaction);
    }
    
    private static NodeSearchResult<TextNode> SearchSelectedNodes(KeyPressInfo keyPressInfo, ITransaction transaction)
    {
        var (originOffset, rightOriginOffset) = keyPressInfo.Selection!.GetConvertedOffsets();
        var (originId, rightOriginId) = keyPressInfo.Selection.GetConvertedNodeIds();
        var origin = transaction.Document.FindNode(originId);
        var rightOrigin = transaction.Document.FindNode(rightOriginId);

        if (origin is not TextNode originTextNode || rightOrigin is not TextNode rightOriginTextNode)
        {
            throw new ArgumentException($"Expected TextNodes for {originId} and {rightOriginId}");
        }

        return new NodeSearchResult<TextNode>(
            Origin: new NodeInfo<TextNode>(originTextNode, originOffset),
            RightOrigin: new NodeInfo<TextNode>(rightOriginTextNode, rightOriginOffset));
    }

    private static DNodeSearchResult DeleteContentInParent(NodeSearchResult<TextNode> nodeSearchResult, ITransaction transaction)
    {
        var deleteStart = nodeSearchResult.Origin.HasFoundNode 
            ? transaction.SplitText(nodeSearchResult.Origin.Node, nodeSearchResult.Origin.Offset) 
            : null;

        var rightOrigin = nodeSearchResult.RightOrigin.Node;
        var relativeRightOriginOffset = nodeSearchResult.RightOrigin.Offset;
        
        if (nodeSearchResult.Origin.HasFoundNode 
            && nodeSearchResult.RightOrigin.HasFoundNode
            && nodeSearchResult.Origin.Node.Id == nodeSearchResult.RightOrigin.Node.Id)
        {
            rightOrigin = deleteStart;
            relativeRightOriginOffset -= nodeSearchResult.Origin.Offset;
        }

        DNode? deleteEnd = rightOrigin;
        if (rightOrigin != null)
        {
            if (relativeRightOriginOffset > 0)
            {
                transaction.SplitText(rightOrigin, relativeRightOriginOffset);
            }
            else
            {
                deleteEnd = rightOrigin.Origin;
            }
        }
        
        transaction.DeleteRange(deleteStart ?? nodeSearchResult.Origin.Node?.RightOrigin, deleteEnd);
        return new DNodeSearchResult(
            Origin: new DNodeInfo(nodeSearchResult.Origin.Node, nodeSearchResult.Origin.Offset), 
            RightOrigin: new DNodeInfo(rightOrigin?.RightOrigin, relativeRightOriginOffset));
    }

    private static DNodeSearchResult DeleteAndMerge(NodeSearchResult<TextNode> nodeSearchResult, ITransaction transaction)
    {
        var deleteStart = nodeSearchResult.Origin.HasFoundNode 
            ? transaction.SplitText(nodeSearchResult.Origin.Node, nodeSearchResult.Origin.Offset) 
            : null;
        
        if(nodeSearchResult.RightOrigin.HasFoundNode) transaction.SplitText(nodeSearchResult.RightOrigin.Node, nodeSearchResult.RightOrigin.Offset);

        transaction.DeleteRange(deleteStart, null);
        transaction.DeleteRange(null, nodeSearchResult.RightOrigin.Node);
        
        transaction.MoveRange(nodeSearchResult.RightOrigin.Node?.RightOrigin, null, deleteStart?.Parent!, deleteStart?.Parent?.LastChild);
        transaction.DeleteRange(deleteStart?.Parent?.RightOrigin, nodeSearchResult.RightOrigin.Node?.Parent);
        
        return new DNodeSearchResult(
            Origin: new DNodeInfo(deleteStart?.Origin, nodeSearchResult.Origin.AbsoluteOffsetIfPresent ?? 0), 
            RightOrigin: new DNodeInfo(nodeSearchResult.RightOrigin.Node?.RightOrigin, nodeSearchResult.RightOrigin.AbsoluteOffsetIfPresent ?? 0));
    }
}