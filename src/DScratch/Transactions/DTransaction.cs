using DScratch.Interactions;
using DScratch.Nodes;
using DScratch.Nodes.Marks;
using DScratch.Rendering;
using DScratch.Transactions.Steps;
using DScratch.Transactions.Steps.Marks;

namespace DScratch.Transactions;

internal class DTransaction(DScratchDocument document, INodeFactory nodeFactory, INodeIdGenerator nodeIdGenerator, bool disableCleanUp) 
    : ITransaction, IRunningTransaction
{
    private readonly List<IStep> steps = [];
    private readonly List<StepDiff> additionalStepDiffs = [];
    
    private readonly List<DNode> modifiedNodes = [];
    private readonly List<DNode> addedNodes = [];
    private SelectionInfo? cursorPosition;

    public IReadOnlyList<IStep> Steps => steps;

    public DNode Root => document.Root;

    public INodeFactory NodeFactory => nodeFactory;

    public TransactionResult Commit()
    {
        var stepDiffs = steps.SelectMany(s => s.Execute(this, document)).ToList();
        
        addedNodes.ForEach(document.AddNode);
        var cleanUpSteps = CleanupCode(modifiedNodes);
        stepDiffs = [..additionalStepDiffs, ..stepDiffs, ..cleanUpSteps];
        
        modifiedNodes.Clear();
        addedNodes.Clear();
        additionalStepDiffs.Clear();
        
        return new TransactionResult(stepDiffs, cursorPosition);
    }
    
    public void Insert(DNode node, DNode parent)
    {
        steps.Add(new InsertStep(node, parent));
        addedNodes.Add(node);
    }
    
    public void Delete(DNode node)
    {
        steps.Add(new DeleteStep(node));
    }
    
    public void DeleteRange(DNode? start, DNode? end)
    {
        steps.Add(new DeleteRangeStep(start, end));
    }
    
    public void MoveRange(DNode? start, DNode? end, DNode targetParent, DNode? targetOrigin)
    {
        steps.Add(new MoveRangeStep(start, end, targetParent, targetOrigin));
    }
    
    public void ReplaceNode(DNode node, Func<DNode, DNode> copyFactory)
    {
        steps.Add(new ReplaceNodeStep(node, copyFactory));
    }

    public void AddMark(TextNode node, Mark mark)
    {
        steps.Add(new AddMarkStep(node, mark));
    }

    public void RemoveMark(TextNode node, MarkKey key)
    {
        steps.Add(new RemoveMarkStep(node, key));
    }

    public void AddCursorPosition(NodeId nodeId, int offset)
    {
        cursorPosition = new SelectionInfo
        {
            Direction = SelectionDirection.None,
            AnchorId = nodeId.Value,
            AnchorOffset = offset,
            FocusId = nodeId.Value,
            FocusOffset = offset
        };
    }
    
    public void AddCursorPosition(SelectionInfo selectionInfo)
    {
        cursorPosition = selectionInfo;
    }

    public DNode? FindNode(NodeId nodeId) => document.FindNode(nodeId);

    public TextNode? SplitText(TextNode node, int offset)
    {
        var splitNode = node.Split(offset, nodeIdGenerator.GetNextId);
        
        if (splitNode is not null && splitNode.Id != node.Id)
        {
            addedNodes.Add(splitNode);
            additionalStepDiffs.Add(new StepDiff.DeleteTextDiff(node.Id.Value, offset, splitNode.Length));
            additionalStepDiffs.AddRange(splitNode.ToInsertSteps());
        }
        
        return splitNode;
    }

    public void NotifyNodeChange(DNode node) => modifiedNodes.Add(node);

    private List<StepDiff?> CleanupCode(IReadOnlyList<DNode> nodes)
    {
        List<StepDiff?> result = [];
        if(disableCleanUp)
        {
            return result;
        }
        
        foreach (var node in nodes.OfType<TextNode>())
        {
            if (node.Origin is TextNode originTextNode && originTextNode.IsDeleted == node.IsDeleted && originTextNode.LastId.IsContinuesTo(node.Id))
            {
                if (!originTextNode.IsDeleted)
                {
                    var textInsert = new StepDiff.InsertTextDiff(
                        originTextNode.Id.Value,
                        originTextNode.Length,
                        node.TextContent);
                
                    result.Add(textInsert);
                    result.Add(node.ToDeleteSteps());
                }

                cursorPosition = AdjustSelection(cursorPosition, node, originTextNode);

                originTextNode.AddText(node.TextContent);
                node.Remove();
                document.RemoveNode(node);
            }
            else if (node.RightOrigin is TextNode rightOriginTextNode && rightOriginTextNode.IsDeleted == node.IsDeleted && node.LastId.IsContinuesTo(rightOriginTextNode.Id))
            {
                if (!rightOriginTextNode.IsDeleted)
                {
                    var textInsert = new StepDiff.InsertTextDiff(
                        node.Id.Value,
                        node.Length,
                        rightOriginTextNode.TextContent);
                
                    result.Add(textInsert);
                    result.Add(rightOriginTextNode.ToDeleteSteps());
                }

                cursorPosition = AdjustSelection(cursorPosition, rightOriginTextNode, node);
                
                node.AddText(rightOriginTextNode.TextContent);
                rightOriginTextNode.Remove();
                document.RemoveNode(rightOriginTextNode);
            }
        }

        return result;
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