using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace SmartHome
{
    class RemoteControl
    {
        static async Task Main(string[] args)
        {
            using (
                var connection = EventStoreConnection.Create(
                    ConnectionSettings.Create().UseConsoleLogger().FailOnNoServerResponse(),
                    new Uri("tcp://eventstore:1113")
                )
            )
            {
                await connection.ConnectAsync();

                var RemoteControlCommandModel = new RemoteControlCommandModel()
                {
                    Identifier = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"),
                    Connection = connection
                };

                Random Random = new Random();
                while(true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(Random.Next(1,10)));
                    switch(Random.Next(0,3))
                    {
                        case 0:
                            RemoteControlCommandModel.DoIncrementBrightness();
                            break;
                        case 1:
                            RemoteControlCommandModel.DoDecrementBrightness();
                            break;
                        case 2:
                            RemoteControlCommandModel.DoIncrementColor();
                            break;
                        case 3:
                            RemoteControlCommandModel.DoDecrementColor();
                            break;
                    }
                }
            }
        }
    }
}