using DScratch.Client.Services;
using DScratch.Transactions;
using DScratch.Interactions;

namespace DScratch.Client.Pages.Editor.Components.Debugging;

public partial class DebugTransaction(IEditorDebugService editorDebugService) : IDisposable
{
     private DebugTransactionInfo? LastTransactionInfo => editorDebugService.Transactions.LastOrDefault();
     private TransactionResult? LastTransaction => LastTransactionInfo?.Result;
     private KeyPressInfo? LastKeyPressInfo => LastTransactionInfo?.KeyPressInfo;

     protected override void OnInitialized()
     {
          editorDebugService.DocumentChanged += StateHasChanged;
     }

     public void Dispose()
     {
          editorDebugService.DocumentChanged -= StateHasChanged;
     }
}