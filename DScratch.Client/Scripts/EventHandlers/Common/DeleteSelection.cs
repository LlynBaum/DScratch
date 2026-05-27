using DScratch.Client.Scripts.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Client.Scripts.EventHandlers.Common;

public static class DeleteSelection
{
    public static NodeSearchResult Handle(
        KeyPressInfo keyPressInfo,
        ITransaction transaction, 
        DNode parent)
    {
        // TODO: deleting over two paragraphs will be more complex. Need to merge them together in that case...
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
            Origin: NodeInfo.Create(origin, originOffset, relativeOriginOffset), 
            RightOrigin: NodeInfo.Create(rightOrigin?.RightOrigin, rightOriginOffset, relativeRightOriginOffset));
    }
}