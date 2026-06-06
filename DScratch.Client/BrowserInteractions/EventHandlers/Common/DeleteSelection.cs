using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers.Common;

public static class DeleteSelection
{
    public static NodeSearchResult Handle(KeyPressInfo keyPressInfo, ITransaction transaction, DNode parent)
    {
        return keyPressInfo.Selection.AnchorId == keyPressInfo.Selection.FocusId 
            ? DeleteContentInParent(keyPressInfo, transaction, parent) 
            : DeleteAndMerge(keyPressInfo, transaction);
    }

    private static NodeSearchResult DeleteContentInParent(KeyPressInfo keyPressInfo, ITransaction transaction, DNode parent)
    {
        var (originOffset, rightOriginOffset) = keyPressInfo.Selection.GetConvertedOffsets();
        
        var walker = new TreeWalker<TextNode>(parent);
        var currentOffset = 0;
        var currentNode = walker.FirstChild();
        while (currentNode is not null)
        {
            var length = currentNode.Length;
            if (currentOffset + length >= originOffset)
            {
                break;
            }

            currentOffset += length;
            currentNode = walker.NextSibling();
        }

        var relativeOriginOffset = originOffset - currentOffset;
        var origin = currentNode;
        
        while (currentNode is not null)
        {
            var length = currentNode.Length;
            if (currentOffset + length >= rightOriginOffset)
            {
                break;
            }

            currentOffset += length;
            currentNode = walker.NextSibling();
        }

        var relativeRightOriginOffset = rightOriginOffset - currentOffset;
        var rightOrigin = currentNode;

        var deleteStart = origin is not null ? transaction.SplitText(origin, relativeOriginOffset) : null;

        if (origin is not null && rightOrigin is not null && origin.Id == rightOrigin.Id)
        {
            rightOrigin = deleteStart;
            relativeRightOriginOffset -= origin.Length;
        }
        
        if (rightOrigin != null)
        {
            transaction.SplitText(rightOrigin, relativeRightOriginOffset);
        }
        
        transaction.DeleteRange(deleteStart ?? origin?.RightOrigin, rightOrigin);
        return new NodeSearchResult(
            Origin: new NodeInfo(origin, originOffset), 
            RightOrigin: new NodeInfo(rightOrigin?.RightOrigin, rightOriginOffset));
    }

    private static NodeSearchResult DeleteAndMerge(KeyPressInfo keyPressInfo, ITransaction transaction)
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
        
        return new NodeSearchResult(
            Origin: new NodeInfo(deleteStart.Origin.Node, firstParentOffset), 
            RightOrigin: new NodeInfo(deleteEnd.RightOrigin.Node, secondParentOffset));
    }

    private static NodeSearchResult GetNode(DNode parent, int offset, ITransaction transaction)
    {
        var walker = new TreeWalker<TextNode>(parent);
        
        var currentOffset = 0;
        var currentNode = walker.FirstChild();
        while (currentNode is not null)
        {
            var length = currentNode.Length;
            if (currentOffset + length >= offset)
            {
                break;
            }

            currentOffset += length;
            currentNode = walker.NextSibling();
        }
        
        var relativeOffset = offset - currentOffset;
        var origin = currentNode is not null ? transaction.SplitText(currentNode, relativeOffset) : null;
        
        var originInfo = new NodeInfo(currentNode, offset);
        var rightOriginInfo = new NodeInfo(origin ?? currentNode?.RightOrigin, offset);
        return new NodeSearchResult(originInfo, rightOriginInfo);
    }
}