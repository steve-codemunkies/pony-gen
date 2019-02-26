using System.Collections.Generic;

namespace Pony.Generator.Console
{
    public class WeightedPony : WeightedItem
    {
        public string Name { get; set; }
        public IEnumerable<WeightedActivity> Activities { get; set; }
    }
}
