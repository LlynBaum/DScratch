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

    public static IEnumerable<IResolveConstraint> Repeat(IEnumerable<IResolveConstraint> template, int count)
    {
        return Enumerable.Range(0, count).SelectMany(_ => template);
    }
    
    public static IEnumerable<IResolveConstraint> Repeat<T>(int count) where T : StepDiff
    {
        return Enumerable.Range(0, count).Select(IResolveConstraint (_) => Is.TypeOf<T>());
    }
}