using Microsoft.Azure.ServiceBus;
using System;
using System.Text;

namespace SendMessageToServiceBus
{
    class Program
    {
        const string ServiceBusConnectionString = "Endpoint=sb://mchservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=wqNToRLry6tmtI5eJ60pLSGSXf5ykZV87Ut8Yc7Nygs=";
        const string TopicName = "myTopic";
        const int numberOfMessagesToSend = 100;

        static ITopicClient topicClient;

        static void Main(string[] args)
        {
            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            Console.WriteLine("Press ENTER key to exit after sending all the messages.");

            Console.WriteLine();

            try
            {
                for (var i = 0; i < numberOfMessagesToSend; i++)
                {
                    string messageBody = $"Message {i} {DateTime.Now}";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                    Console.WriteLine($"Sending message: {messageBody}");

                    topicClient.SendAsync(message);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
            Console.ReadKey();

            topicClient.CloseAsync();
        }
    }
}
