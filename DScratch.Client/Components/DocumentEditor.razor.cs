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
        var node = nodeFactory.Char(keyPressInfo.Key.Value[0]);
        transaction.InsertAt(node, keyPressInfo.Path, keyPressInfo.Selection.Offset);
    }
    
    public void Dispose()
    {
        KeyPressEventHelper.OnKeyPress -= OnKeyPress;
    }
}