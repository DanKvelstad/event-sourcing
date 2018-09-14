using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Hello
{
    class Lamp
    {
        static async Task Main(string[] args)
        {

            Func<IPAddress> GetLocalIPAddress = () =>
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip;
                    }
                }
                throw new Exception("No network adapters with an IPv4 address in the system!");
            };

            using (var CancellationTokenSource = new CancellationTokenSource())
            using (var connection = EventStoreConnection.Create(
                ConnectionSettings.Create().UseConsoleLogger(),
                new IPEndPoint(GetLocalIPAddress(), 1113))
            )
            {

                string streamName = "smart-bulb";

                await connection.ConnectAsync();
                await connection.SubscribeToStreamAsync(
                    streamName, 
                    false, 
                    (EventStoreSubscription EventStoreSubscription, ResolvedEvent ResolvedEvent) => 
                    {
                        if(nameof(DimmerChangedEvent) == ResolvedEvent.Event.EventType)
                        {
                            var DimmerChangedEvent = JsonConvert.DeserializeObject(
                                Encoding.UTF8.GetString(ResolvedEvent.Event.Data)
                            ) as DimmerChangedEvent;
                            Console.WriteLine(
                                "The bulb is now the color {0} with intensity {1}",
                                DimmerChangedEvent.Color,
                                DimmerChangedEvent.Intensity
                            );
                        }
                    },
                    (EventStoreSubscription EventStoreSubscription, SubscriptionDropReason SubscriptionDropReason, Exception Exception) =>
                    {
                        Console.WriteLine(
                            "The subscription was dropped because of: {0}",
                            SubscriptionDropReason
                        );
                        CancellationTokenSource.Cancel();
                    }, 
                    null
                );

                CancellationTokenSource.Token.WaitHandle.WaitOne();

            }

        }
    }
}