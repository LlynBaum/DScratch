using DScratch.Interactions.CommandHandlers.Handlers;
using DScratch.Interactions.UserStates;
using DScratch.Tests.Helpers;

namespace DScratch.Tests.DScratchTests.Interactions.CommandHandlers;

public class AddLinkHandlerTest
{
    private TreeBuilder builder;
    private DScratchService dScratchService;
    private AddLinkHandler handler;
    
    [SetUp]
    public void SetUp()
    {
        builder = new TreeBuilder();
        dScratchService = new DScratchService(
            document: builder.CreateDocument(), 
            nodeFactory: new DNodeFactory(builder.IdGenerator), 
            userStateService: new UserStateService()) { DisableCleanUp = true };
        
        handler = new AddLinkHandler(dScratchService);
    }
}