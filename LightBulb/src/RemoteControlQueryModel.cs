using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace SmartHome
{

    public class RemoteControlQueryModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public RemoteControlQueryModel(IEventStoreConnection connection, Guid identifier)
        {

            StreamName = "remote_control-" + identifier.ToString("N");
            
            connection.SubscribeToStreamAsync(
                StreamName, 
                false, 
                (EventStoreSubscription EventStoreSubscription, ResolvedEvent ResolvedEvent) => 
                {
                    switch(ResolvedEvent.Event.EventType)
                    {
                    case nameof(Events.BrightnessChanged):
                        var BrightnessChanged = JsonConvert.DeserializeObject(
                            Encoding.UTF8.GetString(ResolvedEvent.Event.Data),
                            typeof(Events.BrightnessChanged)
                        ) as Events.BrightnessChanged;
                        Brightness += BrightnessChanged.Change;
                        break;
                    case nameof(Events.ColorChanged):
                        var ColorChanged = JsonConvert.DeserializeObject(
                            Encoding.UTF8.GetString(ResolvedEvent.Event.Data),
                            typeof(Events.ColorChanged)
                        ) as Events.ColorChanged;
                        Color += ColorChanged.Change;
                        break;
                    }
                },
                (EventStoreSubscription EventStoreSubscription, SubscriptionDropReason SubscriptionDropReason, Exception Exception) =>
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

        private string StreamName;

        private int brightness;
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