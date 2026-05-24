namespace DScratch.TreeVisualizers;

public class DocumentVisualizer(DScratchDocument document)
{
    public void Print()
    {
        var visualizer = new TreeVisualizer(document.Page.Root);
        visualizer.Print();
    }
}