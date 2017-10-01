// (c) Copyright 2016 Hewlett Packard Enterprise Development LP

// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.

// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,

// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

// See the License for the specific language governing permissions and limitations under the License.

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
        
        protected static RestConnector restConnector = new RestConnector();
        protected static EntityService entityService = new EntityService(restConnector);

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
                ConnectionInfo connectionInfo;
                string clientId = ConfigurationManager.AppSettings["clientId"];
                if (clientId != null)
                {
                    userName = clientId;
                    connectionInfo = new APIKeyConnectionInfo(clientId, ConfigurationManager.AppSettings["clientSecret"]);
                } else
                {
                    userName = ConfigurationManager.AppSettings["userName"];
                    connectionInfo = new UserPassConnectionInfo(ConfigurationManager.AppSettings["userName"], ConfigurationManager.AppSettings["password"]);
                }
                
                restConnector.Connect(host, connectionInfo);


                sharedSpaceId = int.Parse(ConfigurationManager.AppSettings["sharedSpaceId"]);
                workspaceId = int.Parse(ConfigurationManager.AppSettings["workspaceId"]);

                workspaceContext = new WorkspaceContext(sharedSpaceId, workspaceId);
                sharedSpaceContext = new SharedSpaceContext(sharedSpaceId);
            }
        }
    }
}
