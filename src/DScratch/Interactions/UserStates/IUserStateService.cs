using DScratch.Marks;
using DScratch.Nodes;

namespace DScratch.Interactions.UserStates;

public interface IUserStateService
{
    IReadOnlyDictionary<MarkKey, string> ActiveMarks { get; }
    IReadOnlyDictionary<MarkKey, string> PendingMarks { get; }

    event Action OnStateChange;
    
    void AddPendingMark(MarkKey key, string value);

    bool CheckMark(MarkKey key, out string? value);

    IReadOnlyDictionary<MarkKey, string> PopPending();

    void UpdateState(DNode? node);
}