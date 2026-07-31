using DScratch.Marks;
using DScratch.Nodes;
using DScratch.Rendering;
using DScratch.Transactions;

namespace DScratch.Tests.DScratchTests.Rendering;

public class MarkRenderExtensionsTests
{
    private static readonly IReadOnlyDictionary<MarkKey, string> Marks = new Dictionary<MarkKey, string>
    {
        { MarkKey.FontWeight, "bold" },
        { MarkKey.FontStyle, "italic" },
        { MarkKey.Color, "#fff" }
    };

    private static void AssertMarks(StepDiff.UpdateMarksDiff step)
    {
        AssertMark(step, "font-weight", "bold");
        AssertMark(step, "font-style", "italic");
        AssertMark(step, "color", "#fff");
    }

    [Test]
    public void ToMarkUpdate_TranslatesMarksCorrectly()
    {
        // Arrange
        var nodeId = new NodeId("Test", 1);
        var textNode = new TextNode(nodeId, null, null);
        foreach (var mark in Marks)
        {
            textNode.SetMark(mark.Key, mark.Value);
        }

        // Act
        var result = textNode.ToMarkUpdate();
        
        // Assert
        Assert.That(result, Is.TypeOf<StepDiff.UpdateMarksDiff>());
        var step = (StepDiff.UpdateMarksDiff)result;
        
        Assert.That(step.NodeId, Is.EqualTo(nodeId.Value));
        AssertMarks(step);
    }
    
    private static void AssertMark(StepDiff.UpdateMarksDiff step, string key, string value)
    {
        Assert.That(step.Marks, Contains.Key(key));
        Assert.That(step.Marks[key], Is.EqualTo(value));
    }
}