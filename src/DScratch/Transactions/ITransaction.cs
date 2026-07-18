using DScratch.Interactions;
using DScratch.Nodes;
using DScratch.Nodes.Marks;

namespace DScratch.Transactions;

public interface ITransaction
{
    DScratchDocument Document { get; }
    
    DNode Root { get; }
    
    INodeFactory NodeFactory { get; }

    internal TransactionResult Commit();

    void Insert(DNode node, DNode parent);

    void Delete(DNode node);
    
    void DeleteRange(DNode? start, DNode? end);
    
    void MoveRange(DNode? start, DNode? end, DNode targetParent, DNode? targetOrigin);

    void ReplaceNode(DNode node, Func<DNode, DNode> copyFactory);
    
    void AddMark(TextNode node, Mark mark);
    
    void RemoveMark(TextNode node, MarkKey key);
    
    void AddCursorPosition(NodeId nodeId, int offset);
    
    void AddCursorPosition(SelectionInfo selectionInfo);

    TextNode? SplitText(TextNode node, int offset);

    IReadOnlySet<Mark> PopPendingMarks();
}