using System;

namespace Pony.Generator.Console
{
    public abstract class PonyActivity : WeightedItem
    {
        public abstract PonyBaseEvent CreateEvent(string name, DateTime timestamp);
    }
}
