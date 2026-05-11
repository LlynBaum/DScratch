namespace DScratch;

public class ParagraphNode(DNode? origin = null, DNode? rightOrigin = null) : DNode(origin, rightOrigin)
{
    public string Value { get; private set; } = string.Empty;
    
    public CharNode? Characters { get; private set; }

    public void Insert(CharNode start)
    {
        var end = start;
        while (true)
        {
            if(end.RightOrigin is null) break;
            end = end.RightOrigin;
        }
        
        var (insert, idx) = FindCharNode(start);

        if(insert is null) return; // TODO: should not happen, but how to protect?

        insert.RightOrigin?.Origin = end;
        insert.RightOrigin = start;
        
        var value = GetValue(start);
        Value = Value.Insert(idx, value);
    }

    private (CharNode? node, int idx) FindCharNode(CharNode node, CharNode? start = null)
    {
        var idx = 0;
        var current = start ?? Characters;
        while (true)
        {
            if (current is null) return (null, -1);
            if (current.Equals(current)) return (node, idx);
            current = current.RightOrigin;
            idx++;
        }
    }

    private static string GetValue(CharNode node)
    {
        var str = "";
        var current = node;
        while (current is not null)
        {
            str += current.Value;
            current = node.RightOrigin;
        }

        return str;
    }
}