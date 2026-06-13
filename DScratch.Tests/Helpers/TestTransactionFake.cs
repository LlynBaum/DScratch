using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Tests.Helpers;

public class TestTransactionFake : ITransaction, IRunningTransaction
{
    public List<DNode> ChangedNodes => changedNodes;

    private readonly List<DNode> changedNodes = [];
    
    public DNode Root { get; }
    
    public INodeFactory NodeFactory { get; }

    public TransactionResult Commit()
    {
        throw new NotImplementedException();
    }

    public ITransaction Insert(DNode node, DNode parent)
    {
        throw new NotImplementedException();
    }

    public ITransaction Delete(DNode node)
    {
        throw new NotImplementedException();
    }

    public ITransaction DeleteRange(DNode? start, DNode? end)
    {
        throw new NotImplementedException();
    }

    public ITransaction MoveRange(DNode? start, DNode? end, DNode targetParent, DNode? targetOrigin)
    {
        throw new NotImplementedException();
    }

    public ITransaction ReplaceNode(DNode node, Func<DNode, DNode> copyFactory)
    {
        throw new NotImplementedException();
    }

    public ITransaction AddCursorPosition(NodeId nodeId, int offset)
    {
        throw new NotImplementedException();
    }

    public DNode? FindNode(NodeId nodeId)
    {
        throw new NotImplementedException();
    }

    public TextNode? SplitText(TextNode node, int offset)
    {
        throw new NotImplementedException();
    }

    public void NotifyNodeChange(DNode node)
    {
        changedNodes.Add(node);
    }
}