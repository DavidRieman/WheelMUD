using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpikeShared
{
    public interface ICanFindNearByThings
    {
        List<Thing> FindThingsNear(Thing thing);
    }

    public class Thing { }

    public class Behavior
    {
        public Thing Parent { get; set; }
    }

}
