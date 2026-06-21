namespace DScratch.Client.Services;

public class EditorDebugService
{
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

    public void NotifyDocumentChanged()
    {
        DocumentChanged?.Invoke();
    }
}
