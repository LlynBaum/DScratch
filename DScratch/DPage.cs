using DScratch.Nodes;

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
            Root = new ParagraphNode("Darki-0", null, null) // TODO, should be done properly later with the id generator
        };
    }
}