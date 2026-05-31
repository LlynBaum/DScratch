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
                Assert.That(first, second, $"Expected {first?.Type ?? "null"} to be of expected type.");
            }
        }
    }
    
    public static void ThatCursorPositionEqualTo(CursorPosition? actual, string expectedId, int expectedOffset)
    {
        Assert.That(actual, Is.Not.Null, "Expected to have a cursor position, but got null.");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.ParentId, Is.EqualTo(expectedId), $"Expected {expectedId} as target parent for selection.");
            Assert.That(actual.Offset, Is.EqualTo(expectedOffset), $"Expected an offset of{expectedOffset} for selection.");
        }
    }
}