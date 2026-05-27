using DScratch.Nodes;
using DScratch.Transactions.Steps;

namespace DScratch.Transactions;

internal class DTransaction(DScratchDocument document, INodeIdGenerator nodeIdGenerator) : ITransaction
{
    private readonly List<IStep> steps = [];

    public IReadOnlyList<IStep> Steps => steps;

    public DNode Root => document.Root;

    public TransactionResult Commit()
    {
        return new TransactionResult(steps.SelectMany(s => s.Execute()).ToList());
    }
    
    public ITransaction Insert(DNode node, DNode parent)
    {
        steps.Add(new InsertStep(node, parent));
        return this;
    }
    
    public ITransaction Delete(DNode node)
    {
        steps.Add(new DeleteStep(node));
        return this;
    }
    
    public ITransaction DeleteRange(DNode? start, DNode? end)
    {
        steps.Add(new DeleteRangeStep(start, end));
        return this;
    }
    
    public ITransaction MoveRange(DNode? start, DNode? end, DNode targetParent, DNode? targetOrigin)
    {
        steps.Add(new MoveRangeStep(start, end, targetParent, targetOrigin));
        return this;
    }

    public DNode? FindNode(NodePath path) => document.FindNode(path);

    public TextNode? SplitText(TextNode node, int offset)
    {
        return node.Split(offset, nodeIdGenerator.GetNextId());
    }
}