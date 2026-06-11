using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch;

public class DScratchService(INodeFactory nodeFactory, INodeIdGenerator nodeIdGenerator) : IDScratchService
{
    private readonly DScratchDocument document = new DScratchDocument(nodeIdGenerator.GetNextId());

    public bool DisableCleanUp { get; init; } = false;

    internal DScratchService(DScratchDocument document, INodeFactory nodeFactory, INodeIdGenerator nodeIdGenerator) 
        : this(nodeFactory, nodeIdGenerator)
    {
        this.document = document;
    }
    
    // TODO: history of past transaction, so things like ctrl-z can be possible.
    private readonly Stack<ITransaction> transactions = [];

    public INodeFactory NodeFactory => nodeFactory;

    public ITransaction StartTransaction()
    {
        return new DTransaction(document, nodeIdGenerator, DisableCleanUp);
    }
    
    public TransactionResult Apply(ITransaction transaction)
    {
        transactions.Push(transaction);
        return transaction.Commit();
    }

    public TransactionResult InitialTransaction()
    { 
        return new TransactionResult(document.Root.ToInsertSteps());
    }
}