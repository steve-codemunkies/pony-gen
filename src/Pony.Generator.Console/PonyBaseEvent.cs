using System;

namespace Pony.Generator.Console
{
    public abstract class PonyBaseEvent
    {
        public string Name { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
