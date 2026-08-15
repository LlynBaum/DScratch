using DScratch.Nodes;
using DScratch.Transactions;

namespace DScratch.Rendering;

public static class AttributeRenderExtensions
{
    extension(DNode node)
    {
        public StepDiff? ToAttributeUpdate()
        {
            var attributes = node.GetAttributes();
            return attributes is null ? null : new StepDiff.UpdateAttributesDiff(node.Id.Value, attributes);
        }
    }
}