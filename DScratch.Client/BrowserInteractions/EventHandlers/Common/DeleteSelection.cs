using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers.Common;

public static class DeleteSelection
{
    public static DNodeSearchResult Handle(KeyPressInfo keyPressInfo, ITransaction transaction, DNode parent)
    {
        return keyPressInfo.Selection.AnchorId == keyPressInfo.Selection.FocusId 
            ? DeleteContentInParent(keyPressInfo, transaction, parent) 
            : DeleteAndMerge(keyPressInfo, transaction);
    }

    private static DNodeSearchResult DeleteContentInParent(KeyPressInfo keyPressInfo, ITransaction transaction, DNode parent)
    {
        var (originOffset, rightOriginOffset) = keyPressInfo.Selection.GetConvertedOffsets();

        var result = GetSelectedNodes(parent, originOffset, rightOriginOffset);

        var deleteStart = result.Origin.HasFoundNode 
            ? transaction.SplitText(result.Origin.Node!, result.Origin.AbsolutOffset) 
            : null;

        var rightOrigin = result.RightOrigin.Node;
        var relativeRightOriginOffset = result.RightOrigin.AbsolutOffset;
        
        if (result.Origin.HasFoundNode && result.RightOrigin.HasFoundNode && result.Origin.Node!.Id == result.RightOrigin.Node!.Id)
        {
            rightOrigin = deleteStart;
            relativeRightOriginOffset -= result.Origin.Node.Length;
        }
        
        if (rightOrigin != null)
        {
            transaction.SplitText(rightOrigin, relativeRightOriginOffset);
        }
        
        transaction.DeleteRange(deleteStart ?? result.Origin.Node?.RightOrigin, rightOrigin);
        return new DNodeSearchResult(
            Origin: new DNodeInfo(result.Origin.Node, originOffset), 
            RightOrigin: new DNodeInfo(rightOrigin?.RightOrigin, rightOriginOffset));
    }
    
    private static NodeSearchResult<TextNode> GetSelectedNodes(DNode parent, int originOffset, int rightOriginOffset)
    {
        var walker = new TreeWalker<TextNode>(parent);
        var currentOffset = 0;
        var currentNode = walker.FirstChild();
        while (currentNode is not null)
        {
            if (currentOffset + currentNode.Length >= originOffset)
            {
                break;
            }

            currentOffset += currentNode.Length;
            currentNode = walker.NextSibling();
        }

        var relativeOriginOffset = originOffset - currentOffset;
        var origin = currentNode;
        
        while (currentNode is not null)
        {
            if (currentOffset + currentNode.Length >= rightOriginOffset)
            {
                break;
            }

            currentOffset += currentNode.Length;
            currentNode = walker.NextSibling();
        }

        var relativeRightOriginOffset = rightOriginOffset - currentOffset;
        var rightOrigin = currentNode;

        return new NodeSearchResult<TextNode>(
            Origin: new NodeInfo<TextNode>(origin, relativeOriginOffset),
            RightOrigin: new NodeInfo<TextNode>(rightOrigin, relativeRightOriginOffset));
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
        var deleteStart = GetNode(firstParent, firstParentOffset, transaction);
        
        var secondParent = transaction.FindNode(secondParentPath);
        if (secondParent is null)
        {
            throw new ArgumentException($"Parent with given path not found: {secondParentPath}");
        }
        
        var deleteEnd = GetNode(secondParent, secondParentOffset, transaction);

        transaction.DeleteRange(deleteStart.RightOrigin.Node, null);
        transaction.DeleteRange(null, deleteEnd.Origin.Node);
        
        // TODO: this is only the case of a block elements... Inline elements will not merge
        // In case of inline elements, search for nearest block element. If they are not the same, merge them. Else it should only delete but not the second parent and also not merging
        transaction.MoveRange(deleteEnd.Origin.Node?.RightOrigin, null, firstParent, firstParent.LastChild);
        transaction.DeleteRange(firstParent.RightOrigin, secondParent);
        
        return new DNodeSearchResult(
            Origin: new DNodeInfo(deleteStart.Origin.Node, firstParentOffset), 
            RightOrigin: new DNodeInfo(deleteEnd.RightOrigin.Node, secondParentOffset));
    }

    private static DNodeSearchResult GetNode(DNode parent, int offset, ITransaction transaction)
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