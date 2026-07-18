using DScratch.Interactions.UserStates;
using DScratch.Rendering;
using DScratch.Transactions;

namespace DScratch;

public class DScratchService(
    INodeFactory nodeFactory, 
    IUserStateService userStateService) : IDScratchService
{
    public DScratchDocument Document { get; } = new DScratchDocument(nodeFactory.NodeIdGenerator.GetNextId());

    public bool DisableCleanUp { get; init; }

    internal DScratchService(
        DScratchDocument document, 
        INodeFactory nodeFactory, 
        IUserStateService userStateService) 
        : this(nodeFactory, userStateService)
    {
        Document = document;
    }
    
    // TODO: history of past transaction, so things like ctrl-z can be possible.
    private readonly Stack<ITransaction> transactions = [];

    public ITransaction StartTransaction()
    {
        return new DTransaction(Document, nodeFactory, userStateService, DisableCleanUp);
    }
    
    public TransactionResult Apply(ITransaction transaction)
    {
        transactions.Push(transaction);
        return transaction.Commit();
    }

    public TransactionResult InitialTransaction()
    { 
        return new TransactionResult(Document.Root.ToInsertSteps());
    }
}