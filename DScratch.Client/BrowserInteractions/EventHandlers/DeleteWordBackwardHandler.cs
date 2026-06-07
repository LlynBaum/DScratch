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
        var walker = new TreeWalker<TextNode>(parent);

        var offset = 0;
        var current = walker.NextNode();
        while (current?.Length < keyPressInfo.Selection.AnchorOffset)
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
        
        var index = current.Length;
        while (index > 0 && char.IsWhiteSpace(current.TextContent[index--])) { }
        while (index > 0 && !char.IsWhiteSpace(current.TextContent[index--])) { }

        var word = transaction.SplitText(current, index);
        if (word is not null) transaction.Delete(word);

        return NodeOffset.From(current, current.Length);
    }
}