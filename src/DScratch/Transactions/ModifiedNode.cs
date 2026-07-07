using DScratch.Nodes;

namespace DScratch.Transactions;

public record ModifiedNode(DNode Node, Modification Modification);

public enum Modification
{
    Insert,
    Delete,
    Changed
}