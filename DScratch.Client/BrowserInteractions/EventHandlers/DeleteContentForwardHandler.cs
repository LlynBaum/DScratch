using DScratch.Client.BrowserInteractions.EventHandlers.Common;
using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;
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
        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            // TODO: we can detect this, when simpleDelete has nothing found to delete, then we are at the end of the paragraph
            if (parent.IsParagraphNode() && keyPressInfo.Selection.Offset >= 10)
            {
                // so we are at the start of a text element... like a p element... we have to delete it, and move text over to previous element, if possible, else fuck it xD
                throw new NotImplementedException();
            }
            
            var deletedNodeInfo = SimpleDeleteForward(keyPressInfo, transaction, parent);
            cursorPosition = deletedNodeInfo.AbsoluteOffsetIfPresent;

        }
        else
        {
            var nodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction, parent);
            cursorPosition = nodeSearchResult.Origin.AbsoluteOffsetIfPresent;
        }
        
        if (cursorPosition is not null) transaction.AddCursorPosition(parent.Id, cursorPosition.Value);
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