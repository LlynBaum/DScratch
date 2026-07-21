using DScratch.Interactions.UserStates;
using DScratch.Nodes;
using DScratch.Nodes.Marks;

namespace DScratch.Tests;

public class UserStateServiceFake : IUserStateService
{
    public readonly List<Mark> AddedMarks = [];
    public readonly List<Mark> RemovedMarks = [];

    public IReadOnlySet<Mark> ActiveMarks { get; } = new HashSet<Mark>();

    public IReadOnlySet<Mark> PendingMarks { get; } = new HashSet<Mark>();

    public IReadOnlySet<MarkKey> PendingMarkRemovals { get; } = new HashSet<MarkKey>();
    
    public event Action? OnStateChange;
    
    public void AddPendingMark(Mark mark)
    {
        AddedMarks.Add(mark);
    }

    public void RemovePendingMark(Mark mark)
    {
        RemovedMarks.Add(mark);
    }

    public bool CheckMark(MarkKey key, out string? value)
    {
        var result = AddedMarks.Except(RemovedMarks).ToHashSet().TryGetValue(new Mark(key), out var mark);
        value = mark.Value;
        return result;
    }

    public IReadOnlySet<Mark> PopPending()
    {
        throw new NotImplementedException();
    }

    public IReadOnlySet<MarkKey> PopPendingRemovals()
    {
        throw new NotImplementedException();
    }

    public void UpdateState(DNode? selectedNode)
    {
        throw new NotImplementedException();
    }
}