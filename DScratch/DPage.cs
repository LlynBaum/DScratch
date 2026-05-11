namespace DScratch;

public class DPage
{
    public required DNode Root { get; set; }

    public int PageNumber { get; }

    private DPage(int pageNumber)
    {
        PageNumber = pageNumber;
    }
    
    public static DPage Create(int pageNumber)
    {
        return new DPage(pageNumber)
        {
            Root = new ParagraphNode()
        };
    }
}