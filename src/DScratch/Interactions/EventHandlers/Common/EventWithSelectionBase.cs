using DScratch.Interactions.EventHandlers.Models;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Common;

public abstract class EventWithSelectionBase : IEditorEventHandler
{
    public void Handle(KeyPressInfo keyPressInfo, ITransaction transaction)
    {
        var targetNode = transaction.FindNode(keyPressInfo.Selection.AnchorNodeId);
        if (targetNode is null)
        {
            throw new ArgumentException($"Node not found: {keyPressInfo.Selection.AnchorId}");
        }

        DNodeSearchResult nodeSearchResult;
        if (keyPressInfo.Selection.Direction is SelectionDirection.None)
        {
            if (targetNode is TextNode targetTextNode)
            {
                nodeSearchResult = HandleNoneSelection(keyPressInfo, transaction, targetTextNode);
            }
            else if (SearchTextNode(targetNode, keyPressInfo.Selection) is { } textNode)
            {
                nodeSearchResult = HandleNoneSelection(keyPressInfo, transaction, textNode);
            }
            else
            {
                HandleEmptyBlock(keyPressInfo, transaction, targetNode);
                return;
            }
        }
        else
        {
            nodeSearchResult = DeleteSelection.Handle(keyPressInfo, transaction);
            
            var cursorPosition = nodeSearchResult.Origin.AbsoluteOffsetIfPresent ?? 0;
            var cursorTarget = nodeSearchResult.Origin.Node ?? targetNode;
            transaction.AddCursorPosition(cursorTarget.Id, cursorPosition);
        }
        
        OnAfterSelection(keyPressInfo, transaction, targetNode, nodeSearchResult);
    }

    private static TextNode? SearchTextNode(DNode targetNode, SelectionInfo selection)
    {
        var walker = new TreeWalker<TextNode>(targetNode);

        var node = walker.NextNode();
        var offset = 0;
        while (walker.Node is not null)
        {
            if (walker.Node.TextContent.Length + offset >= selection.AnchorOffset)
            {
                node = walker.Node;
                break;
            }

            offset += walker.Node.TextContent.Length;
            walker.NextNode();
        }

        return node;
    }

    protected abstract DNodeSearchResult HandleNoneSelection(KeyPressInfo keyPressInfo,
        ITransaction transaction,
        TextNode anchorTextNode);

    protected abstract void HandleEmptyBlock(
        KeyPressInfo keyPressInfo,
        ITransaction transaction,
        DNode anchorNode);

    protected virtual void OnAfterSelection(KeyPressInfo keyPressInfo,
        ITransaction transaction,
        DNode anchorNode,
        DNodeSearchResult nodeSearchResult) { }
}