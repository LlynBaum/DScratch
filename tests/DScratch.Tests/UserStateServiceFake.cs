using DScratch.Interactions.UserStates;
using DScratch.Marks;
using DScratch.Nodes;

namespace DScratch.Tests;

public class UserStateServiceFake : IUserStateService
{
    public readonly Dictionary<MarkKey, string> AddedMarks = [];

    public IReadOnlyDictionary<MarkKey, string> ActiveMarks { get; } = new Dictionary<MarkKey, string>();

    public IReadOnlyDictionary<MarkKey, string> PendingMarks { get; } = new Dictionary<MarkKey, string>();
    
    public event Action? OnStateChange;

    public void AddPendingMark(MarkKey key, string value)
    {
        AddedMarks[key] = value;
    }

    public bool CheckMark(MarkKey key, out string? value)
    {
        return AddedMarks.TryGetValue(key, out value);
    }

    IReadOnlyDictionary<MarkKey, string> IUserStateService.PopPending()
    {
        throw new NotImplementedException();
    }

    public IReadOnlyDictionary<MarkKey, string> PopPending()
    {
        throw new NotImplementedException();
    }

    public void UpdateState(DNode? node)
    {
        OnStateChange?.Invoke();
    }
}