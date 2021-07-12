using JetBrains.dotMemoryUnit;
using JetBrains.dotMemoryUnit.Kernel;
using NUnit.Framework;

namespace Svg.UnitTests
{
    [DotMemoryUnit(FailIfRunWithoutSupport=false, CollectAllocations=true)]
    [TestFixture]
    public class SvgElementMemoryUsageTests
    {
        [Test]
        public void EmptyClass_new_size()
        {
            if (!dotMemoryApi.IsEnabled)
            {
                return;
            }

            var snap1 = dotMemoryApi.GetSnapshot();

            var result = new EmptyClass();

            var snap2 = dotMemoryApi.GetSnapshot();
            var traffic = dotMemoryApi.GetTrafficBetween(snap1, snap2);
            var o = traffic.Where(w => w.Type.Is<EmptyClass>());
            Assert.AreEqual(1, o.AllocatedMemory.ObjectsCount);
            Assert.AreEqual(24, o.AllocatedMemory.SizeInBytes);
        }

        [Test]
        public void SvgElementEmpty_new_size()
        {
            if (!dotMemoryApi.IsEnabled)
            {
                return;
            }

            var snap1 = dotMemoryApi.GetSnapshot();

            var result = new SvgElementEmpty();

            var snap2 = dotMemoryApi.GetSnapshot();
            var traffic = dotMemoryApi.GetTrafficBetween(snap1, snap2);

            var o = traffic.Where(w => w.Type.Is<SvgElementEmpty>());
            Assert.AreEqual(1, o.AllocatedMemory.ObjectsCount);
            Assert.AreEqual(200, o.AllocatedMemory.SizeInBytes);
        }
        
        [Test]
        public void SvgElementEmpty_new()
        {
            var result = new SvgElementEmpty();
            Assert.NotNull(result);
        }
    }
}
