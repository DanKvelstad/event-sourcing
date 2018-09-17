using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace SmartHome
{
    class LightBulb
    {
        static async Task Main(string[] args)
        {

            Guid   identifier = Guid.NewGuid();
            string streamName = "smart_bulb-" + identifier;

            var CancellationTokenSource = new CancellationTokenSource();
            var connection = EventStoreConnection.Create(
                ConnectionSettings.Create().UseConsoleLogger().FailOnNoServerResponse().LimitReconnectionsTo(0),
                new Uri("tcp://eventstore:1113")
            );
            
            await connection.ConnectAsync();

            var QueryModel = new RemoteControlQueryModel(
                connection,
                new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00")
            );
            QueryModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                var RemoteControlQueryModel = sender as RemoteControlQueryModel;
                switch(e.PropertyName)
                {
                    case null:
                        CancellationTokenSource.Cancel();
                        break;
                    default:
                        Console.WriteLine(
                            "The bulb is now the color {0} with brightness {1}",
                            RemoteControlQueryModel.Color,
                            RemoteControlQueryModel.Brightness
                        );
                        break;
                }
            };
            
            CancellationTokenSource.Token.WaitHandle.WaitOne();
          
        }
    }
}