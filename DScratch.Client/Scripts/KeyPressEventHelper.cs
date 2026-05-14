using Microsoft.JSInterop;

namespace DScratch.Client.Scripts;

public static class KeyPressEventHelper
{
    public static event Action<KeyPressInfo> OnKeyPress = null!;
    
    [JSInvokable]
    public static void OnKeyPressCallback(KeyPressInfo keyPressInfo)
    {
        OnKeyPress.Invoke(keyPressInfo);
    }
}