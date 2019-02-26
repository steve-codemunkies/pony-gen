using System;

namespace Pony.Generator.Console
{
    public class PonyTalk : PonyActivity
    {
        public string Speech { get; set; }

        public override PonyBaseEvent CreateEvent(string name, DateTime timestamp)
        {
            return new PonyTalkEvent{Name = name, Speech = Speech, Timestamp = timestamp};
        }
    }
}
