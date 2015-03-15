using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Spike2;
using SpikeShared;
using System.Reflection;

namespace TestProject
{
    public static class TestHelpers
    {
        public static Mock<T> MockSingletonInstance<T>()
            where T : class
        {
            var mock = new Mock<T>();
            var instance = typeof(T).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic);
            instance.SetValue(null, mock.Object);
            return mock;
        }
    }

    [TestClass()]
    public class TestSpike2
    {
        private Mock<ThingManager> mockThingManager;
        private Thing parent;

        [TestInitialize()]
        public void Setup()
        {
            parent = new Thing();
            this.mockThingManager = TestHelpers.MockSingletonInstance<ThingManager>();
        }

        [TestMethod()]
        public void LooksForNearByThingsWhenHunting()
        {
            this.mockThingManager.Setup(m => m.FindThingsNear(parent)).Returns(new List<Thing>());
            var grue = new GrueBehavior() { Parent = parent };
            grue.Hunt();
            this.mockThingManager.Verify(m => m.FindThingsNear(parent));
        }
    }
}