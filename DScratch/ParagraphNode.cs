namespace DScratch;

public class ParagraphNode(string id, DNode? origin = null, DNode? rightOrigin = null) : DNode(id, origin, rightOrigin)
{
    public string Value { get; private set; } = string.Empty;
    
    public CharNode? Characters { get; private set; }

    public void Insert(CharNode start)
    {
        var end = start;
        while (true)
        {
            if(end?.RightOrigin is null) break;
            end = end.NextChar;
        }

        if (start.Origin is null) throw new NullReferenceException("Origin must be set on DNode, to be able to insert it.");
        var (insert, idx) = FindCharNode(start.Origin);

        if(insert is null) return; // TODO: should not happen, but how to protect?

        insert.RightOrigin?.Origin = end;
        insert.RightOrigin = start;
        
        var value = GetValue(start);
        Value = Value.Insert(idx, value);
    }

    private (CharNode? node, int idx) FindCharNode(DNode node)
    {
        var idx = 0;
        var current = Characters;
        while (true)
        {
            if (current is null) return (null, -1);
            if (current.Id == node.Id) return (current, idx);
            current = current.NextChar;
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
            current = node.NextChar;
        }

        return str;
    }
}