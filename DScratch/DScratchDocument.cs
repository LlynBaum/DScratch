namespace DScratch;

public class DScratchDocument
{
    public DPage Page { get; }

    public DScratchDocument(DPage? page = null)
    {
        Page = page ?? DPage.Create(1);
    }
}