using DScratch.Client.Scripts;

namespace DScratch.Client.Components;

public partial class DocumentEditor(DNodeFactory nodeFactory) : IDisposable
{
    private readonly DScratchDocument document = new DScratchDocument();

    protected override void OnInitialized()
    {
        KeyPressEventHelper.OnKeyPress += OnKeyPress;
    }

    private void OnKeyPress(KeyPressInfo keyPressInfo)
    {
        if (keyPressInfo.Key.Value.Length == 1 && char.IsLetter(keyPressInfo.Key.Value, 0))
        {
            HandleLetter(keyPressInfo); // TODO: I think insert does not work as expected... the output of the tree is reversed lmao
            StateHasChanged();
            return;
        }

        switch (keyPressInfo.Key.Value)
        {
            case "Backspace":
                var transaction = new DTransaction(document);
                transaction.DeleteNode(keyPressInfo.GetNodePath(), keyPressInfo.Selection.Offset);
                break;
        }

        throw new NotImplementedException();
    }

    private void HandleLetter(KeyPressInfo keyPressInfo)
    {
        var transaction = new DTransaction(document);
        var node = nodeFactory.Char(keyPressInfo.Key.Value[0]);
        transaction.InsertAt(node, keyPressInfo.GetNodePath(), keyPressInfo.Selection.Offset);
    }

    public void Dispose()
    {
        KeyPressEventHelper.OnKeyPress -= OnKeyPress;
    }
}