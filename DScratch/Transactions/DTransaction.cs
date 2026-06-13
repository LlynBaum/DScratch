using DScratch.Nodes;
using DScratch.Transactions.Steps;

namespace DScratch.Transactions;

internal class DTransaction(DScratchDocument document, INodeFactory nodeFactory, INodeIdGenerator nodeIdGenerator, bool disableCleanUp) 
    : ITransaction, IRunningTransaction
{
    private readonly List<IStep> steps = [];
    
    private readonly List<DNode> modifiedNodes = [];
    private readonly List<DNode> addedNodes = [];
    private CursorPosition? cursorPosition;

    public IReadOnlyList<IStep> Steps => steps;

    public DNode Root => document.Root;

    public INodeFactory NodeFactory => nodeFactory;

    public TransactionResult Commit()
    {
        var result = new TransactionResult(steps.SelectMany(s => s.Execute(this, document)).ToList(), cursorPosition);
        
        addedNodes.ForEach(document.AddNode);
        addedNodes.Clear();

        CleanupCode(modifiedNodes);
        modifiedNodes.Clear();
        
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
    
    public ITransaction ReplaceNode(DNode node, Func<DNode, DNode> copyFactory)
    {
        steps.Add(new ReplaceNodeStep(node, copyFactory));
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

    public void NotifyNodeChange(DNode node) => modifiedNodes.Add(node);

    private void CleanupCode(IReadOnlyList<DNode> nodes)
    {
        if(disableCleanUp) return;
        
        foreach (var node in nodes.OfType<TextNode>())
        {
            if (node.Origin is TextNode originTextNode && originTextNode.IsDeleted == node.IsDeleted && originTextNode.LastId.IsContinuesTo(node.Id))
            {
                originTextNode.AddText(node.TextContent);
                node.Remove();
                document.RemoveNode(node);
            }
            else if (node.RightOrigin is TextNode rightOriginTextNode && rightOriginTextNode.IsDeleted == node.IsDeleted && node.LastId.IsContinuesTo(rightOriginTextNode.Id))
            {
                node.AddText(rightOriginTextNode.TextContent);
                rightOriginTextNode.Remove();
                document.RemoveNode(rightOriginTextNode);
            }
        }
    }
}