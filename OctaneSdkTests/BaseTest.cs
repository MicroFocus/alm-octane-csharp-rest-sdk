/*
 * Copyright 2016-2026 Open Text.
 *
 * The only warranties for products and services of Open Text and
 * its affiliates and licensors (“Open Text”) are as may be set forth
 * in the express warranty statements accompanying such products and services.
 * Nothing herein should be construed as constituting an additional warranty.
 * Open Text shall not be liable for technical or editorial errors or
 * omissions contained herein. The information contained herein is subject
 * to change without notice.
 *
 * Except as specifically indicated otherwise, this document contains
 * confidential information and a valid license is required for possession,
 * use or copying. If this work is provided to the U.S. Government,
 * consistent with FAR 12.211 and 12.212, Commercial Computer Software,
 * Computer Software Documentation, and Technical Data for Commercial Items are
 * licensed to the U.S. Government under vendor's standard commercial license.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *   http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


using MicroFocus.Adm.Octane.Api.Core.Connector;
using MicroFocus.Adm.Octane.Api.Core.Connector.Authentication;
using MicroFocus.Adm.Octane.Api.Core.Services;
using MicroFocus.Adm.Octane.Api.Core.Services.RequestContext;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;

namespace MicroFocus.Adm.Octane.Api.Core.Tests
{

    [TestClass]
    public class BaseTest
    {

        protected static RestConnector restConnector = new RestConnector();
        protected static EntityService entityService = new EntityService(restConnector);

        protected static string userName;
        protected static string password;
        protected static string host;

        protected static LwssoAuthenticationStrategy lwssoAuthenticationStrategy;

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

                string ignoreServerCertificateValidation = ConfigurationManager.AppSettings["ignoreServerCertificateValidation"];
                if (ignoreServerCertificateValidation != null && ignoreServerCertificateValidation.ToLower().Equals("true"))
                {
                    NetworkSettings.IgnoreServerCertificateValidation();
                }
                NetworkSettings.EnableAllSecurityProtocols();

                host = ConfigurationManager.AppSettings["webAppUrl"];

                // If webAppUrl is empty we do not try to connect.
                if (string.IsNullOrWhiteSpace(host)) return;

                ConnectionInfo connectionInfo;
                string clientId = ConfigurationManager.AppSettings["clientId"];
                if (clientId != null)
                {
                    userName = clientId;
                    connectionInfo = new APIKeyConnectionInfo(clientId, ConfigurationManager.AppSettings["clientSecret"]);
                }
                else
                {
                    userName = ConfigurationManager.AppSettings["userName"];
                    password = ConfigurationManager.AppSettings["password"];
                    connectionInfo = new UserPassConnectionInfo(userName, password);
                }

                lwssoAuthenticationStrategy = new LwssoAuthenticationStrategy(connectionInfo);
                restConnector.Connect(host, lwssoAuthenticationStrategy);


                var sharedSpaceId = int.Parse(ConfigurationManager.AppSettings["sharedSpaceId"]);
                var workspaceId = int.Parse(ConfigurationManager.AppSettings["workspaceId"]);

                workspaceContext = new WorkspaceContext(sharedSpaceId, workspaceId);
                sharedSpaceContext = new SharedSpaceContext(sharedSpaceId);
            }
        }
    }
}
