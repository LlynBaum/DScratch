using DScratch.Client.BrowserInteractions.EventHandlers.Common;
using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers;

public class DeleteWordForwardHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "deleteWordForward";
    
    public TransactionResult Handle(KeyPressInfo keyPressInfo)
    {
        var transaction = dScratchService.StartTransaction();
        
        var parent = transaction.FindNode(keyPressInfo.Selection.AnchorNodeId);
        if (parent is null)
        {
            throw new ArgumentException($"Parent with given path not found: {keyPressInfo.Selection.AnchorId}");
        }

        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            var nodeInfo = SimpleDeleteBackwards(keyPressInfo, transaction, parent);
            
            if (!nodeInfo.HasFound && parent is ParagraphNode paragraphNode && parent.RightOriginElement is ParagraphNode) // TODO: probably just BLockElements in general
            {
                transaction.AddCursorPosition(parent.RightOriginElement.Id, paragraphNode.GetTextLength());
                
                transaction.MoveRange(parent.FirstChild, null, parent.RightOriginElement, null);
                transaction.Delete(parent);
            }
            else // TODO: add a case for inline elements, that will have the same without the merging
            {
                transaction.AddCursorPosition(parent.Id, nodeInfo.OffsetOrDefault);
            }
        }
        else
        {   
            var nodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction, parent);
            var cursorPosition = nodeSearchResult.Origin.AbsoluteOffsetIfPresent;
            var cursorTarget = nodeSearchResult.Origin.Node?.ParentElement ?? parent;
            if (cursorPosition is not null) transaction.AddCursorPosition(cursorTarget.Id, cursorPosition.Value);
        }

        return dScratchService.Apply(transaction);
    }
    
    private static NodeOffset SimpleDeleteBackwards(KeyPressInfo keyPressInfo, ITransaction transaction, DNode parent)
    {
        var walker = new TreeWalker<CharNode>(parent);

        var offset = 0;
        CharNode? current = null;
        while (offset <= keyPressInfo.Selection.AnchorOffset)
        {
            offset++;
            current = walker.NextNode();
        }

        var result = NodeOffset.From(current, keyPressInfo.Selection.AnchorOffset);
        
        while (current is not null && current.IsWhiteSpace())
        {
            transaction.Delete(current);
            current = walker.NextNode();
        }
        
        while (current is not null && !current.IsWhiteSpace())
        {
            transaction.Delete(current);
            current = walker.NextNode();
        }

        return result;
    }
}