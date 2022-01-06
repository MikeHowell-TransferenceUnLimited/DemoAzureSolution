using Microsoft.Azure.ServiceBus;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace ReceiveMessageFromServiceBus
{
    class Program
    {
        const string ServiceBusConnectionString = "Endpoint=sb://mchservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=wqNToRLry6tmtI5eJ60pLSGSXf5ykZV87Ut8Yc7Nygs=";
        const string TopicName = "mytopic";
        const string SubscriptionName = "mchSubscription";
        static ISubscriptionClient subscriptionClient;

        static void Main(string[] args)
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            subscriptionClient = new SubscriptionClient(ServiceBusConnectionString, TopicName, SubscriptionName);
            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

            Console.WriteLine("Finished...");
            Console.ReadKey();
            subscriptionClient.CloseAsync();
        }


        static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            Console.WriteLine($"Received message: Sequence number:{message.SystemProperties.EnqueuedSequenceNumber} Body: {Encoding.UTF8.GetString(message.Body)}");
            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
