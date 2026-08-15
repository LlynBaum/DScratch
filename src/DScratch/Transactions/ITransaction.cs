using DScratch.Interactions;
using DScratch.Marks;
using DScratch.Nodes;

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
    
    void UpdateMark(DNode node, MarkKey key, string value);
    
    void UpdateAttributes(DNode node, Action update);
    
    void AddCursorPosition(NodeId nodeId, int offset);
    
    void AddCursorPosition(SelectionInfo selectionInfo);

    TextNode? SplitText(TextNode node, int offset);

    IReadOnlyDictionary<MarkKey, string> CalculateMarks(IReadOnlyDictionary<MarkKey, string> activeMarks);
}