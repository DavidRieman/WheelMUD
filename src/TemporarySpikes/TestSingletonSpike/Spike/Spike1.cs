using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpikeShared;

namespace Spike1
{
    public class ThingManager : ICanFindNearByThings
    {
        private static ThingManager instance = new ThingManager();

        public static ThingManager Instance
        {
            get { return instance; }
        }

        public virtual List<Thing> FindThingsNear(Thing thing)
        {
            throw new NotImplementedException();
        }
    }

    public class GrueBehavior : Behavior
    {
        private ThingManager thingManager;

        public GrueBehavior(ThingManager thingManager)
        {
            this.thingManager = thingManager;
        }

        public void Hunt()
        {
            var potentialTargets = thingManager.FindThingsNear(this.Parent);
        }
    }
}
