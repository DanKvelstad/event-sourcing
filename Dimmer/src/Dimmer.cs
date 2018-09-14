using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Hello
{
    class Dimmer
    {
        static async Task Main(string[] args)
        {

            string streamName = "smart-bulb";

            var connection = EventStoreConnection.Create(
                ConnectionSettings.Create().UseConsoleLogger().FailOnNoServerResponse().LimitReconnectionsTo(0),
                new Uri("tcp://eventstore:1113")
            );
            await connection.ConnectAsync();
            Random Random = new Random();
            while(true)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                await connection.AppendToStreamAsync(
                streamName,
                ExpectedVersion.Any, 
                    new EventData(
                        Guid.NewGuid(),
                        nameof(DimmerChangedEvent),
                        true,
                        Encoding.UTF8.GetBytes(
                            JsonConvert.SerializeObject(
                                new DimmerChangedEvent()
                                {
                                    Color     = "Red",
                                    Intensity = Random.Next(0, 255)
                                }
                            )
                        ),
                        Encoding.UTF8.GetBytes(
                            JsonConvert.SerializeObject(
                                new GeneralMetaData()
                                {
                                    Name      = "Dan"
                                }
                            )
                        )
                    )
                );
            }
            
        }
    }
}