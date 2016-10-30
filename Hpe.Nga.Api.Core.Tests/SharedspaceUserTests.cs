using System;
using System.Collections.Generic;
using System.Linq;
using Hpe.Nga.Api.Core.Entities;
using Hpe.Nga.Api.Core.Services;
using Hpe.Nga.Api.Core.Services.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hpe.Nga.Api.Core.Tests
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
        public void GetCurrentUser()
        {
            LogicalQueryPhrase byName = new LogicalQueryPhrase(SharedspaceUser.NAME_FIELD, CurrentUserName);
            List<QueryPhrase> queries = new List<QueryPhrase>();
            queries.Add(byName);

            List<String> fields = new List<string> { SharedspaceUser.NAME_FIELD, SharedspaceUser.WORKSPACE_ROLES_FIELD };

            EntityListResult<SharedspaceUser> users = entityService.Get<SharedspaceUser>(sharedSpaceContext, queries, fields);
            Assert.AreEqual<int>(1, users.total_count.Value);

        }

        [TestMethod]
        public void GetAllSharedspaceUsersTest()
        {
            EntityListResult<SharedspaceUser> users = entityService.Get<SharedspaceUser>(sharedSpaceContext, null, null);
            Assert.IsTrue(users.total_count >= 1);

        }

        [TestMethod]
        public void GetSharedspaceUserByNameTest()
        {
            LogicalQueryPhrase byName = new LogicalQueryPhrase(WorkspaceUser.NAME_FIELD, CurrentUserName);
            List<QueryPhrase> queries = new List<QueryPhrase>();
            queries.Add(byName);

            EntityListResult<SharedspaceUser> users = entityService.Get<SharedspaceUser>(sharedSpaceContext, queries, null);
            Assert.IsTrue(users.total_count == 1);
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

            List<long> assignedWorkspaceRoles = updatedUser.WorkspaceRoles.data.Select(p => p.Id).ToList<long>();
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

            entityService.Update<SharedspaceUser>(sharedSpaceContext, userForUpdate, serviceArgs);

            //READ USER
            List<String> fields = new List<string> { SharedspaceUser.NAME_FIELD, SharedspaceUser.WORKSPACE_ROLES_FIELD };
            SharedspaceUser updatedUser = entityService.GetById<SharedspaceUser>(sharedSpaceContext, createdUser.Id, fields);

            List<long> assignedWorkspaceRoles = updatedUser.WorkspaceRoles.data.Select(p => p.Id).ToList<long>();
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

            SharedspaceUser createdUser = entityService.Create<SharedspaceUser>(sharedSpaceContext, user);
            Assert.AreEqual<string>(user.Name, createdUser.Name);
            return createdUser;
        }
    }
}
