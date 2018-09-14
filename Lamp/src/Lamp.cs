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

            string streamName = "smart-bulb";

            var CancellationTokenSource = new CancellationTokenSource();
            var connection = EventStoreConnection.Create(
                ConnectionSettings.Create().UseConsoleLogger(),
                new Uri("tcp://eventstore:1113")
            );
            await connection.ConnectAsync();
            await connection.SubscribeToStreamAsync(
                streamName, 
                false, 
                (EventStoreSubscription EventStoreSubscription, ResolvedEvent ResolvedEvent) => 
                {
                    if(nameof(DimmerChangedEvent) == ResolvedEvent.Event.EventType)
                    {
                        var DimmerChangedEvent = JsonConvert.DeserializeObject(
                            Encoding.UTF8.GetString(ResolvedEvent.Event.Data),
                            typeof(DimmerChangedEvent)
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