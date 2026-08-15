using DScratch.Interactions;
using DScratch.Marks;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Tests.Helpers;

public class TestTransactionFake : ITransaction, IRunningTransaction
{
    public IReadOnlyList<DNode> ChangedNodes => changedNodes;

    private readonly List<DNode> changedNodes = [];

    public DScratchDocument Document { get; } = null!;
    
    public DNode Root { get; } = null!;

    public INodeFactory NodeFactory { get; } = null!;

    public TransactionResult Commit()
    {
        throw new NotImplementedException();
    }

    public void Insert(DNode node, DNode parent)
    {
        throw new NotImplementedException();
    }

    public void Delete(DNode node)
    {
        throw new NotImplementedException();
    }

    public void DeleteRange(DNode? start, DNode? end)
    {
        throw new NotImplementedException();
    }

    public void MoveRange(DNode? start, DNode? end, DNode targetParent, DNode? targetOrigin)
    {
        throw new NotImplementedException();
    }

    public void ReplaceNode(DNode node, Func<DNode, DNode> copyFactory)
    {
        throw new NotImplementedException();
    }

    public void UpdateMark(DNode node, MarkKey key, string value)
    {
        throw new NotImplementedException();
    }

    public void UpdateAttributes(DNode node, Action update)
    {
        throw new NotImplementedException();
    }

    public void AddCursorPosition(NodeId nodeId, int offset)
    {
        throw new NotImplementedException();
    }

    public void AddCursorPosition(SelectionInfo selectionInfo)
    {
        throw new NotImplementedException();
    }

    public TextNode SplitText(TextNode node, int offset)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyDictionary<MarkKey, string> CalculateMarks(IReadOnlyDictionary<MarkKey, string> activeMarks)
    {
        throw new NotImplementedException();
    }

    public void NotifyNodeChange(DNode node)
    {
        changedNodes.Add(node);
    }
}