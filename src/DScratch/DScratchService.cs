using DScratch.LayoutEngine;
using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch;

public class DScratchService(
    INodeFactory nodeFactory,
    INodeIdGenerator nodeIdGenerator,
    ILayoutEngineService layoutEngineService) : IDScratchService
{
    public DScratchDocument Document { get; } = new DScratchDocument(nodeIdGenerator.GetNextId());

    public bool DisableCleanUp { get; init; }

    internal DScratchService(DScratchDocument document, INodeFactory nodeFactory, INodeIdGenerator nodeIdGenerator, ILayoutEngineService layoutEngineService) 
        : this(nodeFactory, nodeIdGenerator, layoutEngineService)
    {
        Document = document;
    }

    // TODO: history of past transaction, so things like ctrl-z can be possible.
    private readonly Stack<ITransaction> transactions = [];

    public ITransaction StartTransaction()
    {
        return new DTransaction(Document, nodeFactory, nodeIdGenerator, DisableCleanUp);
    }
    
    public async Task ApplyAsync(ITransaction transaction)
    {
        transactions.Push(transaction);
        await layoutEngineService.LayoutAsync(transaction.Commit());
    }

    public TransactionResult InitialTransaction()
    { 
        return new TransactionResult(Document.Root.ToInsertSteps());
    }
}