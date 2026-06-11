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
        
        var parent = transaction.FindNode(keyPressInfo.Selection.AnchorNodeId);
        if (parent is null)
        {
            throw new ArgumentException($"Parent with given path not found: {keyPressInfo.Selection.AnchorId}");
        }

        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            var nodeInfo = SimpleDeleteBackwards(keyPressInfo, transaction, parent);
            
            if (!nodeInfo.HasFound && parent is ParagraphNode && parent.OriginElement is ParagraphNode paragraphNode) // TODO: probably just BLockElements in general
            {
                transaction.AddCursorPosition(parent.OriginElement.Id, paragraphNode.GetTextLength());

                transaction.MoveRange(parent.FirstChild, null, parent.OriginElement, parent.OriginElement.LastChild);
                transaction.Delete(parent);
            }
            else if (nodeInfo.HasFound)
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
        if (keyPressInfo.Selection.AnchorOffset is 0)
        {
            return NodeOffset.NotFound();
        }
        
        var walker = new TreeWalker<TextNode>(parent);

        var offset = 0;
        var current = walker.NextNode();
        while (current is not null && offset + current.Length < keyPressInfo.Selection.AnchorOffset)
        {
            offset += current.Length;
            current = walker.NextNode();
        }
        
        if (current is null)
        {
            return NodeOffset.NotFound();
        }
        
        var relativeOffset = keyPressInfo.Selection.AnchorOffset - offset;
        transaction.SplitText(current, relativeOffset);

        TextNode? previous = null;
        var index = current.Length - 1;
        
        while (index >= 0 && current is not null && char.IsWhiteSpace(current.TextContent[index]))
        {
            if (index == 0)
            {
                transaction.Delete(current);
                previous = current;
                current = walker.MovePrevious();
                index = current?.Length ?? 0;
            }
            index--;
        }

        while (index >= 0 && current is not null && !char.IsWhiteSpace(current.TextContent[index]))
        {
            if (index == 0)
            {
                transaction.Delete(current);
                previous = current;
                current = walker.MovePrevious();
                index = current?.Length ?? 0;
            }
            index--;
        }

        var word = current is not null ? transaction.SplitText(current, index + 1) : null;
        if (word is not null) transaction.Delete(word);

        return NodeOffset.From(current ?? previous, current?.Length ?? 0);
    }
}