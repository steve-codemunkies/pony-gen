using System.Collections.Generic;

namespace Pony.Generator.Console
{
    public class WeightedActivity : WeightedItem
    {
        public IEnumerable<PonyActivity> PonysActivities { get; set; }
    }
}
