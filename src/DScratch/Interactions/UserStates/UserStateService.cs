using DScratch.Nodes;
using DScratch.Nodes.Marks;

namespace DScratch.Interactions.UserStates;

public class UserStateService : IUserStateService
{
    private readonly HashSet<Mark> pendingMarks = new HashSet<Mark>(new Mark.MarkTable());
    private HashSet<Mark> activeMarks = new HashSet<Mark>(new Mark.MarkTable());

    public IReadOnlySet<Mark> PendingMarks => pendingMarks;
    public IReadOnlySet<Mark> ActiveMarks => activeMarks;

    public event Action? OnStateChange;

    public void AddPendingMark(Mark mark)
    {
        pendingMarks.Remove(mark);
        pendingMarks.Add(mark);
    }

    public void RemovePendingMark(Mark mark)
    {
        pendingMarks.Remove(mark);
    }

    public bool CheckMark(MarkKey key, out string? value)
    {
        var mark = new Mark(key);
        if (pendingMarks.TryGetValue(mark, out var v))
        {
            value = v.Value;
            return true;
        }
        if (activeMarks.TryGetValue(mark, out v))
        {
            value = v.Value;
            return true;
        }

        value = null;
        return false;
    }

    public IReadOnlySet<Mark> PopPending()      
    {                                       
        var result = pendingMarks.ToHashSet();
        pendingMarks.Clear();
        return result;
    }

    public void UpdateState(DNode? selectedNode)
    {
        pendingMarks.Clear();
        activeMarks = selectedNode is TextNode textNode ? textNode.Marks.ToHashSet() : [];
        OnStateChange?.Invoke();
    }
}