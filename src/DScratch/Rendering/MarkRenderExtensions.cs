using DScratch.Nodes;
using DScratch.Nodes.Marks;
using DScratch.Transactions;

namespace DScratch.Rendering;

internal static class MarkRenderExtensions
{
    extension(TextNode node)
    {
        public StepDiff[] ToMarkUpdate()
        {
            var marks = node.Marks.Select(m => m.ToStyle()).ToDictionary();
            return [new StepDiff.UpdateMarksDiff(node.Id.Value, marks)];
        }
    }

    private static KeyValuePair<string, string> ToStyle(this Mark mark)
    {
        return mark.Key switch
        {
            MarkKey.Bold => new KeyValuePair<string, string>("font-weight", "bold"),
            MarkKey.Italic => new KeyValuePair<string, string>("font-style", "italic"),
            MarkKey.Color => new KeyValuePair<string, string>("color", mark.Value!),
            _ => throw new ArgumentOutOfRangeException(nameof(mark))
        };
    }
}