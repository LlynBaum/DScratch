using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers.Common;

public static class DeleteSelection
{
    public static DNodeSearchResult Handle(KeyPressInfo keyPressInfo, ITransaction transaction)
    {
        var result = SearchSelectedNodes(keyPressInfo, transaction);
        return keyPressInfo.Selection.AnchorId == keyPressInfo.Selection.FocusId 
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
        
        if (rightOrigin != null)
        {
            transaction.SplitText(rightOrigin, relativeRightOriginOffset);
        }
        
        transaction.DeleteRange(deleteStart ?? nodeSearchResult.Origin.Node?.RightOrigin, rightOrigin);
        return new DNodeSearchResult(
            Origin: new DNodeInfo(nodeSearchResult.Origin.Node, nodeSearchResult.Origin.AbsolutOffset), 
            RightOrigin: new DNodeInfo(rightOrigin?.RightOrigin, relativeRightOriginOffset));
    }

    private static DNodeSearchResult DeleteAndMerge(KeyPressInfo keyPressInfo, ITransaction transaction)
    {
        var (firstParentOffset, secondParentOffset) = keyPressInfo.Selection.GetConvertedOffsets();
        var (firstParentPath, secondParentPath) = keyPressInfo.Selection.GetConvertedNodeIds();
        
        var firstParent = transaction.FindNode(firstParentPath);
        if (firstParent is null)
        {
            throw new ArgumentException($"Parent with given path not found: {firstParentPath}");
        }
        var deleteStart = SearchOrigins(firstParent, firstParentOffset, transaction);
        
        var secondParent = transaction.FindNode(secondParentPath);
        if (secondParent is null)
        {
            throw new ArgumentException($"Parent with given path not found: {secondParentPath}");
        }
        
        var deleteEnd = SearchOrigins(secondParent, secondParentOffset, transaction);

        transaction.DeleteRange(deleteStart.RightOrigin.Node, null);
        transaction.DeleteRange(null, deleteEnd.Origin.Node);
        
        transaction.MoveRange(deleteEnd.Origin.Node?.RightOrigin, null, firstParent, firstParent.LastChild);
        transaction.DeleteRange(firstParent.RightOrigin, secondParent);
        
        return new DNodeSearchResult(
            Origin: new DNodeInfo(deleteStart.Origin.Node, firstParentOffset), 
            RightOrigin: new DNodeInfo(deleteEnd.RightOrigin.Node, secondParentOffset));
    }

    // TODO: does not take into account that InlineElements might exist
    private static DNodeSearchResult SearchOrigins(DNode parent, int offset, ITransaction transaction)
    {
        var walker = new TreeWalker<TextNode>(parent);
        
        var currentOffset = 0;
        var currentNode = walker.FirstChild();
        while (currentNode is not null)
        {
            if (currentOffset + currentNode.Length >= offset)
            {
                break;
            }

            currentOffset += currentNode.Length;
            currentNode = walker.NextSibling();
        }
        
        var relativeOffset = offset - currentOffset;
        var origin = currentNode is not null ? transaction.SplitText(currentNode, relativeOffset) : null;
        
        var originInfo = new DNodeInfo(currentNode, offset);
        var rightOriginInfo = new DNodeInfo(origin ?? currentNode?.RightOrigin, offset);
        return new DNodeSearchResult(originInfo, rightOriginInfo);
    }
}