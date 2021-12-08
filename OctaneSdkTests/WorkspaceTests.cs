/*!
* (c) Copyright 2021 Micro Focus or one of its affiliates.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using MicroFocus.Adm.Octane.Api.Core.Entities;
using MicroFocus.Adm.Octane.Api.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MicroFocus.Adm.Octane.Api.Core.Tests
{
    [TestClass]
    public class WorkspaceTests : BaseTest
    {
        [TestCategory("LongTest")]
        [TestMethod]
        public void CreateWorkspace()
        {
            Workspace workspace = new Workspace();
            workspace.Name = "WS_" + Guid.NewGuid();

            Workspace created = entityService.Create<Workspace>(sharedSpaceContext, workspace, TestHelper.NameFields);

            Assert.AreEqual<string>(workspace.Name, created.Name);

        }

        [TestMethod]
        public void GetAllWorkspacesForCurrentUser()
        {
            EntityListResult<Workspace> workspaces = entityService.Get<Workspace>(sharedSpaceContext, null, null);
            foreach (Workspace workspace in workspaces.data)
            {
                try
                {
                    long workspaceId = long.Parse((string)workspace.Id);
                    EntityListResult<WorkspaceUser> workspace_users = entityService.Get<WorkspaceUser>(new Services.RequestContext.WorkspaceContext(sharedSpaceContext.SharedSpaceId, workspaceId));
                    foreach (WorkspaceUser workspaceUser in workspace_users.data)
                    {
                        if (workspaceUser.Email.Equals(CurrentUserName))
                        {
                            System.Diagnostics.Trace.WriteLine(workspace.Id);
                        }
                    }
                }
                catch (Exception)
                {
                    // do nothing
                }
            }

        }

    }
}
