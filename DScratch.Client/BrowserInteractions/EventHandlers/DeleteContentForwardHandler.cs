using DScratch.Client.BrowserInteractions.EventHandlers.Common;
using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers;

public class DeleteContentForwardHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "deleteContentForward";
    
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
            var deletedNodeInfo = SimpleDeleteForward(keyPressInfo, transaction, parent);
            
            // TODO: for some reason, it deletes the first char in the next para first
            if (!deletedNodeInfo.HasFoundNode && parent is ParagraphNode paragraphNode && parent.RightOriginElement is ParagraphNode)
            {
                cursorTarget = parent.RightOriginElement;
                cursorPosition = paragraphNode.GetTextLength();
                
                transaction.MoveRange(parent.FirstChild, null, parent.RightOriginElement, null);
                transaction.Delete(parent);
            }
            else
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
    
    private static NodeInfo SimpleDeleteForward(KeyPressInfo keyPressInfo, ITransaction transaction, DNode parent)
    {
        var walker = new TreeWalker<TextNode>(parent);
        
        var currentOffset = 0;
        var current = walker.NextNode();
        while (current is not null)
        {
            var length = current.Length;
            if (currentOffset + length > keyPressInfo.Selection.Offset)
            {
                break;
            }

            currentOffset += length;
            current = walker.NextNode();
        }

        var relativeOffset = keyPressInfo.Selection.Offset - currentOffset;
        var nodeToDelete = current?.ChildAt(relativeOffset);
        if (nodeToDelete is not null)
        {
            transaction.Delete(nodeToDelete);
        }

        return new NodeInfo(nodeToDelete, keyPressInfo.Selection.Offset, relativeOffset);
    }
}