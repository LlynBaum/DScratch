using DScratch.Interactions.EventHandlers.Common;
using DScratch.Marks;
using DScratch.Transactions;

namespace DScratch.Interactions.EventHandlers.Events;

internal class InsertParagraphHandler(IDScratchService dScratchService) : EventWithSelectionBase(dScratchService)
{
    public const string EventName = "insertParagraph";

    protected override void HandleEvent(KeyPressInfo keyPressInfo, ITransaction transaction)
    {
        var currentParent = Context.GetParent();
        var marks = GetMarksForParagraph();
        
        // Cursor at the very start of the paragraph
        if (!Context.HasAnchor)
        {
            var previousSibling = currentParent.PreviousSibling();
            var newPreviousParagraph = transaction.NodeFactory.Paragraph(previousSibling, currentParent, marks);
            transaction.Insert(newPreviousParagraph, currentParent.Parent!);
            transaction.AddCursorPosition(currentParent.Id, 0);
            return;
        }

        var nextSibling = currentParent.NextSibling();
        var newNextParagraph = transaction.NodeFactory.Paragraph(currentParent, nextSibling, marks);
        transaction.Insert(newNextParagraph, currentParent.Parent!);
        transaction.MoveRange(Context.RightAnchorNode, null, newNextParagraph, null);
        transaction.AddCursorPosition(newNextParagraph.Id, 0);
    }
    
    private IReadOnlyDictionary<MarkKey, string> GetMarksForParagraph()
    {
        var referenceNode = Context.RightAnchorNode ?? Context.AnchorNode ?? Context.GetParent();
        return referenceNode.GetComputedMarks();
    }
}