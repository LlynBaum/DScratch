namespace DScratch.Nodes;

public readonly record struct Mark(string Key, string Value)
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