using DScratch.LayoutEngine;
using DScratch.Transactions;

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
        await layoutEngineService.RenderAsync(Document, transaction.Commit());
    }

    public async Task InitialTransactionAsync()
    {
        layoutEngineService.AddRoot(Document.Root);
        var transactionResult = new TransactionResult(new HashSet<ModifiedNode>
        {
            new ModifiedNode(Document.Root.FirstChild!, Modification.Insert)
        });
        await layoutEngineService.RenderAsync(Document, transactionResult);
    }
}