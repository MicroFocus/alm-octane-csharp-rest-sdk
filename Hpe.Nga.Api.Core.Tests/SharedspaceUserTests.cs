/*!
* (c) 2016-2018 EntIT Software LLC, a Micro Focus company
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
using MicroFocus.Adm.Octane.Api.Core.Services.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroFocus.Adm.Octane.Api.Core.Tests
{
	/// <summary>
	/// This test should be executed by suer with role of sharedSpace admin
	/// </summary>
	[TestClass]
    public class SharedspaceUserTests : BaseTest
    {

        [TestMethod]
        public void CreateUserTest()
        {
            CreateUser();
        }

        [TestMethod]
        public void GetAllSharedspaceUsersTest()
        {
            EntityListResult<SharedspaceUser> users = entityService.Get<SharedspaceUser>(sharedSpaceContext, null, null);
            Assert.IsTrue(users.total_count >= 1);

        }

        [TestMethod]
        public void GetWorkspaceAdminRoleTest()
        {
            LogicalQueryPhrase logicalNamePhrase = new LogicalQueryPhrase(Role.LOGICAL_NAME_FIELD, "role.workspace.admin");
            CrossQueryPhrase byRole = new CrossQueryPhrase(WorkspaceRole.ROLE_FIELD, logicalNamePhrase);

            LogicalQueryPhrase workspaceIdPhrase = new LogicalQueryPhrase(Workspace.ID_FIELD, workspaceContext.WorkspaceId);
            CrossQueryPhrase byWorkpace = new CrossQueryPhrase(WorkspaceRole.WORKSPACE_FIELD, workspaceIdPhrase);

            List<QueryPhrase> queries = new List<QueryPhrase>();
            queries.Add(byWorkpace);
            queries.Add(byRole);
            EntityListResult<WorkspaceRole> roles = entityService.Get<WorkspaceRole>(sharedSpaceContext, queries, null);
            WorkspaceRole workspaceAdminRole = roles.data[0];
        }

        [TestMethod]
        public void AssignSharedSpaceAdminRoleToUserTest()
        {

            //FIND shared space admin role
            LogicalQueryPhrase roleLogicalName = new LogicalQueryPhrase(Role.LOGICAL_NAME_FIELD, "role.shared.space.admin");
            CrossQueryPhrase byRole = new CrossQueryPhrase(WorkspaceRole.ROLE_FIELD, roleLogicalName);
            List<QueryPhrase> queries = new List<QueryPhrase>();
            queries.Add(byRole);
            EntityListResult<WorkspaceRole> roles = entityService.Get<WorkspaceRole>(sharedSpaceContext, queries, null);
            Assert.AreEqual<int>(1, roles.total_count.Value);
            WorkspaceRole sharedSpaceAdminRole = roles.data[0];

            //CREATE USER
            SharedspaceUser createdUser = CreateUser();

            //UPDATE USER by adding shared space admin role
            SharedspaceUser userForUpdate = new SharedspaceUser(createdUser.Id);
            userForUpdate.WorkspaceRoles = createdUser.WorkspaceRoles;
            userForUpdate.WorkspaceRoles.data.Add(sharedSpaceAdminRole);
            entityService.Update<SharedspaceUser>(sharedSpaceContext, userForUpdate);

            //READ USER
            List<String> fields = new List<string> { SharedspaceUser.NAME_FIELD, SharedspaceUser.WORKSPACE_ROLES_FIELD };
            SharedspaceUser updatedUser = entityService.GetById<SharedspaceUser>(sharedSpaceContext, createdUser.Id, fields);

            List<EntityId> assignedWorkspaceRoles = updatedUser.WorkspaceRoles.data.Select(p => p.Id).ToList();
            Assert.IsTrue(assignedWorkspaceRoles.Contains(sharedSpaceAdminRole.Id));
        }

        [TestMethod]
        public void AppendSharedSpaceAdminRoleToUserTest()
        {

            //FIND shared space admin role
            LogicalQueryPhrase roleLogicalName = new LogicalQueryPhrase(Role.LOGICAL_NAME_FIELD, "role.shared.space.admin");
            CrossQueryPhrase byRole = new CrossQueryPhrase(WorkspaceRole.ROLE_FIELD, roleLogicalName);
            List<QueryPhrase> queries = new List<QueryPhrase>();
            queries.Add(byRole);
            EntityListResult<WorkspaceRole> roles = entityService.Get<WorkspaceRole>(sharedSpaceContext, queries, null);
            Assert.AreEqual<int>(1, roles.total_count.Value);
            WorkspaceRole sharedSpaceAdminRole = roles.data[0];

            //CREATE USER
            SharedspaceUser createdUser = CreateUser();

            //UPDATE USER by appending shared space admin role
            SharedspaceUser userForUpdate = new SharedspaceUser(createdUser.Id);
            userForUpdate.WorkspaceRoles = new EntityList<BaseEntity>();
            userForUpdate.WorkspaceRoles.data.Add(sharedSpaceAdminRole);
            Dictionary<String, String> serviceArgs = new Dictionary<string, string>();
            serviceArgs.Add("reference_update_mode", "append");

            entityService.Update<SharedspaceUser>(sharedSpaceContext, userForUpdate, serviceArgs, null);

            //READ USER
            List<String> fields = new List<string> { SharedspaceUser.NAME_FIELD, SharedspaceUser.WORKSPACE_ROLES_FIELD };
            SharedspaceUser updatedUser = entityService.GetById<SharedspaceUser>(sharedSpaceContext, createdUser.Id, fields);

            List<EntityId> assignedWorkspaceRoles = updatedUser.WorkspaceRoles.data.Select(p => p.Id).ToList();
            Assert.IsTrue(assignedWorkspaceRoles.Contains(sharedSpaceAdminRole.Id));
            Assert.IsTrue(assignedWorkspaceRoles.Count > 1);
        }

        private static SharedspaceUser CreateUser()
        {
            SharedspaceUser user = new SharedspaceUser();
            user.Name = "Name" + Guid.NewGuid() + "@hpe.com";
            user.Password = "Welcome1";
            user.LastName = "Last name";
            user.FirstName = "First name";
            user.Email = user.Name;
            user.Phone1 = "123-123-123";

            var fields = new string[] { "name", "workspace_roles" };

            SharedspaceUser createdUser = entityService.Create<SharedspaceUser>(sharedSpaceContext, user, fields);
            Assert.AreEqual<string>(user.Name, createdUser.Name);
            return createdUser;
        }
    }
}
