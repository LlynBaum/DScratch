using DScratch.Nodes;

namespace DScratch.Transactions.Steps;

public static class StepHelpers
{
    extension(DNode node)
    {
        public StepDiff ToInsert(NodePath path, int offset)
        {
            if (node is DCharNode charNode)
            {
                return new StepDiff.InsertTextDiff(path.Path, offset, charNode.Value.ToString());
            }

            return new StepDiff.InsertElementDiff(path.Path, offset, node.TagName, node.Id);
        }
        
        public StepDiff ToDelete(NodePath path, int offset)
        {
            if (node is DCharNode)
            {
                return new StepDiff.DeleteTextDiff(path.Path, offset, 1);
            }

            return new StepDiff.DeleteElementDiff(path.Path);
        }
    } 
}