using Newtonsoft.Json.Linq;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class JsonValueTests : PuppeteerPageBaseTest
    {
        public JsonValueTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.jsonValue", "should work")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWork()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync("({ foo: 'bar'})");
            var json = await aHandle.JsonValueAsync();
            Assert.Equal(JObject.Parse("{ foo: 'bar' }"), json);
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.jsonValue", "should not work with dates")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldNotWorkWithDates()
        {
            var dateHandle = await Page.EvaluateExpressionHandleAsync("new Date('2017-09-26T00:00:00.000Z')");
            var json = await dateHandle.JsonValueAsync();
            Assert.Equal(JObject.Parse("{}"), json);
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.jsonValue", "should throw for circular objects")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldThrowForCircularObjects()
        {
            var windowHandle = await Page.EvaluateExpressionHandleAsync("window");
            var exception = await Assert.ThrowsAsync<MessageException>(()
                => windowHandle.JsonValueAsync());

            Assert.Contains(TestConstants.IsChrome ? "Object reference chain is too long" : "Object is not serializable", exception.Message);
        }
    }
}