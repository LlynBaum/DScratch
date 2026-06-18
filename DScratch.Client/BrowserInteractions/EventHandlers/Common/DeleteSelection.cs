using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers.Common;

public static class DeleteSelection
{
    public static DNodeSearchResult Handle(KeyPressInfo keyPressInfo, ITransaction transaction)
    {
        var result = SearchSelectedNodes(keyPressInfo, transaction);
        return result.Origin.Node?.Parent == result.RightOrigin.Node?.Parent 
            ? DeleteContentInParent(result, transaction) 
            : DeleteAndMerge(keyPressInfo, transaction);
    }
    
    private static NodeSearchResult<TextNode> SearchSelectedNodes(KeyPressInfo keyPressInfo, ITransaction transaction)
    {
        var (originOffset, rightOriginOffset) = keyPressInfo.Selection.GetConvertedOffsets();
        var (originId, rightOriginId) = keyPressInfo.Selection.GetConvertedNodeIds();
        var origin = transaction.FindNode(originId);
        var rightOrigin = transaction.FindNode(rightOriginId);

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
            ? transaction.SplitText(nodeSearchResult.Origin.Node!, nodeSearchResult.Origin.AbsolutOffset) 
            : null;

        var rightOrigin = nodeSearchResult.RightOrigin.Node;
        var relativeRightOriginOffset = nodeSearchResult.RightOrigin.AbsolutOffset;
        
        if (nodeSearchResult.Origin.HasFoundNode 
            && nodeSearchResult.RightOrigin.HasFoundNode
            && nodeSearchResult.Origin.Node!.Id == nodeSearchResult.RightOrigin.Node!.Id)
        {
            rightOrigin = deleteStart;
            relativeRightOriginOffset -= nodeSearchResult.Origin.Node.Length;
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
            Origin: new DNodeInfo(nodeSearchResult.Origin.Node, nodeSearchResult.Origin.AbsolutOffset), 
            RightOrigin: new DNodeInfo(rightOrigin?.RightOrigin, relativeRightOriginOffset));
    }

    private static DNodeSearchResult DeleteAndMerge(KeyPressInfo keyPressInfo, ITransaction transaction)
    {
        var (startOffset, endOffset) = keyPressInfo.Selection.GetConvertedOffsets();
        var (startNodeId, endNodeId) = keyPressInfo.Selection.GetConvertedNodeIds();
        
        var deleteStart = transaction.FindNode(startNodeId);
        if (deleteStart is null)
        {
            throw new ArgumentException($"Node not found: {startNodeId}");
        }

        if (deleteStart is not TextNode startText)
        {
            throw new ArgumentException($"Expected TextNode: {startNodeId}");
        }

        deleteStart = transaction.SplitText(startText, startOffset);
        
        var deleteEnd = transaction.FindNode(endNodeId);
        if (deleteEnd is null)
        {
            throw new ArgumentException($"Node not found: {endNodeId}");
        }
        
        if (deleteEnd is not TextNode endText)
        {
            throw new ArgumentException($"Expected TextNode: {startNodeId}");
        }
        
        transaction.SplitText(endText, endOffset);

        transaction.DeleteRange(deleteStart, null);
        transaction.DeleteRange(null, deleteEnd);
        
        transaction.MoveRange(deleteEnd.RightOrigin, null, deleteStart?.Parent!, deleteStart?.Parent?.LastChild);
        transaction.DeleteRange(deleteStart?.Parent?.RightOrigin, deleteEnd.Parent);
        
        return new DNodeSearchResult(
            Origin: new DNodeInfo(deleteStart?.Origin, startOffset), 
            RightOrigin: new DNodeInfo(deleteEnd.RightOrigin, endOffset));
    }
}