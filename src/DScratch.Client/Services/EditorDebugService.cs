using DScratch.Interactions;

namespace DScratch.Client.Services;

public class EditorDebugService
{
    private readonly List<KeyPressInfo> keyPressInfos = [];
    public IReadOnlyList<KeyPressInfo> KeyPressInfos => keyPressInfos;
    
    public bool IsDebugEnabled
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                DebugModeChanged?.Invoke();
            }
        }
    }
    
    public event Action? DocumentChanged;
    public event Action? DebugModeChanged;

    public void NotifyKeyPress(KeyPressInfo keyPressInfo)
    {
        keyPressInfos.Add(keyPressInfo);
        DocumentChanged?.Invoke();
    }
}
