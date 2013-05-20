using NUnit.Framework;
using NeuroNet.Model.FuzzyNumbers;
using NeuroNet.Model.Net;

namespace NeuroNet.Model.Tests.Net
{
    [TestFixture]
    public class SimpleFuzzyNetShould
    {
        [Test]
        public void CorrectlyReturnNumberOfWeights()
        {
            const int inputs = 5;
            const int hidden = 7;
            const int output = 1;
            var net = new SimpleFuzzyNet(inputs, hidden, RealNumber.GenerateLittleNumber);

            var weightsCount = net.WeightsCount;

            Assert.That(weightsCount, Is.EqualTo(inputs * hidden + hidden * output));
        }
    }
}