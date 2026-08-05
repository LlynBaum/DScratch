using System.Collections.Frozen;
using DScratch.Marks;
using DScratch.Nodes;

namespace DScratch.Interactions.UserStates;

public class UserStateService : IUserStateService
{
    private DNode? selectedNode;
    private readonly Dictionary<MarkKey, string> pendingMarks = new Dictionary<MarkKey, string>();

    public IReadOnlyDictionary<MarkKey, string> ActiveMarks => selectedNode?.GetComputedMarks() 
                                                               ?? FrozenDictionary<MarkKey, string>.Empty;
    
    public IReadOnlyDictionary<MarkKey, string> PendingMarks => pendingMarks;
    
    public event Action? OnStateChange;

    public void AddPendingMark(MarkKey key, string value)
    {
        pendingMarks[key] = value;
    }

    public bool CheckMark(MarkKey key, out string? value)
    {
        if (pendingMarks.TryGetValue(key, out value))
        {
            return true;
        }
        if (ActiveMarks.TryGetValue(key, out value))
        {
            return true;
        }

        value = null;
        return false;
    }

    public IReadOnlyDictionary<MarkKey, string> PopPending()      
    {                                       
        var result = pendingMarks.ToDictionary();
        pendingMarks.Clear();
        return result;
    }

    public void UpdateState(DNode? node)
    {
        pendingMarks.Clear();
        selectedNode = node;
        OnStateChange?.Invoke();
    }
}