using DScratch.Interactions;
using DScratch.Nodes;
using DScratch.Transactions.Steps;

namespace DScratch.Transactions;

internal class DTransaction(DScratchDocument document, INodeFactory nodeFactory, INodeIdGenerator nodeIdGenerator, bool disableCleanUp) 
    : ITransaction, IRunningTransaction
{
    private readonly List<IStep> steps = [];

    private readonly HashSet<DNode> modifiedNodes = [];
    private readonly List<DNode> addedNodes = [];
    private SelectionInfo? cursorPosition;

    public IReadOnlyList<IStep> Steps => steps;

    public DNode Root => document.Root;

    public INodeFactory NodeFactory => nodeFactory;

    public TransactionResult Commit()
    {
        foreach (var step in steps)
        {
            step.Execute(this, document);
        }
        
        addedNodes.ForEach(document.AddNode);
        CleanupCode(modifiedNodes);
        
        modifiedNodes.Clear();
        addedNodes.Clear();

        return new TransactionResult(modifiedNodes, cursorPosition);
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
        cursorPosition = new SelectionInfo
        {
            Direction = SelectionDirection.None,
            AnchorId = nodeId.Value,
            AnchorOffset = offset,
            FocusId = nodeId.Value,
            FocusOffset = offset
        };
        return this;
    }
    
    public ITransaction AddCursorPosition(SelectionInfo selectionInfo)
    {
        cursorPosition = selectionInfo;
        return this;
    }

    public DNode? FindNode(NodeId nodeId) => document.FindNode(nodeId);

    public TextNode? SplitText(TextNode node, int offset)
    {
        var splitNode = node.Split(offset, nodeIdGenerator.GetNextId);
        
        if (splitNode is not null && splitNode.Id != node.Id)
        {
            addedNodes.Add(splitNode);
        }
        
        return splitNode;
    }

    public void NotifyNodeChange(DNode node) => modifiedNodes.Add(node);

    private void CleanupCode(IReadOnlySet<DNode> nodes)
    {
        if(disableCleanUp)
        {
            return;
        }
        
        // ToList is required here, it creates a copy of the original HashSet so we can safely add/remove items from the HashSet
        foreach (var node in nodes.OfType<TextNode>().ToList())
        {
            if (node.Origin is TextNode originTextNode && originTextNode.IsDeleted == node.IsDeleted && originTextNode.LastId.IsContinuesTo(node.Id))
            {
                cursorPosition = AdjustSelection(cursorPosition, node, originTextNode);

                originTextNode.AddText(node.TextContent);
                node.Remove();
                modifiedNodes.Remove(node);
                modifiedNodes.Add(originTextNode);
                document.RemoveNode(node);
            }
            else if (node.RightOrigin is TextNode rightOriginTextNode && rightOriginTextNode.IsDeleted == node.IsDeleted && node.LastId.IsContinuesTo(rightOriginTextNode.Id))
            {
                cursorPosition = AdjustSelection(cursorPosition, rightOriginTextNode, node);
                
                node.AddText(rightOriginTextNode.TextContent);
                rightOriginTextNode.Remove();
                modifiedNodes.Remove(rightOriginTextNode);
                modifiedNodes.Add(node);
                document.RemoveNode(rightOriginTextNode);
            }
        }
    }

    private static SelectionInfo? AdjustSelection(SelectionInfo? selectionInfo, TextNode oldNode, TextNode targetNode)
    {
        if (selectionInfo is null) return null;
        
        if (selectionInfo.AnchorId == oldNode.Id.Value && selectionInfo.FocusId == oldNode.Id.Value)
        {
            return new SelectionInfo
            {
                AnchorId = targetNode.Id.Value,
                AnchorOffset = targetNode.Length + selectionInfo.AnchorOffset,
                FocusId = targetNode.Id.Value,
                FocusOffset = targetNode.Length + selectionInfo.AnchorOffset
            };
        }
        
        if (selectionInfo.AnchorId == oldNode.Id.Value && selectionInfo.FocusId != oldNode.Id.Value)
        {
            return new SelectionInfo
            {
                AnchorId = targetNode.Id.Value,
                AnchorOffset = targetNode.Length + selectionInfo.AnchorOffset,
                FocusId = selectionInfo.FocusId,
                FocusOffset = selectionInfo.FocusOffset
            };
        }
        
        if (selectionInfo.AnchorId != oldNode.Id.Value && selectionInfo.FocusId == oldNode.Id.Value)
        {
            return new SelectionInfo
            {
                AnchorId = selectionInfo.AnchorId,
                AnchorOffset = selectionInfo.AnchorOffset,
                FocusId = targetNode.Id.Value,
                FocusOffset = targetNode.Length + selectionInfo.AnchorOffset
            };
        }

        return selectionInfo;
    }
}