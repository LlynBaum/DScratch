using System.Collections.Frozen;
using DScratch.Nodes;
using DScratch.Nodes.Marks;

namespace DScratch.Interactions.UserStates;

public class UserStateService : IUserStateService
{
    private DNode? selectedNode;
    private readonly Dictionary<MarkKey, string> pendingMarks = new Dictionary<MarkKey, string>();
    private readonly HashSet<MarkKey> pendingMarkRemovals = new HashSet<MarkKey>();

    public IReadOnlyDictionary<MarkKey, string> ActiveMarks => selectedNode?.Marks 
                                                               ?? FrozenDictionary<MarkKey, string>.Empty;
    
    public IReadOnlyDictionary<MarkKey, string> PendingMarks => pendingMarks;
    
    public IReadOnlySet<MarkKey> PendingMarkRemovals => pendingMarkRemovals;
    
    public event Action? OnStateChange;

    public void AddPendingMark(MarkKey key, string value)
    {
        pendingMarks[key] = value;
    }

    public void RemovePendingMark(MarkKey key)
    {
        if (!pendingMarks.Remove(key))
        {
            pendingMarkRemovals.Add(key);
        }
    }

    public bool CheckMark(MarkKey key, out string? value)
    {
        if (pendingMarkRemovals.Contains(key))
        {
            value = null;
            return false;
        }

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

    public IReadOnlySet<MarkKey> PopPendingRemovals()
    {
        var result = pendingMarkRemovals.ToHashSet();
        pendingMarkRemovals.Clear();
        return result;
    }

    public void UpdateState(DNode? node)
    {
        pendingMarks.Clear();
        pendingMarkRemovals.Clear();
        selectedNode = node;
        OnStateChange?.Invoke();
    }
}