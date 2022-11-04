using Azure.Storage.Queues;
using System;
using System.Threading.Tasks;

namespace StorageQueueWriter
{
    class Program
    {
        static string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=psqueueueueueues;AccountKey=SI0VBVPB4vyfQyHAK8V6u5gBojOlakubd/DN57hC1HbFa+ldbMQIEoDeKmedl4tCv8bdmj7onJCu+ASt5SiFKA==;EndpointSuffix=core.windows.net";
        static string QueueName = "kassa";
        static async Task Main(string[] args)
        {
            await WriteToQueueAsync();
            Console.WriteLine("Press Enter to Quit");
            Console.ReadLine();
        }

        private static async Task WriteToQueueAsync()
        {
            var client = new QueueClient(ConnectionString, QueueName);
            for (int i = 0; i < 100; i++)
            {
                await client.SendMessageAsync($"Hello Number {i}", timeToLive:TimeSpan.FromHours(20));
            }
            
        }

    }
}
