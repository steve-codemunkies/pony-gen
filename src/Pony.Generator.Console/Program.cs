using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Pony.Generator.Console
{
    public class Program
    {
        public static Task<int> Main(string[] args) => new HostBuilder()
            .UseConsoleLifetime()
            .RunCommandLineApplicationAsync<Program>(args);

        [Option("-n|--number-of-ponies", CommandOptionType.SingleValue)]
        public int Number { get; } = 10000;

        private readonly IConsole _console;
        private readonly Random _random;
        private DateTime _date = DateTime.MinValue;

        private IEnumerable<WeightedPony> _ponies = new [] {
            new WeightedPony{
                Name = "Twilight Sparkle",
                Weight = 6000,
                Activities = new [] {
                    new WeightedActivity {
                        Weight = 1000,
                        PonysActivities = new [] {
                            new PonyAction { Action = "Nuzzle", Weight = 1000 },
                            new PonyAction { Action = "Find book", Weight = 2500 },
                            new PonyAction { Action = "Try flying", Weight = 500}
                        }
                    },
                    new WeightedActivity {
                        Weight = 10000,
                        PonysActivities = new [] {
                            new PonyTalk { Speech = "Friendship is magic", Weight = 3000 },
                            new PonyTalk { Speech = "We must get organised!", Weight = 2000 },
                            new PonyTalk { Speech = "I can practice flying tomorrow!", Weight = 4000 },
                            new PonyTalk { Speech = "What?!", Weight = 1000 }
                        }
                    }
                }
            },
            new WeightedPony{
                Name = "Pinkie Pie",
                Weight = 1000,
                Activities = new [] {
                    new WeightedActivity {
                        Weight = 1000,
                        PonysActivities = new [] {
                            new PonyAction { Action = "Fire confetti canon", Weight = 1000 },
                            new PonyAction { Action = "Jump", Weight = 1000 },
                            new PonyAction { Action = "Pie splat", Weight = 1000}
                        }
                    },
                    new WeightedActivity {
                        Weight = 1000,
                        PonysActivities = new [] {
                            new PonyTalk { Speech = "Party!", Weight = 1000 },
                            new PonyTalk { Speech = "Surprise!", Weight = 1000 },
                            new PonyTalk { Speech = "Cheer up!", Weight = 1000 }
                        }
                    }
                }
            },
            new WeightedPony{
                Name = "Applejack",
                Weight = 1000,
                Activities = new [] {
                    new WeightedActivity {
                        Weight = 1000,
                        PonysActivities = new [] {
                            new PonyAction { Action = "Farming", Weight = 1000 },
                            new PonyAction { Action = "Jobs", Weight = 1000 },
                            new PonyAction { Action = "Apple picking", Weight = 1000}
                        }
                    },
                    new WeightedActivity {
                        Weight = 1000,
                        PonysActivities = new [] {
                            new PonyTalk { Speech = "We'll get through this job!", Weight = 1000 },
                            new PonyTalk { Speech = "Be sensible!", Weight = 1000 },
                            new PonyTalk { Speech = "I ain't wearing that!", Weight = 1000 }
                        }
                    }
                }
            },
            new WeightedPony{
                Name = "Rarity",
                Weight = 1000,
                Activities = new [] {
                    new WeightedActivity {
                        Weight = 1000,
                        PonysActivities = new [] {
                            new PonyAction { Action = "Dressmaking", Weight = 1000 },
                            new PonyAction { Action = "Fabric selection", Weight = 1000 },
                            new PonyAction { Action = "Organisation", Weight = 1000}
                        }
                    },
                    new WeightedActivity {
                        Weight = 1000,
                        PonysActivities = new [] {
                            new PonyTalk { Speech = "Fabulous!", Weight = 1000 },
                            new PonyTalk { Speech = "Ugh no!", Weight = 1000 },
                            new PonyTalk { Speech = "But Applejack you look adorable!", Weight = 1000 }
                        }
                    }
                }
            },
            new WeightedPony{
                Name = "Rainbow Dash",
                Weight = 6000,
                Activities = new [] {
                    new WeightedActivity {
                        Weight = 10000,
                        PonysActivities = new [] {
                            new PonyAction { Action = "Practicing flying", Weight = 1000 },
                            new PonyAction { Action = "Flying", Weight = 1000 },
                            new PonyAction { Action = "Cloud bashing", Weight = 1000}
                        }
                    },
                    new WeightedActivity {
                        Weight = 1000,
                        PonysActivities = new [] {
                            new PonyTalk { Speech = "You can do it!", Weight = 1000 },
                            new PonyTalk { Speech = "But Twilight you need to practice!", Weight = 1000 }
                        }
                    }
                }
            },
            new WeightedPony{
                Name = "Fluttershy",
                Weight = 1000,
                Activities = new [] {
                    new WeightedActivity {
                        Weight = 1000,
                        PonysActivities = new [] {
                            new PonyAction { Action = "Caring for sick animals", Weight = 1000 },
                            new PonyAction { Action = "Growing plants", Weight = 1000 },
                            new PonyAction { Action = "Making peace", Weight = 1000}
                        }
                    },
                    new WeightedActivity {
                        Weight = 1000,
                        PonysActivities = new [] {
                            new PonyTalk { Speech = "Oh no!", Weight = 1000 },
                            new PonyTalk { Speech = "You poor thing!", Weight = 1000 },
                            new PonyTalk { Speech = "We should be friends!", Weight = 1000 }
                        }
                    }
                }
            }
        };

        public Program(IConsole console)
        {
            _console = console;
            _random = new Random((int)DateTime.UtcNow.Ticks);
        }

        public async Task<int> OnExecuteAsync()
        {
            _console.WriteLine($"Creating {Number} ponies");

            _date = DateTime.Now.AddSeconds(Number * -1);

            using (var connection = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"), "Pony.Generator.Console"))
            {
                await connection.ConnectAsync();

                for(var i = 0; i < Number; i++)
                {
                    await connection.AppendToStreamAsync("Ponies", ExpectedVersion.Any, GeneratePonyEventData());
                    _date = _date.AddSeconds(1);

                    if (i % 100 == 0)
                    {
                        _console.WriteLine($"Created {i} ponies");
                    }
                }
            }

            return Number;
        }

        private EventData GeneratePonyEventData()
        {
            var ponyEvent = GenerateRandomPonyEvent();
            var serializedEvent = JsonConvert.SerializeObject(ponyEvent);
            return new EventData(Guid.NewGuid(), ponyEvent.GetType().Name, true, Encoding.UTF8.GetBytes(serializedEvent), null);
        }

        private PonyBaseEvent GenerateRandomPonyEvent()
        {
            var pony = (WeightedPony) GetWeightedItem(_ponies);
            var activityType = (WeightedActivity) GetWeightedItem(pony.Activities);
            var activity = (PonyActivity) GetWeightedItem(activityType.PonysActivities);

            return activity.CreateEvent(pony.Name, _date);
        }

        private WeightedItem GetWeightedItem(IEnumerable<WeightedItem> items)
        {
            var total = items.Sum(wi => wi.Weight);
            var position = (int)(_random.NextDouble() * total);

            foreach(var item in items)
            {
                position -= item.Weight;

                if (position <= 0)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
