using DScratch.Nodes;
using DScratch.Nodes.Marks;

namespace DScratch.Interactions.UserStates;

public interface IUserStateService
{
    event Action OnStateChange;
    
    IReadOnlySet<Mark> ActiveMarks { get; }
    
    void AddPendingMark(Mark mark);
    
    void RemovePendingMark(Mark mark);

    void DiscardPending();

    IReadOnlySet<Mark> PopPending();

    void UpdateState(DNode? selectedNode);
}