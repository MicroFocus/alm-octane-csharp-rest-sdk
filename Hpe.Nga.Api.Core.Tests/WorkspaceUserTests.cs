using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Entities;
using Hpe.Nga.Api.Core.Services;
using Hpe.Nga.Api.Core.Services.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hpe.Nga.Api.Core.Tests
{
    [TestClass]
    public class WorkspaceUserTests : BaseTest
    {

        [TestMethod]
        public void GetAllWorkspaceUsers()
        {
            EntityListResult<WorkspaceUser> users = EntityService.GetInstance().Get<WorkspaceUser>(workspaceContext, null, null);
            Assert.IsTrue(users.total_count >= 1);

        }

        [TestMethod]
        public void GetWorkspaceUserByName()
        {
            LogicalQueryPhrase byName = new LogicalQueryPhrase(WorkspaceUser.NAME_FIELD, CurrentUserName);
            List<QueryPhrase> queries = new List<QueryPhrase>();
            queries.Add(byName);

            EntityListResult<WorkspaceUser> users = EntityService.GetInstance().Get<WorkspaceUser>(workspaceContext, queries, null);
            Assert.IsTrue(users.total_count == 1);

        }

    }
}
