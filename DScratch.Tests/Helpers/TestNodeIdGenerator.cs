namespace DScratch.Tests.Helpers;

public class TestNodeIdGenerator : INodeIdGenerator
{
    private int currentId;
    
    public string GetNextId()
    {
        return (currentId++).ToString();
    }
}