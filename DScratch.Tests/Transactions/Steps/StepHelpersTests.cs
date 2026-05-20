using DScratch.Nodes;
using DScratch.Transactions;
using DScratch.Transactions.Steps;

namespace DScratch.Tests.Transactions.Steps;

public class StepHelpersTests
{
    private class ToInsert
    {
        [Test]
        public void CharNode_ReturnsInsertTextDiff()
        {
            var node = new CharNode('a', "2", null, null, null);
            var path = new NodePath(["1"]);

            var result = node.ToInsert(path, 42);
            
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.That(result.Single(), Is.TypeOf<StepDiff.InsertTextDiff>());
            
            var step = (StepDiff.InsertTextDiff)result.Single();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(step.Parent, Is.EquivalentTo(["1"]));
                Assert.That(step.Offset, Is.EqualTo(42));
                Assert.That(step.Text, Is.EqualTo("a"));
            }
        }
        
        [Test]
        public void TextElement_ReturnsExpectedSteps()
        {
        }
        
        [Test]
        public void Element_ReturnsInsertElementDiff()
        {
        }
        
        [Test]
        public void UnknownNodes_ThrowArgumentException()
        {
        }
    }
    
    private class ToDelete
    {
        
    }
}