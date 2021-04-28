using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DemoFunction.ServiceBusDurableFunction
{
    public static class DurableOrchestrator
    {
        [FunctionName(nameof(DurableOrchestrator))]
        public static async Task<bool> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            bool input = context.GetInput<bool>();
            return await context.CallActivityAsync<bool>(nameof(ActivityFunction), input);
        }

        [FunctionName(nameof(ActivityFunction))]
        public static async Task<bool> ActivityFunction([ActivityTrigger] bool input, ILogger log)
        {
            return await Task.FromResult<bool>(input);
        }
    }
}