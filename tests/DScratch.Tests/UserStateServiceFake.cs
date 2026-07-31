using DScratch.Interactions.UserStates;
using DScratch.Nodes;
using DScratch.Nodes.Marks;

namespace DScratch.Tests;

public class UserStateServiceFake : IUserStateService
{
    public readonly Dictionary<MarkKey, string> AddedMarks = [];
    public readonly HashSet<MarkKey> RemovedMarks = [];

    public IReadOnlyDictionary<MarkKey, string> ActiveMarks { get; } = new Dictionary<MarkKey, string>();

    public IReadOnlyDictionary<MarkKey, string> PendingMarks { get; } = new Dictionary<MarkKey, string>();

    public IReadOnlySet<MarkKey> PendingMarkRemovals { get; } = new HashSet<MarkKey>();
    
    public event Action? OnStateChange;

    public void AddPendingMark(MarkKey key, string value)
    {
        AddedMarks[key] = value;
    }

    public void RemovePendingMark(MarkKey key)
    {
        RemovedMarks.Add(key);
    }

    public bool CheckMark(MarkKey key, out string? value)
    {
        return AddedMarks.ExceptBy(RemovedMarks, m => m.Key)
            .ToDictionary()
            .TryGetValue(key, out value);
    }

    IReadOnlyDictionary<MarkKey, string> IUserStateService.PopPending()
    {
        throw new NotImplementedException();
    }

    public IReadOnlyDictionary<MarkKey, string> PopPending()
    {
        throw new NotImplementedException();
    }

    public IReadOnlySet<MarkKey> PopPendingRemovals()
    {
        throw new NotImplementedException();
    }

    public void UpdateState(DNode? node)
    {
        throw new NotImplementedException();
    }
}