using System.Collections.Frozen;
using DScratch.Interactions.EventHandlers.Common;
using DScratch.Marks;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Events;

internal class InsertTextHandler(IDScratchService dScratchService) : EventWithSelectionBase(dScratchService)
{
    public const string EventName = "insertText";

    protected override void HandleEvent(KeyPressInfo keyPressInfo, ITransaction transaction)
    {
        if (string.IsNullOrEmpty(keyPressInfo.Data))
        {
            return;
        }
        
        var marks = GetMarks(transaction);
        var textNode = transaction.NodeFactory.String(
            value: keyPressInfo.Data,
            origin: HandlerContext.AnchorNodeId,
            rightOrigin: HandlerContext.RightAnchorNodeId,
            initMarks: marks);
        transaction.Insert(textNode, HandlerContext.GetParent());
        transaction.AddCursorPosition(textNode.Id, textNode.Length);
    }

    private IReadOnlyDictionary<MarkKey, string> GetMarks(ITransaction transaction)
    {
        var referenceNode = HandlerContext.AnchorNode ?? HandlerContext.RightAnchorNode ?? HandlerContext.GetParent().FirstChild;
        return transaction.CalculateMarks(referenceNode?.Marks ?? FrozenDictionary<MarkKey, string>.Empty);
    }
}