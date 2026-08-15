using DScratch.Interactions.UserStates;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Marks;

public class ToggleFontWeight : IMarkCommand
{
    public void Execute(ITransaction transaction, DNode anchor, IEnumerable<DNode> nodes)
    {
        var value = anchor.GetComputedMarks().GetValueOrDefault(MarkKey.FontWeight);
        SetMark(value is "bold" ? "normal" : "bold");
        return;
        
        void SetMark(string fontWeight)
        {
            foreach (var node in nodes)
            {
                transaction.UpdateMark(node, MarkKey.FontWeight, fontWeight);
            } 
        }
    }
    
    public void AddPending(IUserStateService userStateService)
    {
        userStateService.CheckMark(MarkKey.FontWeight, out var value);
        userStateService.AddPendingMark(MarkKey.FontWeight, value is "bold" ? "normal" : "bold");
    }
}