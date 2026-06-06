using DScratch.Nodes;

namespace DScratch.Transactions;

public interface ITransaction
{
    DNode Root { get; }
    
    internal TransactionResult Commit();

    ITransaction Insert(DNode node, DNode parent);

    ITransaction Delete(DNode node);
    
    ITransaction DeleteRange(DNode? start, DNode? end);
    
    ITransaction MoveRange(DNode? start, DNode? end, DNode targetParent, DNode? targetOrigin);
    
    ITransaction AddCursorPosition(string nodeId, int offset);

    DNode? FindNode(NodeId nodeId);

    TextNode? SplitText(TextNode node, int offset);
}