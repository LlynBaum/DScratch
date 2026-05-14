using DScratch.Client.Scripts;
using Microsoft.JSInterop;

namespace DScratch.Client.Components;

public partial class DocumentEditor
{
    private DScratchDocument document = new DScratchDocument();

    [JSInvokable]
    public static void OnKeyPress(KeyPressInfo keyPressInfo)
    {
        
    }
}