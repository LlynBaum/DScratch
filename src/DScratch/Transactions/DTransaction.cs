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
        var cleanUpSteps = CleanupTree(modifiedNodes);
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

    private List<StepDiff?> CleanupTree(IReadOnlyList<DNode> nodes)
    {
        List<StepDiff?> result = [];
        if(disableCleanUp)
        {
            return result;
        }
        
        foreach (var node in nodes.OfType<TextNode>())
        {
            if (CleanUpHelper.CanMergeWithOrigin(node, out var origin))
            {
                if (!origin.IsDeleted)
                {
                    var textInsert = new StepDiff.InsertTextDiff(
                        origin.Id.Value,
                        origin.Length,
                        node.TextContent);
                
                    result.Add(textInsert);
                    result.Add(node.ToDeleteSteps());
                }

                cursorPosition = CleanUpHelper.AdjustSelection(cursorPosition, node, origin);

                origin.AddText(node.TextContent);
                node.Remove();
                document.RemoveNode(node);
            }
            else if (CleanUpHelper.CanMergeWithRightOrigin(node, out var rightOrigin))
            {
                if (!rightOrigin.IsDeleted)
                {
                    var textInsert = new StepDiff.InsertTextDiff(
                        node.Id.Value,
                        node.Length,
                        rightOrigin.TextContent);

                    result.Add(textInsert);
                    result.Add(rightOrigin.ToDeleteSteps());
                }

                cursorPosition = CleanUpHelper.AdjustSelection(cursorPosition, rightOrigin, node);
                
                node.AddText(rightOrigin.TextContent);
                rightOrigin.Remove();
                document.RemoveNode(rightOrigin);
            }
        }

        return result;
    }
}