using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hpe.Nga.Api.Core.Tests
{
    [TestClass]
    public class ConnectionTests : BaseTest
    {
        [TestMethod]
        public void ConnectionDisconnectionTest()
        {
            //we are connected here
            Assert.IsTrue(restConnector.IsConnected());

            restConnector.Disconnect();
            Assert.IsFalse(restConnector.IsConnected());

            InitConnection(null);
            Assert.IsTrue(restConnector.IsConnected());
        }
    }
}
