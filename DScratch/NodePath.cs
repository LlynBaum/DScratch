namespace DScratch;

public readonly struct NodePath(List<string> path)
{
    public int Length => path.Count;

    public string[] Path => path.ToArray();
    
    public string this[int index] => path[index];
    
    public override string ToString()
    {
        return string.Join("__", path);
    }

    public static NodePath FromJs(string[] path)
    {
        return new NodePath(path.Reverse().ToList()); // JavaScript creates the Path in reverse order
    }
}