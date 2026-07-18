using DScratch.Nodes;
using DScratch.Nodes.Marks;

namespace DScratch.Interactions.UserStates;

public class UserStateService : IUserStateService
{
    private readonly HashSet<Mark> pendingMarks = new HashSet<Mark>(new Mark.MarkTable());

    public event Action? OnStateChange;
    public IReadOnlySet<Mark> ActiveMarks { get; private set; } = new HashSet<Mark>();

    public void AddPendingMark(Mark mark)
    {
        pendingMarks.Remove(mark);
        pendingMarks.Add(mark);
    }

    public void RemovePendingMark(Mark mark)
    {
        pendingMarks.Remove(mark);
    }

    public void DiscardPending()
    {
        pendingMarks.Clear();
    }

    public IReadOnlySet<Mark> PopPending()
    {
        var result = pendingMarks.ToHashSet();
        pendingMarks.Clear();
        return result;
    }

    public void UpdateState(DNode? selectedNode)
    {
        DiscardPending();
        ActiveMarks = selectedNode is TextNode textNode ? textNode.Marks : new HashSet<Mark>();
        OnStateChange?.Invoke();
    }
}