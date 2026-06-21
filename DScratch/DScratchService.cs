using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch;

public class DScratchService(INodeFactory nodeFactory, INodeIdGenerator nodeIdGenerator) : IDScratchService
{
    private readonly DScratchDocument document = new DScratchDocument(nodeIdGenerator.GetNextId());

    public DScratchDocument Document => document;

    private bool isDebugEnabled;
    public bool IsDebugEnabled
    {
        get => isDebugEnabled;
        set
        {
            if (isDebugEnabled != value)
            {
                isDebugEnabled = value;
                DebugModeChanged?.Invoke();
            }
        }
    }

    public event Action? DocumentChanged;
    public event Action? DebugModeChanged;

    public bool DisableCleanUp { get; init; } = false;

    internal DScratchService(DScratchDocument document, INodeFactory nodeFactory, INodeIdGenerator nodeIdGenerator) 
        : this(nodeFactory, nodeIdGenerator)
    {
        this.document = document;
    }
    
    // TODO: history of past transaction, so things like ctrl-z can be possible.
    private readonly Stack<ITransaction> transactions = [];

    public ITransaction StartTransaction()
    {
        return new DTransaction(document, nodeFactory, nodeIdGenerator, DisableCleanUp);
    }
    
    public TransactionResult Apply(ITransaction transaction)
    {
        transactions.Push(transaction);
        var result = transaction.Commit();
        DocumentChanged?.Invoke();
        return result;
    }

    public TransactionResult InitialTransaction()
    { 
        var result = new TransactionResult(document.Root.ToInsertSteps());
        DocumentChanged?.Invoke();
        return result;
    }
}