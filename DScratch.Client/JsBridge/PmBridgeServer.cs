using DScratch.Client.JsBridge.ProseMirrorCommand;

namespace DScratch.Client.JsBridge;

public class PmBridgeServer : IPmBridge
{
    public PmTransaction StartTransaction()
    {
        throw new InvalidOperationException("Can not use Transactions on server side.");
    }

    public Task DispatchCommandAsync(UpdateCommand update)
    {
        throw new InvalidOperationException("Can not dispatch Commands on server side.");
    }
}