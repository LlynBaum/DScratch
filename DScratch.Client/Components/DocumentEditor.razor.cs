using DScratch.Client.Scripts;

namespace DScratch.Client.Components;

public partial class DocumentEditor : IDisposable
{
    private DScratchDocument document = new DScratchDocument();

    protected override void OnInitialized()
    {
        KeyPressEventHelper.OnKeyPress += OnKeyPress;
    }

    public void OnKeyPress(KeyPressInfo keyPressInfo)
    {
        
    }

    public void Dispose()
    {
        KeyPressEventHelper.OnKeyPress -= OnKeyPress;
    }
}