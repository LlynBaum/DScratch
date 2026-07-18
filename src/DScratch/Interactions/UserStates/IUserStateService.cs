using DScratch.Nodes;
using DScratch.Nodes.Marks;

namespace DScratch.Interactions.UserStates;

public interface IUserStateService
{
    IReadOnlySet<Mark> PendingMarks { get; }
    IReadOnlySet<Mark> ActiveMarks { get; }

    event Action OnStateChange;
    
    void AddPendingMark(Mark mark);
    
    void RemovePendingMark(Mark mark);

    bool CheckMark(MarkKey key, out string? value);

    IReadOnlySet<Mark> PopPending();

    void UpdateState(DNode? selectedNode);
}