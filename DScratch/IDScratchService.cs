using DScratch.Transactions;

namespace DScratch;

public interface IDScratchService
{
    INodeFactory NodeFactory { get; }
    
    ITransaction StartTransaction();
    
    IReadOnlyList<StepDiff?> Apply(ITransaction transaction);
    
    IReadOnlyList<StepDiff?> InitialTransaction();
}