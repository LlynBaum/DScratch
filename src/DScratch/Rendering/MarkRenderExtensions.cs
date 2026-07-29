using DScratch.Nodes;
using DScratch.Nodes.Marks;
using DScratch.Transactions;

namespace DScratch.Rendering;

internal static class MarkRenderExtensions
{
    extension(DNode node)
    {
        public StepDiff ToMarkUpdate()
        {
            var marks = node.Marks.Select(m => m.Key.ToStyle(m.Value)).ToDictionary();
            return new StepDiff.UpdateMarksDiff(node.Id.Value, marks);
        }
    }

    private static KeyValuePair<string, string> ToStyle(this MarkKey key, string value)
    {
        return key switch
        {
            MarkKey.FontWeight => new KeyValuePair<string, string>("font-weight", value),
            MarkKey.FontStyle => new KeyValuePair<string, string>("font-style", value),
            MarkKey.Color => new KeyValuePair<string, string>("color", value),
            _ => throw new ArgumentOutOfRangeException(nameof(key))
        };
    }
}