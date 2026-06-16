using DScratch.Client.BrowserInteractions.EventHandlers.Common;
using DScratch.Client.BrowserInteractions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Nodes.NodeTypes;
using DScratch.Transactions;

namespace DScratch.Client.BrowserInteractions.EventHandlers.Events;

public class DeleteWordForwardHandler(IDScratchService dScratchService) : IEditorEventHandler
{
    public const string EventName = "deleteWordForward";
    
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
            
            if (!nodeInfo.HasFound && parent is IBlockTextNode blockTextNode && parent.RightOriginElement is IBlockTextNode)
            {
                transaction.AddCursorPosition(parent.RightOriginElement.Id, blockTextNode.GetTextLength());
                
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
            var nodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction);
            var cursorPosition = nodeSearchResult.Origin.AbsoluteOffsetIfPresent;
            var cursorTarget = nodeSearchResult.Origin.Node?.ParentElement ?? parent;
            if (cursorPosition is not null) transaction.AddCursorPosition(cursorTarget.Id, cursorPosition.Value);
        }

        return dScratchService.Apply(transaction);
    }
    
    private static NodeOffset SimpleDeleteBackwards(KeyPressInfo keyPressInfo, ITransaction transaction, DNode parent)
    {
        var walker = new TreeWalker<TextNode>(parent);
        var relativeOffset = SearchNode(keyPressInfo, walker);
        
        if (walker.Node is null)
        {
            return NodeOffset.NotFound();
        }
        
        var rightNode = transaction.SplitText(walker.Node, relativeOffset);
        if (rightNode is null || relativeOffset > 0)
        {
            walker.NextNode();
        }
        
        var result = NodeOffset.From(walker.Node, keyPressInfo.Selection.AnchorOffset);
        
        var remainingCharacterOffset = DeleteWord(transaction, walker);
        if (walker.Node is not null)
        {
            transaction.SplitText(walker.Node, remainingCharacterOffset);
            transaction.Delete(walker.Node);
        }
        
        return result;
    }

    private static int DeleteWord(ITransaction transaction, TreeWalker<TextNode> walker)
    {
        var characterOffset = 0;
        while (walker.Node is not null && characterOffset < walker.Node.Length && char.IsWhiteSpace(walker.Node.TextContent[characterOffset]))
        {
            if (characterOffset == walker.Node.Length - 1)
            {
                transaction.Delete(walker.Node);
                walker.NextNode();
                characterOffset = 0;
            }
            characterOffset++;
        }

        while (walker.Node is not null && characterOffset < walker.Node.Length && !char.IsWhiteSpace(walker.Node.TextContent[characterOffset]))
        {
            if (characterOffset == walker.Node.Length - 1)
            {
                transaction.Delete(walker.Node);
                walker.NextNode();
                characterOffset = 0;
            }
            characterOffset++;
        }

        return characterOffset;
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