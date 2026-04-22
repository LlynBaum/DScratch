using DScratch.Client.JsBridge.ProseMirrorCommand;

namespace DScratch.Client.JsBridge;

public interface IPmBridge
{
    PmTransaction StartTransaction();

    Task DispatchCommandAsync(UpdateCommand update);
}