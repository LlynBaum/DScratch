using DScratch.Interactions;
using DScratch.Marks;
using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Tests.Helpers;

public class DTransactionSpy : ITransaction, IRunningTransaction
{
    public DScratchDocument Document { get; set; } = null!;
    public DNode Root { get; set; } = null!;
    public INodeFactory NodeFactory { get; set; } = null!;

    public List<(DNode Node, DNode Parent)> InsertCalls { get; } = [];
    public List<DNode> DeleteCalls { get; } = [];
    public List<(DNode? Start, DNode? End)> DeleteRangeCalls { get; } = [];
    public List<(DNode? Start, DNode? End, DNode TargetParent, DNode? TargetOrigin)> MoveRangeCalls { get; } = [];
    public List<(DNode Node, Func<DNode, DNode> CopyFactory)> ReplaceNodeCalls { get; } = [];
    public List<(DNode Node, MarkKey Key, string Value)> AddMarkCalls { get; } = [];
    public List<(DNode Node, MarkKey Key)> RemoveMarkCalls { get; } = [];
    public List<(NodeId NodeId, int Offset)> AddCursorPositionNodeIdCalls { get; } = [];
    public List<SelectionInfo> AddCursorPositionSelectionCalls { get; } = [];
    public List<(TextNode Node, int Offset)> SplitTextCalls { get; } = [];
    public List<IReadOnlyDictionary<MarkKey, string>> CalculateMarksCalls { get; } = [];
    public List<DNode> NotifyNodeChangeCalls { get; } = [];
    public int CommitCallCount { get; private set; }

    public Func<TransactionResult>? OnCommit { get; set; }
    public Func<TextNode, int, TextNode?>? OnSplitText { get; set; }
    public Func<IReadOnlyDictionary<MarkKey, string>, IReadOnlyDictionary<MarkKey, string>>? OnCalculateMarks { get; set; }

    public DTransactionSpy(
        DScratchDocument? document = null,
        DNode? root = null,
        INodeFactory? nodeFactory = null)
    {
        if (document is not null) Document = document;
        if (root is not null) Root = root;
        if (nodeFactory is not null) NodeFactory = nodeFactory;
    }

    TransactionResult ITransaction.Commit()
    {
        CommitCallCount++;
        return OnCommit?.Invoke() ?? new TransactionResult([]);
    }

    public void Insert(DNode node, DNode parent)
    {
        InsertCalls.Add((node, parent));
    }

    public void Delete(DNode node)
    {
        DeleteCalls.Add(node);
    }

    public void DeleteRange(DNode? start, DNode? end)
    {
        DeleteRangeCalls.Add((start, end));
    }

    public void MoveRange(DNode? start, DNode? end, DNode targetParent, DNode? targetOrigin)
    {
        MoveRangeCalls.Add((start, end, targetParent, targetOrigin));
    }

    public void ReplaceNode(DNode node, Func<DNode, DNode> copyFactory)
    {
        ReplaceNodeCalls.Add((node, copyFactory));
    }

    public void AddMark(DNode node, MarkKey key, string value)
    {
        AddMarkCalls.Add((node, key, value));
    }

    public void RemoveMark(DNode node, MarkKey key)
    {
        RemoveMarkCalls.Add((node, key));
    }

    public void AddCursorPosition(NodeId nodeId, int offset)
    {
        AddCursorPositionNodeIdCalls.Add((nodeId, offset));
    }

    public void AddCursorPosition(SelectionInfo selectionInfo)
    {
        AddCursorPositionSelectionCalls.Add(selectionInfo);
    }

    public TextNode? SplitText(TextNode node, int offset)
    {
        SplitTextCalls.Add((node, offset));
        return OnSplitText?.Invoke(node, offset);
    }

    public IReadOnlyDictionary<MarkKey, string> CalculateMarks(IReadOnlyDictionary<MarkKey, string> activeMarks)
    {
        CalculateMarksCalls.Add(activeMarks);
        return OnCalculateMarks?.Invoke(activeMarks) ?? activeMarks;
    }

    public void NotifyNodeChange(DNode node)
    {
        NotifyNodeChangeCalls.Add(node);
    }
}
