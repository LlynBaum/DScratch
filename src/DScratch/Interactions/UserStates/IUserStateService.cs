using DScratch.Nodes;
using DScratch.Nodes.Marks;

namespace DScratch.Interactions.UserStates;

public interface IUserStateService
{
    IReadOnlyDictionary<MarkKey, string> ActiveMarks { get; }
    IReadOnlyDictionary<MarkKey, string> PendingMarks { get; }
    IReadOnlySet<MarkKey> PendingMarkRemovals { get; }

    event Action OnStateChange;
    
    void AddPendingMark(MarkKey key, string value);
    
    void RemovePendingMark(MarkKey key);

    bool CheckMark(MarkKey key, out string? value);

    IReadOnlyDictionary<MarkKey, string> PopPending();

    IReadOnlySet<MarkKey> PopPendingRemovals();

    void UpdateState(DNode? selectedNode);
}