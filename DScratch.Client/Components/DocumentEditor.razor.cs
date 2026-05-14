using DScratch.Client.Scripts;
using DScratch.Nodes;

namespace DScratch.Client.Components;

public partial class DocumentEditor(DNodeFactory nodeFactory) : IDisposable
{
    private DScratchDocument document = new DScratchDocument();

    protected override void OnInitialized()
    {
        KeyPressEventHelper.OnKeyPress += OnKeyPress;
    }

    private void OnKeyPress(KeyPressInfo keyPressInfo)
    {
        Console.WriteLine(keyPressInfo.Path.Length);
        
        if (keyPressInfo.Key.Value.Length == 1 && char.IsLetter(keyPressInfo.Key.Value, 0))
        {
            HandleLetter(keyPressInfo);
            return;
        }

        throw new NotImplementedException();
    }

    private void HandleLetter(KeyPressInfo keyPressInfo)
    {
        var transaction = new DTransaction(document);

        Console.WriteLine(string.Join('_', keyPressInfo.Path));
        var currentParagraph = transaction.FindNode<ParagraphNode>(keyPressInfo.Path);

        if (currentParagraph is null)
        {
            throw new ArgumentException("Could not find a paragraph at the expected path.");
        }

        var origin = currentParagraph.GetChild<DCharNode>(keyPressInfo.Selection.Offset - 1);
        var rightOrigin = currentParagraph.GetChild<DCharNode>(keyPressInfo.Selection.Offset);
            
        var node = nodeFactory.Char(keyPressInfo.Key.Value[0], origin, rightOrigin);
        currentParagraph.Insert(node);
    }
    
    public void Dispose()
    {
        KeyPressEventHelper.OnKeyPress -= OnKeyPress;
    }
}