using DScratch.Transactions;
using NUnit.Framework.Constraints;

namespace DScratch.Tests;

public static class AssertHelper
{
    public static void ThatStepsEqualTo(IReadOnlyList<StepDiff?> actual, params IResolveConstraint[] expected)
    {
        Assert.That(actual, Has.Count.EqualTo(expected.Length), $"Expected actual to contain {expected.Length} steps, but got {actual.Count}");
        using (Assert.EnterMultipleScope())
        {
            foreach (var (first, second) in actual.Zip(expected))
            {
                Assert.That(first, second);
            }
        }
    }
    
    public static void ThatCursorPositionEqualTo(CursorPosition? actual, string expectedId, int expectedOffset)
    {
        Assert.That(actual, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.ParentId, Is.EqualTo(expectedId));
            Assert.That(actual.Offset, Is.EqualTo(expectedOffset));
        }
    }
}