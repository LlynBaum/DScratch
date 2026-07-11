namespace DScratch.Nodes.Marks;

public readonly record struct Mark(MarkKey Key, string? Value = null)
{
    public class MarkTable : IEqualityComparer<Mark>
    {
        public bool Equals(Mark x, Mark y)
        {
            return x.Key == y.Key;
        }

        public int GetHashCode(Mark obj)
        {
            return obj.Key.GetHashCode();
        }
    }
}