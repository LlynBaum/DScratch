using DScratch.Nodes;
using DScratch.Transactions.Steps;

namespace DScratch.Transactions;

internal class DTransaction(DScratchDocument document, INodeIdGenerator nodeIdGenerator) : ITransaction, IRunningTransaction
{
    private readonly List<IStep> steps = [];

    public IReadOnlyList<IStep> Steps => steps;

    public DNode Root => document.Root;

    private readonly List<DNode> changedNodes = [];
    private readonly List<DNode> addedNodes = [];
    private CursorPosition? cursorPosition;

    public TransactionResult Commit()
    {
        var result = new TransactionResult(steps.SelectMany(s => s.Execute(this)).ToList(), cursorPosition);
        
        addedNodes.ForEach(document.AddNode);
        addedNodes.Clear();

        CleanupCode(changedNodes);
        changedNodes.Clear();
        
        return result;
    }
    
    public ITransaction Insert(DNode node, DNode parent)
    {
        steps.Add(new InsertStep(node, parent));
        addedNodes.Add(node);
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

    public ITransaction AddCursorPosition(NodeId nodeId, int offset)
    {
        cursorPosition = new CursorPosition(nodeId.Value, offset);
        return this;
    }

    public DNode? FindNode(NodeId nodeId) => document.FindNode(nodeId);

    public TextNode? SplitText(TextNode node, int offset)
    {
        var splitNode = node.Split(offset, nodeIdGenerator.GetNextId());
        if (splitNode is not null)
        {
            addedNodes.Add(splitNode);
        }
        return splitNode;
    }

    public void NotifyNodeChange(DNode node)
    {
        changedNodes.Add(node);
    }

    private static void CleanupCode(IReadOnlyList<DNode> nodes)
    {
        foreach (var node in nodes)
        {
            
        }
    }
}