using System.Configuration;
using Hpe.Nga.Api.Core.Connector;
using Hpe.Nga.Api.Core.Services;
using Hpe.Nga.Api.Core.Services.RequestContext;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hpe.Nga.Api.Core.Tests
{

    [TestClass]
    public class BaseTest
    {
        protected static EntityService entityService = EntityService.GetInstance();
        protected static RestConnector restConnector = RestConnector.GetInstance();

        private static int sharedSpaceId;
        private static int workspaceId;
        private static string userName;

        protected string CurrentUserName
        {
            get { return BaseTest.userName; }
        }

        protected static WorkspaceContext workspaceContext;
        protected static SharedSpaceContext sharedSpaceContext;

        [AssemblyInitialize]
        public static void InitConnection(TestContext context)
        {
            if (!restConnector.IsConnected())
            {
                string host = ConfigurationManager.AppSettings["webAppUrl"];
                string password = ConfigurationManager.AppSettings["password"];
                userName = ConfigurationManager.AppSettings["userName"];
                restConnector.Connect(host, userName, password);


                sharedSpaceId = int.Parse(ConfigurationManager.AppSettings["sharedSpaceId"]);
                workspaceId = int.Parse(ConfigurationManager.AppSettings["workspaceId"]);

                workspaceContext = new WorkspaceContext(sharedSpaceId, workspaceId);
                sharedSpaceContext = new SharedSpaceContext(sharedSpaceId);
            }
        }
    }
}
