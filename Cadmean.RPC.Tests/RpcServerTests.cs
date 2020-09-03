using Xunit;
using Xunit.Abstractions;

namespace Cadmean.RPC.Tests
{
    public class RpcServerTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public RpcServerTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        
        private object bru = new
        {
            Name = "bdskfjhd",
            Nice = 69,
        };
        
        
        [Fact]
        public async void ShouldCallFunction1()
        {
            var rpc = new RpcServer("https://localhost:5001");
            ((DefaultFunctionUrlProvider) rpc.Configuration.FunctionUrlProvider).Prefix = "api/v1";
            var f = rpc.Function("bru");
            testOutputHelper.WriteLine(rpc.Configuration.FunctionUrlProvider.GetUrl(rpc.Function("bru")));
            var output = await f.Call();
            testOutputHelper.WriteLine(output.Result.GetType().ToString());
            testOutputHelper.WriteLine(output.Result.ToString());
        }
    }
}