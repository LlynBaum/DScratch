namespace DScratch.Client.Updates;

public class CommandUpdate(string type, string method)
{
    public string Type { get; } = type;

    public string Method { get; } = method;
}