using DScratch.Nodes;
using DScratch.Transactions.Steps;

namespace DScratch.Transactions;

public interface ITransaction
{
    DNode Root { get; }
    
    internal IReadOnlyList<StepDiff?> Commit();

    ITransaction Insert(DNode node, DNode parent);

    ITransaction Delete(DNode node);
    
    ITransaction DeleteRange(DNode? start, DNode? end);
    
    ITransaction MoveRange(DNode? start, DNode? end, DNode targetParent, DNode? targetOrigin);

    DNode? FindNode(NodePath path);

    TextNode? SplitText(TextNode node, int offset);
}