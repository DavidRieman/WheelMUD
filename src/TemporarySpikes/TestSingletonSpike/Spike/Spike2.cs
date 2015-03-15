using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpikeShared;

namespace Spike2
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
        public GrueBehavior()
        {
        }

        public void Hunt()
        {
            var potentialTargets = ThingManager.Instance.FindThingsNear(this.Parent);
        }
    }
}