using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace Hello
{
    class Program
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

            using (var connection = EventStoreConnection.Create(
                ConnectionSettings.Create().UseConsoleLogger(),
                new IPEndPoint(GetLocalIPAddress(), 1113))
            )
            {

                await connection.ConnectAsync();

                var myEvent = new EventData(
                    Guid.NewGuid(),
                     "testEvent",
                      false,
                      Encoding.UTF8.GetBytes("some data"),
                      Encoding.UTF8.GetBytes("some metadata")
                );

                await connection.AppendToStreamAsync(
                    "test-stream",
                    ExpectedVersion.Any, myEvent
                );

                var streamEvents = await connection.ReadStreamEventsForwardAsync(
                    "test-stream",
                     0,
                     1,
                     false
                );

                var returnedEvent = streamEvents.Events[0].Event;

                Console.WriteLine(
                    "Alpha read event with data: {0}, metadata: {1}",
                    Encoding.UTF8.GetString(returnedEvent.Data),
                    Encoding.UTF8.GetString(returnedEvent.Metadata)
                );

            }

        }
    }
}