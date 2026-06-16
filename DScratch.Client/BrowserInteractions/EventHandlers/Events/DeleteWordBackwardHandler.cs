using DScratch.Client.BrowserInteractions.EventHandlers.Common;
using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers.Events;

public class DeleteWordBackwardHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "deleteWordBackward";
    
    public TransactionResult Handle(KeyPressInfo keyPressInfo)
    {
        var transaction = dScratchService.StartTransaction();
        
        var parent = transaction.FindNode(keyPressInfo.Selection.AnchorNodeId)?.GetNearestBlock(); // TODO: make that easier now that we have directly the text node
        if (parent is null)
        {
            throw new ArgumentException($"Parent with given path not found: {keyPressInfo.Selection.AnchorId}");
        }

        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            var nodeInfo = SimpleDeleteBackwards(keyPressInfo, transaction, parent);
            
            if (!nodeInfo.HasFound && parent is IBlockTextNode && parent.OriginElement is IBlockTextNode blockTextNode)
            {
                transaction.AddCursorPosition(parent.OriginElement.Id, blockTextNode.GetTextLength());

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
            var nodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction);
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

        var relativeOffset = SearchNode(keyPressInfo, walker);
        if (walker.Node is null)
        {
            return NodeOffset.NotFound();
        }
        
        transaction.SplitText(walker.Node, relativeOffset);

        var previousNode = DeleteWord(transaction, walker, out var remainingCharacterOffset);
        var word = walker.Node is not null ? transaction.SplitText(walker.Node, remainingCharacterOffset + 1) : null;
        if (word is not null) transaction.Delete(word);

        return NodeOffset.From(walker.Node ?? previousNode, walker.Node?.Length ?? 0);
    }

    private static TextNode? DeleteWord(ITransaction transaction, TreeWalker<TextNode> walker, out int index)
    {
        TextNode? previous = null;
        index = walker.Node!.Length - 1;
        
        while (index >= 0 && walker.Node is not null && char.IsWhiteSpace(walker.Node.TextContent[index]))
        {
            if (index == 0)
            {
                transaction.Delete(walker.Node);
                previous = walker.Node;
                walker.MovePrevious();
                index = walker.Node?.Length ?? 0;
            }
            index--;
        }

        while (index >= 0 && walker.Node is not null && !char.IsWhiteSpace(walker.Node.TextContent[index]))
        {
            if (index == 0)
            {
                transaction.Delete(walker.Node);
                previous = walker.Node;
                walker.MovePrevious();
                index = walker.Node?.Length ?? 0;
            }
            index--;
        }

        return previous;
    }

    private static int SearchNode(KeyPressInfo keyPressInfo, TreeWalker<TextNode> walker)
    {
        var offset = 0;
        var current = walker.NextNode();
        while (current is not null && offset + current.Length < keyPressInfo.Selection.AnchorOffset)
        {
            offset += current.Length;
            current = walker.NextNode();
        }

        return keyPressInfo.Selection.AnchorOffset - offset;
    }
}