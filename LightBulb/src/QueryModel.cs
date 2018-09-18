using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using MongoDB;

namespace SmartHome
{

    public class QueryModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public void LinkTo(IEventStoreConnection connection, Guid identifier)
        {
            connection.SubscribeToStreamFrom(
                "remote_control-" + Identifier.ToString("N"), 
                EventNumber,
                CatchUpSubscriptionSettings.Default, 
                (EventStoreCatchUpSubscription EventStoreCatchUpSubscription, ResolvedEvent ResolvedEvent) =>
                {
                    switch(ResolvedEvent.Event.EventType)
                    {
                    case nameof(Events.BrightnessChanged):
                        var BrightnessChanged = JsonConvert.DeserializeObject(
                            Encoding.UTF8.GetString(ResolvedEvent.Event.Data),
                            typeof(Events.BrightnessChanged)
                        ) as Events.BrightnessChanged;
                        Brightness  += BrightnessChanged.Change;
                        EventNumber  = ResolvedEvent.Event.EventNumber;
                        break;
                    case nameof(Events.ColorChanged):
                        var ColorChanged = JsonConvert.DeserializeObject(
                            Encoding.UTF8.GetString(ResolvedEvent.Event.Data),
                            typeof(Events.ColorChanged)
                        ) as Events.ColorChanged;
                        Color       += ColorChanged.Change;
                        EventNumber  = ResolvedEvent.Event.EventNumber;
                        break;
                    }
                },
                (EventStoreCatchUpSubscription EventStoreCatchUpSubscription) =>
                {
                    
                },
                (EventStoreCatchUpSubscription EventStoreCatchUpSubscription, SubscriptionDropReason SubscriptionDropReason, Exception Exception) =>
                {
                    Console.WriteLine(
                        "The subscription was dropped because of: {0}",
                        SubscriptionDropReason
                    );
                    NotifyPropertyChanged(null);
                },
                null
            );
        }

        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public Guid Identifier
        {
            get;
            set;
        }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("event_number")]
        public long? EventNumber
        {
            set;
            get;
        }

        private int brightness;
        [MongoDB.Bson.Serialization.Attributes.BsonElement("brightness")]
        public int Brightness
        {
            set
            {
                if (value!=brightness && 0<=value && 10>value)
                {
                    brightness = value;
                    NotifyPropertyChanged();
                }
            }
            get => brightness;
        }
        
        private int color;
        [MongoDB.Bson.Serialization.Attributes.BsonElement("color")]
        public int Color
        {
            set
            {
                if (value!=brightness && 0<=value && 3>value)
                {
                    color = value;
                    NotifyPropertyChanged();
                }
            }
            get => color;
        }

    }

}