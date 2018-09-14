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

            using (
                var connection = EventStoreConnection.Create(
                    ConnectionSettings.Create().UseConsoleLogger(),
                    new IPEndPoint(GetLocalIPAddress(), 1113)
                )
            )
            {

                string streamName = "smart-bulb";

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
}