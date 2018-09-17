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

            var connection = EventStoreConnection.Create(
                ConnectionSettings.Create().UseConsoleLogger().FailOnNoServerResponse().LimitReconnectionsTo(0),
                new Uri("tcp://eventstore:1113")
            );
            await connection.ConnectAsync();
            
            var RemoteControlQueryModel = new RemoteControlQueryModel()
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
                        RemoteControlQueryModel.DoIncrementBrightness();
                        break;
                    case 1:
                        RemoteControlQueryModel.DoDecrementBrightness();
                        break;
                    case 2:
                        RemoteControlQueryModel.DoIncrementColor();
                        break;
                    case 3:
                        RemoteControlQueryModel.DoDecrementColor();
                        break;
                }
            }
            
        }
    }
}