using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DemoFunction.ServiceBusDurableFunction
{
    public static class ServiceBusTriggerFunction
    {
        //[FunctionName("ServiceBusTriggerFunction")]
        //public static void Run2([ServiceBusTrigger("demoqueue", Connection = "servicebusconnectionstring")] string myQueueItem, ILogger log)
        //{
        //    log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        //}

        [FunctionName("ServiceBusTriggerFunction")]
        public static async Task Run(
                [ServiceBusTrigger("demoqueue", Connection = "ServiceBusConnectionString")] Message message,
                MessageReceiver messageReceiver,
                string lockToken,
                [DurableClient] IDurableOrchestrationClient starter, ILogger log)
        {
            string inputMessage = Encoding.UTF8.GetString(message.Body);
            log.LogInformation($"message - " + inputMessage);

            string instanceId = await starter.StartNewAsync(nameof(DurableOrchestrator));
            log.LogInformation($"Orchestration Started with ID: {instanceId}");

            var orchestrationStatus = await starter.GetStatusAsync(instanceId);
            var status = orchestrationStatus.RuntimeStatus.ToString().ToUpper();
            log.LogInformation($"Waiting to complete Orchestration function [Status:{status}][ID:{instanceId}]");

            while (status == "PENDING" || status == "RUNNING")
            {
                await Task.Delay(1000);
                orchestrationStatus = await starter.GetStatusAsync(instanceId);
                status = orchestrationStatus.RuntimeStatus.ToString().ToUpper();
            }

            log.LogInformation($"{nameof(DurableOrchestrator)} Function completed [Instance ID:{instanceId}]: {orchestrationStatus.Output}");

        }
    }
}






