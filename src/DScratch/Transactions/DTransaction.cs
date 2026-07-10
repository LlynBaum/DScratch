using DScratch.Interactions;
using DScratch.Nodes;
using DScratch.Transactions.Steps;

namespace DScratch.Transactions;

internal class DTransaction(DScratchDocument document, INodeFactory nodeFactory, INodeIdGenerator nodeIdGenerator, bool disableCleanUp) 
    : ITransaction, IRunningTransaction
{
    private readonly List<IStep> steps = [];

    private readonly HashSet<ModifiedNode> modifiedNodes = [];
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

        var result = new TransactionResult(modifiedNodes.ToHashSet(), cursorPosition);
        
        modifiedNodes.Clear();
        addedNodes.Clear();
        
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
            modifiedNodes.Add(new ModifiedNode(splitNode, Modification.Insert));
        }

        return splitNode;
    }

    public void NotifyNodeChange(ModifiedNode modifiedNode) => modifiedNodes.Add(modifiedNode);

    private void CleanupCode(IReadOnlySet<ModifiedNode> modified)
    {
        if(disableCleanUp)
        {
            return;
        }

        // ToList is required here, it creates a copy of the original HashSet so we can safely add/remove items from the HashSet
        foreach (var modifiedNode in modified.Where(m => m.Node is TextNode).ToList())
        {
            var textNode = (TextNode)modifiedNode.Node;

            if (modifiedNode.Node.Origin is TextNode originTextNode && originTextNode.IsDeleted == textNode.IsDeleted && originTextNode.LastId.IsContinuesTo(textNode.Id))
            {
                cursorPosition = AdjustSelection(cursorPosition, textNode, originTextNode);

                originTextNode.AddText(textNode.TextContent);
                textNode.Remove();
                document.RemoveNode(textNode);

                // Modification must be deleted, so we just leave it, since merging here does not affect rendering in any way.
                if (!modifiedNode.Node.IsDeleted)
                {
                    modifiedNodes.Remove(modifiedNode);
                    if (modifiedNodes.All(m => m.Node != originTextNode))
                    {
                        modifiedNodes.Add(new ModifiedNode(originTextNode, Modification.Changed));
                    }
                }
            }
            else if (textNode.RightOrigin is TextNode rightOriginTextNode && rightOriginTextNode.IsDeleted == textNode.IsDeleted && textNode.LastId.IsContinuesTo(rightOriginTextNode.Id))
            {
                cursorPosition = AdjustSelection(cursorPosition, rightOriginTextNode, textNode);

                textNode.AddText(rightOriginTextNode.TextContent);
                rightOriginTextNode.Remove();
                document.RemoveNode(rightOriginTextNode);

                // Modification must be deleted, so we just leave it, since merging here does not affect rendering in any way.
                if (!modifiedNode.Node.IsDeleted)
                {
                    var existing = modifiedNodes.FirstOrDefault(m => m.Node == rightOriginTextNode);
                    if (existing is not null)
                    {
                        modifiedNodes.Remove(existing);
                    }
                }
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