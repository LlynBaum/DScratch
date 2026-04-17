namespace DScratch.Client.Updates.ProseMirrorCommand;

public class UpdateCommand(string type, string name)
{
    public string Type { get; } = type;

    public string Name { get; } = name;

    public Dictionary<string, object>? Attrs;
}