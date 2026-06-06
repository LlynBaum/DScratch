using DScratch.Client.BrowserInteractions.EventHandlers.Common;
using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers;

public class DeleteContentBackwardHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "deleteContentBackward";
    
    public TransactionResult Handle(KeyPressInfo keyPressInfo)
    {
        var transaction = dScratchService.StartTransaction();
        
        var parent = transaction.FindNode(keyPressInfo.GetNodePath());
        if (parent is null)
        {
            throw new ArgumentException($"Parent with given path not found: {keyPressInfo.GetNodePath()}");
        }

        int? cursorPosition;
        DNode cursorTarget;
        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            var deletedNodeInfo = SimpleDeleteBackwards(keyPressInfo, transaction, parent);

            if (!deletedNodeInfo.HasFoundNode && parent is ParagraphNode && parent.OriginElement is ParagraphNode paragraphNode) // TODO: probably just BLockElements in general
            {
                cursorTarget = parent.OriginElement;
                cursorPosition = paragraphNode.GetTextLength();
                
                transaction.MoveRange(parent.FirstChild, null, parent.OriginElement, parent.OriginElement.LastChild);
                transaction.Delete(parent);
            }
            else // TODO: add a case for inline elements, that will have the same without the merging
            {
                cursorPosition = deletedNodeInfo.AbsoluteOffsetIfPresent;
                cursorTarget = parent;
            }
        }
        else
        {
            var nodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction, parent);
            cursorPosition = nodeSearchResult.Origin.AbsoluteOffsetIfPresent;
            cursorTarget = nodeSearchResult.Origin.Node?.ParentElement ?? parent;
        }
        
        if (cursorPosition is not null) transaction.AddCursorPosition(cursorTarget.Id, cursorPosition.Value);
        return dScratchService.Apply(transaction);
    }

    private static NodeInfo SimpleDeleteBackwards(KeyPressInfo keyPressInfo, ITransaction transaction, DNode parent)
    {
        var walker = new TreeWalker<TextNode>(parent);
        
        var currentOffset = 0;
        var current = walker.NextNode();
        while (current is not null)
        {
            var length = current.Length;
            if (currentOffset + length >= keyPressInfo.Selection.AnchorOffset)
            {
                break;
            }

            currentOffset += length;
            current = walker.NextNode();
        }

        var relativeOffset = keyPressInfo.Selection.AnchorOffset - currentOffset - 1;
        var nodeToDelete = current?.ChildAt(relativeOffset);
        if (nodeToDelete is not null)
        {
            transaction.Delete(nodeToDelete);
        }
        
        return new NodeInfo(nodeToDelete, keyPressInfo.Selection.AnchorOffset - 1, relativeOffset);
    }
}