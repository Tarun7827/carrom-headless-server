using CarromHeadless.Core;
using NUnit.Framework;

namespace CarromHeadless.Tests
{
    public sealed class BoardLifecycleTests
    {
        [Test]
        public void LifecycleContainsExpectedExclusiveStates()
        {
            Assert.That((int)BoardLifecycle.Available, Is.Not.EqualTo((int)BoardLifecycle.Simulating));
            Assert.That((int)BoardLifecycle.Reserved, Is.Not.EqualTo((int)BoardLifecycle.Available));
            Assert.That((int)BoardLifecycle.Resetting, Is.Not.EqualTo((int)BoardLifecycle.Simulating));
        }
    }
}
