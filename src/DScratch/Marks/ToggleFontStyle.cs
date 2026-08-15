using DScratch.Interactions.UserStates;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Marks;

public class ToggleFontStyle : IMarkCommand
{
    public void Execute(ITransaction transaction, DNode anchor, IEnumerable<DNode> nodes)
    {
        var value = anchor.GetComputedMarks().GetValueOrDefault(MarkKey.FontStyle);
        SetMark(value is "italic" ? "normal" : "italic");
        return;
        
        void SetMark(string fontStyle)
        {
            foreach (var node in nodes)
            {
                transaction.UpdateMark(node, MarkKey.FontStyle, fontStyle);
            } 
        }
    }

    public void AddPending(IUserStateService userStateService)
    {
        userStateService.CheckMark(MarkKey.FontStyle, out var value);
        userStateService.AddPendingMark(MarkKey.FontStyle, value is "italic" ? "normal" : "italic");
    }
}