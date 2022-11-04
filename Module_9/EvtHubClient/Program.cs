using System;
using System.Text;
using System.Threading.Tasks;

// New
using NewNS= Azure.Messaging.EventHubs;


namespace EvtHubClient
{
    class Program
    {
        private static string conStr = "Endpoint=sb://ps-hupholland.servicebus.windows.net/;SharedAccessKeyName=schrijvert;SharedAccessKey=mTFhu+aRe3eNbODrHwWXu/zaWMEI5x5rUtIpQ9w555g=;EntityPath=hubby";
        private static string hubName = "hubby";

        static async Task Main(string[] args)
        {
            // Check!! AZ-204 book describes EventHubClient which is an obsolete package
            // Use this solutuin
            await NewStyle();
            
            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        private static async Task NewStyle()
        {
            await using (var producerClient = new NewNS.Producer.EventHubProducerClient(conStr, hubName))
            {
                var eventBatch = await producerClient.CreateBatchAsync();
                for (int i = 0; i < 200; i++)
                {
                    eventBatch.TryAdd(new NewNS.EventData(Encoding.UTF8.GetBytes($"Hello World {i}")));
                }
                await producerClient.SendAsync(eventBatch);
                Console.WriteLine("Sent");
            }         
        }
    }
}
