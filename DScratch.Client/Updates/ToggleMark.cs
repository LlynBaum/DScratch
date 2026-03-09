namespace DScratch.Client.Updates;

public class ToggleMark(string markName) : CommandUpdate("marks", "toggle")
{
    public string MarkName { get; } = markName;
}