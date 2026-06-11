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
            
            if (!nodeInfo.HasFound && parent is ParagraphNode paragraphNode && parent.RightOriginElement is ParagraphNode)
            {
                transaction.AddCursorPosition(parent.RightOriginElement.Id, paragraphNode.GetTextLength());
                
                transaction.MoveRange(parent.FirstChild, null, parent.RightOriginElement, null);
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
        var newTextNode = transaction.SplitText(current, relativeOffset);

        if (newTextNode is null || relativeOffset > 0)
        {
            current = walker.NextNode();
        }
        
        var result = NodeOffset.From(current, keyPressInfo.Selection.AnchorOffset);
        
        var index = 0;
        while (current is not null && index < current.Length && char.IsWhiteSpace(current.TextContent[index]))
        {
            if (index == current.Length - 1)
            {
                transaction.Delete(current);
                current = walker.NextNode();
                index = 0;
            }
            index++;
        }

        while (current is not null && index < current.Length && !char.IsWhiteSpace(current.TextContent[index]))
        {
            if (index == current.Length - 1)
            {
                transaction.Delete(current);
                current = walker.NextNode();
                index = 0;
            }
            index++;
        }
        
        if (current is not null)
        {
            transaction.SplitText(current, index);
            transaction.Delete(current);
        }
        
        return result;
    }
}