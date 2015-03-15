using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Spike1;
using SpikeShared;

namespace TestProject
{
    [TestClass()]
    public class TestSpike1
    {
        private Mock<ThingManager> mockThingManager;
        private Thing parent;

        [TestInitialize()]
        public void Setup()
        {
            this.parent = new Thing();
            this.mockThingManager = new Mock<ThingManager>();
        }

        [TestMethod()]
        public void LooksForNearByThingsWhenHunting()
        {
            this.mockThingManager.Setup(m => m.FindThingsNear(parent)).Returns(new List<Thing>());

            var grue = new GrueBehavior(mockThingManager.Object) { Parent = parent };
            grue.Hunt();
            this.mockThingManager.Verify(m => m.FindThingsNear(parent));
        }
    }
}