using DScratch.Nodes;
using DScratch.Transactions.Steps;

namespace DScratch.Transactions;

internal class DTransaction(DScratchDocument document, INodeIdGenerator nodeIdGenerator) : ITransaction
{
    private readonly List<IStep> steps = [];

    public IReadOnlyList<IStep> Steps => steps;

    public DNode Root => document.Page.Root;

    public TransactionResult Commit()
    {
        var diffs = steps.SelectMany(s => s.Execute()).ToList();
        return new TransactionResult(diffs);
        // TODO: auto normalize tree and merge everything together, then we don't have to cheat with overriding ids and so
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

    public DNode? FindNode(NodePath path) => document.FindNode(path);

    public TextNode? SplitText(TextNode node, int offset)
    {
        return node.Split(offset, nodeIdGenerator.GetNextId());
    }
}