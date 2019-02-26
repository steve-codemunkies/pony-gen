using System;

namespace Pony.Generator.Console
{
    public class PonyAction : PonyActivity
    {
        public string Action { get; set; }

        public override PonyBaseEvent CreateEvent(string name, DateTime timestamp)
        {
            return new PonyActivityEvent{Action = Action, Timestamp = timestamp, Name = name};
        }
    }
}
