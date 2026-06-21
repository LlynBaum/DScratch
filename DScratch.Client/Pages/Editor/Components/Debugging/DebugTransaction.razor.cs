using DScratch.Client.Services;
using DScratch.Transactions;

namespace DScratch.Client.Pages.Editor.Components.Debugging;

public partial class DebugTransaction(EditorDebugService editorDebugService) : IDisposable
{
     private TransactionResult? LastTransaction => editorDebugService.Transactions.LastOrDefault();

     protected override void OnInitialized()
     {
          editorDebugService.DocumentChanged += StateHasChanged;
     }

     public void Dispose()
     {
          editorDebugService.DocumentChanged -= StateHasChanged;
     }
}