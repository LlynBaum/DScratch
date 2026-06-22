using Microsoft.Playwright;

namespace DScratch.Tests.E2E.Framework;

public static class DSelectors
{
    extension(ILocator locator)
    {
        public ILocator Paragraph => locator.Locator("p[data-path-id]");
        
        public ILocator TextSpan => locator.Locator("span[data-path-id]");
    }
}