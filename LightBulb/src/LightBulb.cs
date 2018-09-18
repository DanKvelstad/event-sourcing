using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace SmartHome
{
    class LightBulb
    {
        static async Task Main(string[] args)
        {

            var LightBulbIdentifier = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00");
            QueryModel LightBulbQueryModel;

            var CancellationTokenSource = new CancellationTokenSource();
            
            var MongoClient         = new MongoDB.Driver.MongoClient("mongodb://mongo:27017");
            var MongoSmartHome      = MongoClient.GetDatabase("smart_home");
            var MongoLightBulbs     = MongoSmartHome.GetCollection<QueryModel>("light_bulbs");
            var MongoFilter         = MongoDB.Driver.Builders<QueryModel>.Filter.Eq(
                nameof(QueryModel.Identifier),
                LightBulbIdentifier
            );
            var MongoLightBulbsIt   = await MongoLightBulbs.FindAsync<QueryModel>(MongoFilter);
            if(await MongoLightBulbsIt.MoveNextAsync() && null != MongoLightBulbsIt.Current && 0 < MongoLightBulbsIt.Current.Count())
            {
                LightBulbQueryModel = MongoLightBulbsIt.Current.First();
            }
            else
            {
                LightBulbQueryModel = new QueryModel()
                {
                    Identifier = LightBulbIdentifier
                };
                await MongoLightBulbs.InsertOneAsync(LightBulbQueryModel);
            }
            LightBulbQueryModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                var QueryModel = sender as QueryModel;
                switch(e.PropertyName)
                {
                    case null:
                        CancellationTokenSource.Cancel();
                        break;
                    default:
                        Console.WriteLine(
                            "The bulb is now the color {0} with brightness {1} based on {2}",
                            QueryModel.Color,
                            QueryModel.Brightness,
                            QueryModel.EventNumber
                        );
                        break;
                }
            };
            LightBulbQueryModel.NotifyPropertyChanged("Everything");
            System.AppDomain.CurrentDomain.ProcessExit += (object sender, EventArgs e) =>
            {
                Console.Write("Snapshoting event number {0}...", LightBulbQueryModel.EventNumber);
                MongoLightBulbs.FindOneAndReplace<QueryModel>(
                    MongoFilter,
                    LightBulbQueryModel
                );
                Console.WriteLine(" Success!");
            };

            using(var connection = EventStoreConnection.Create(
                ConnectionSettings.Create().UseConsoleLogger().FailOnNoServerResponse().LimitReconnectionsTo(0),
                new Uri("tcp://eventstore:1113")
            ))
            {
            
                await connection.ConnectAsync();

                LightBulbQueryModel.LinkTo(connection, LightBulbIdentifier);
                
                while(true);
            
            }
          
        }
    }
}