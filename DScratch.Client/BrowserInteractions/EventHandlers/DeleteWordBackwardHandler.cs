using DScratch.Client.BrowserInteractions.EventHandlers.Common;
using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers;

public class DeleteWordBackwardHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "deleteWordBackward";
    
    public TransactionResult Handle(KeyPressInfo keyPressInfo)
    {
        var transaction = dScratchService.StartTransaction();
        
        var parent = transaction.FindNode(keyPressInfo.GetNodePath());
        if (parent is null)
        {
            throw new ArgumentException($"Parent with given path not found: {keyPressInfo.GetNodePath()}");
        }

        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            var nodeInfo = SimpleDeleteBackwards(keyPressInfo, transaction, parent);
            
            if (!nodeInfo.HasFoundNode && parent is ParagraphNode && parent.OriginElement is ParagraphNode paragraphNode) // TODO: probably just BLockElements in general
            {
                transaction.AddCursorPosition(parent.OriginElement.Id, paragraphNode.GetTextLength());

                transaction.MoveRange(parent.FirstChild, null, parent.OriginElement, parent.OriginElement.LastChild);
                transaction.Delete(parent);
            }
            else // TODO: add a case for inline elements, that will have the same without the merging
            {
                transaction.AddCursorPosition(parent.Id, nodeInfo.AbsoluteOffsetIfPresent ?? 0);
            }
        }
        else
        {
            var nodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction, parent);
            var cursorPosition = nodeSearchResult.Origin.AbsoluteOffsetIfPresent;
            var cursorTarget = nodeSearchResult.Origin.Node?.ParentElement ?? parent;
            if(cursorPosition is not null) transaction.AddCursorPosition(cursorTarget.Id, cursorPosition.Value);
        }

        return dScratchService.Apply(transaction);
    }

    private static NodeInfo SimpleDeleteBackwards(KeyPressInfo keyPressInfo, ITransaction transaction, DNode parent)
    {
        var walker = new TreeWalker<CharNode>(parent);

        var offset = 0;
        CharNode? current = null;
        while (offset < keyPressInfo.Selection.AnchorOffset)
        {
            offset++;
            current = walker.NextNode();
        }
        
        while (current is not null && current.IsWhiteSpace())
        {
            offset--;
            transaction.Delete(current);
            current = walker.MovePrevious();
        }
        
        while (current is not null && !current.IsWhiteSpace())
        {
            offset--;
            transaction.Delete(current);
            current = walker.MovePrevious();
        }

        return new NodeInfo(current, offset, current?.Parent?.IndexOf(current) ?? -1);
    }
}