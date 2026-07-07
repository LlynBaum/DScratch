using DScratch.Client.Services;
using DScratch.Interactions;

namespace DScratch.Client.Pages.Editor.Components.Debugging;

public partial class DebugTransaction(EditorDebugService editorDebugService) : IDisposable
{
     private KeyPressInfo? LastKeyPressInfo => editorDebugService.KeyPressInfos.LastOrDefault();

     protected override void OnInitialized()
     {
          editorDebugService.DocumentChanged += StateHasChanged;
     }

     public void Dispose()
     {
          editorDebugService.DocumentChanged -= StateHasChanged;
     }
}