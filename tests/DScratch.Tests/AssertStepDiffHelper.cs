using DScratch.Transactions;
using NUnit.Framework.Constraints;

namespace DScratch.Tests;

public static class AssertStepDiffHelper
{    
    public static readonly IEnumerable<IResolveConstraint> InsertTextNodeTypes = [
        Is.TypeOf<StepDiff.InsertElementDiff>(),
        Is.TypeOf<StepDiff.InsertTextDiff>(),
        Is.TypeOf<StepDiff.UpdateMarksDiff>()
    ];
    
    public static readonly IEnumerable<IResolveConstraint> SplitTextTypes = [
        Is.TypeOf<StepDiff.DeleteTextDiff>(),
        ..InsertTextNodeTypes
    ];
}