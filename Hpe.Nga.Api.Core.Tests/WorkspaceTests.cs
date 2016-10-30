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
    public class WorkspaceTests : BaseTest
    {

        [TestMethod]
        public void CreateWorkspace()
        {
            Workspace workspace = new Workspace();
            workspace.Name = "WS_" + Guid.NewGuid();

            Workspace created = entityService.Create<Workspace>(sharedSpaceContext, workspace);

            Assert.AreEqual<String>(workspace.Name, created.Name);

        }

      

    }
}
