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
		public void CreateUserWithRoleTest()
		{
			SharedspaceUser user = CreateUser(GetWorkspaceAdminRole());
			Assert.AreEqual(WorkspaceRole.WORKSPACE_ADMIN_ROLE_LOGICAL_NAME, user.WorkspaceRoles.data[0].Role.LogicalName);
		}


		[TestMethod]
        public void GetAllSharedspaceUsersTest()
        {
            EntityListResult<SharedspaceUser> users = entityService.Get<SharedspaceUser>(sharedSpaceContext, null, null);
            Assert.IsTrue(users.total_count >= 1);

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
            entityService.Update(sharedSpaceContext, userForUpdate);

            //READ USER
            List<string> fields = new List<string> { SharedspaceUser.NAME_FIELD, SharedspaceUser.WORKSPACE_ROLES_FIELD };
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
            userForUpdate.WorkspaceRoles = new EntityList<WorkspaceRole>();
            userForUpdate.WorkspaceRoles.data.Add(sharedSpaceAdminRole);
            Dictionary<string, string> serviceArgs = new Dictionary<string, string>();
            serviceArgs.Add("reference_update_mode", "append");

            entityService.Update(sharedSpaceContext, userForUpdate, serviceArgs, null);

            //READ USER
            List<string> fields = new List<string> { SharedspaceUser.NAME_FIELD, SharedspaceUser.WORKSPACE_ROLES_FIELD };
            SharedspaceUser updatedUser = entityService.GetById<SharedspaceUser>(sharedSpaceContext, createdUser.Id, fields);

            List<EntityId> assignedWorkspaceRoles = updatedUser.WorkspaceRoles.data.Select(p => p.Id).ToList();
            Assert.IsTrue(assignedWorkspaceRoles.Contains(sharedSpaceAdminRole.Id));
            Assert.IsTrue(assignedWorkspaceRoles.Count > 1);
        }

        private static SharedspaceUser CreateUser(WorkspaceRole role = null)
        {
            SharedspaceUser user = new SharedspaceUser();
            user.Name = "Name" + Guid.NewGuid() + "@hpe.com";
            user.Password = "Welcome1";
            user.LastName = "Last name";
            user.FirstName = "First name";
            user.Email = user.Name;
            user.Phone1 = "123-123-123";

			if (role != null)
			{
				user.WorkspaceRoles = EntityList<WorkspaceRole>.Create(role);
			}

            var fields = new string[] { "name", "workspace_roles" };

            SharedspaceUser createdUser = entityService.Create<SharedspaceUser>(sharedSpaceContext, user, fields);
            Assert.AreEqual<string>(user.Name, createdUser.Name);
            return createdUser;
        }

		[TestMethod]
		public void GetWorkspaceAdminRoleTest()
		{
			GetWorkspaceAdminRole();

		}

		private static WorkspaceRole workspaceAdminRole;
		public static WorkspaceRole GetWorkspaceAdminRole()
		{
			if (workspaceAdminRole == null)
			{
				
				LogicalQueryPhrase logicalNamePhrase = new LogicalQueryPhrase(Role.LOGICAL_NAME_FIELD, WorkspaceRole.WORKSPACE_ADMIN_ROLE_LOGICAL_NAME);
				CrossQueryPhrase byRole = new CrossQueryPhrase(WorkspaceRole.ROLE_FIELD, logicalNamePhrase);

				LogicalQueryPhrase workspaceIdPhrase = new LogicalQueryPhrase(Workspace.ID_FIELD, workspaceContext.WorkspaceId);
				CrossQueryPhrase byWorkpace = new CrossQueryPhrase(WorkspaceRole.WORKSPACE_FIELD, workspaceIdPhrase);

				List<QueryPhrase> queries = new List<QueryPhrase>();
				queries.Add(byWorkpace);
				queries.Add(byRole);
				EntityListResult<WorkspaceRole> roles = entityService.Get<WorkspaceRole>(sharedSpaceContext, queries, null);
				workspaceAdminRole = roles.data[0];
				Assert.AreEqual(WorkspaceRole.WORKSPACE_ADMIN_ROLE_LOGICAL_NAME, workspaceAdminRole.Role.LogicalName);
				Assert.AreEqual(workspaceContext.WorkspaceId.ToString(), workspaceAdminRole.Workspace.Id.ToString());


			}
			return workspaceAdminRole;
		}

        private static SharedspaceUser CreateLDAPUser()
        {
            SharedspaceUser user = new SharedspaceUser();
            user.UID = Guid.NewGuid().ToString();//should be mapped value of uid field, for example ,entryUUID, will be updated on login
            user.Name = "mail16@nga";
            user.LastName = "a";//no matter , will be updated on login
            user.FirstName = "a";//no matter, will be updated on login
            user.Email = user.Name;
            user.Phone1 = "";//no matter, will be updated on login
            user.SetValue("ldap_dn", "cn=uid16@nga,ou=dummy_users,dc=maxcrc,dc=com");

            var returnFields = new string[] { "name" };

            SharedspaceUser createdUser = entityService.Create<SharedspaceUser>(sharedSpaceContext, user, returnFields);
            return createdUser;
        }
    }
}
