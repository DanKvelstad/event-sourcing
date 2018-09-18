using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using SmartHome.Events;

namespace SmartHome
{

    public class RemoteControlCommandModel
    {

        public Guid Identifier
        {
            get;
            set;
        }

        public string StreamName
        {
            get => streamName ?? (streamName = "remote_control-" + Identifier.ToString("N"));
        }
        private string streamName;

        public IEventStoreConnection Connection
        {
            set;
            get;
        }

        public async void DoIncrementBrightness()
        {
            await Connection.AppendToStreamAsync(
                StreamName,
                ExpectedVersion.Any, 
                new EventData(
                    Guid.NewGuid(),
                    nameof(Events.BrightnessChanged),
                    true,
                    Encoding.UTF8.GetBytes(
                        JsonConvert.SerializeObject(
                            new Events.BrightnessChanged()
                            {
                                Identifier = Identifier,
                                Change     = 1
                            }
                        )
                    ),
                    Encoding.UTF8.GetBytes(
                        JsonConvert.SerializeObject(
                            new Events.GeneralMetaData()
                            {
                                Name      = "Dan"
                            }
                        )
                    )
                )
            );
        }

        public async void DoDecrementBrightness()
        {
            
            await Connection.AppendToStreamAsync(
                StreamName,
                ExpectedVersion.Any, 
                new EventData(
                    Guid.NewGuid(),
                    nameof(Events.BrightnessChanged),
                    true,
                    Encoding.UTF8.GetBytes(
                        JsonConvert.SerializeObject(
                            new Events.BrightnessChanged()
                            {
                                Identifier = Identifier,
                                Change     = -1
                            }
                        )
                    ),
                    Encoding.UTF8.GetBytes(
                        JsonConvert.SerializeObject(
                            new Events.GeneralMetaData()
                            {
                                Name      = "Dan"
                            }
                        )
                    )
                )
            );
        }

        public async void DoIncrementColor()
        {
            await Connection.AppendToStreamAsync(
                StreamName,
                ExpectedVersion.Any, 
                new EventData(
                    Guid.NewGuid(),
                    nameof(Events.ColorChanged),
                    true,
                    Encoding.UTF8.GetBytes(
                        JsonConvert.SerializeObject(
                            new Events.ColorChanged()
                            {
                                Identifier = Identifier,
                                Change     = 1
                            }
                        )
                    ),
                    Encoding.UTF8.GetBytes(
                        JsonConvert.SerializeObject(
                            new Events.GeneralMetaData()
                            {
                                Name      = "Dan"
                            }
                        )
                    )
                )
            );
        }

        public async void DoDecrementColor()
        {
            await Connection.AppendToStreamAsync(
                StreamName,
                ExpectedVersion.Any, 
                new EventData(
                    Guid.NewGuid(),
                    nameof(Events.ColorChanged),
                    true,
                    Encoding.UTF8.GetBytes(
                        JsonConvert.SerializeObject(
                            new Events.ColorChanged()
                            {
                                Identifier = Identifier,
                                Change     = -1
                            }
                        )
                    ),
                    Encoding.UTF8.GetBytes(
                        JsonConvert.SerializeObject(
                            new Events.GeneralMetaData()
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