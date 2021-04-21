using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DemoFunction
{
    public static class HttpFunction
    {
        [FunctionName(nameof(HttpFunction))]
        public static async Task<HttpResponseMessage> RunHttpFunction(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
                [DurableClient] IDurableOrchestrationClient starter,
                ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync(nameof(OrchestratorFunction), null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }


        public static async IList<string> GetMessages()
        {
            static async Task SendMessageAsync()
            {
                // create a Service Bus client 
                await using (ServiceBusClient client = new ServiceBusClient(connectionString))
                {
                    // create a sender for the queue 
                    ServiceBusSender sender = client.CreateSender(queueName);

                    // create a message that we can send
                    ServiceBusMessage message = new ServiceBusMessage("Hello world!");

                    // send the message
                    await sender.SendMessageAsync(message);
                    Console.WriteLine($"Sent a single message to the queue: {queueName}");
                }
            }
        }
    }
}
