using DScratch.Interactions;
using DScratch.Transactions;
using NUnit.Framework.Constraints;

namespace DScratch.Tests;

public static class AssertHelper
{
    public static void ThatCursorPositionEqualTo(SelectionInfo? actual, NodeId expectedId, int expectedOffset)
    {
        Assert.That(actual, Is.Not.Null, "Expected to have a cursor position, but got null.");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.AnchorId, Is.EqualTo(expectedId.Value), $"Expected {expectedId} as target parent for selection.");
            Assert.That(actual.AnchorOffset, Is.EqualTo(expectedOffset), $"Expected an offset of {expectedOffset} for selection.");
        }
    }
    
    public static void ThatCursorPositionEqualTo(SelectionInfo? actual, SelectionInfo expected)
    {
        Assert.That(actual, Is.Not.Null, "Expected to have a cursor position, but got null.");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.AnchorId, Is.EqualTo(expected.AnchorId), $"Expected {expected.AnchorId} as anchor parent for selection.");
            Assert.That(actual.AnchorOffset, Is.EqualTo(expected.AnchorOffset), $"Expected an anchor offset of {expected.AnchorOffset} for selection.");
            
            Assert.That(actual.FocusId, Is.EqualTo(expected.FocusId), $"Expected {expected.FocusId} as focus parent for selection.");
            Assert.That(actual.FocusOffset, Is.EqualTo(expected.FocusOffset), $"Expected an focus offset of {expected.FocusOffset} for selection.");
        }
    }
}