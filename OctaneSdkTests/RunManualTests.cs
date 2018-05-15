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

namespace MicroFocus.Adm.Octane.Api.Core.Tests
{
    [TestClass]
    public class RunManualTests : BaseTest
    {

        [TestMethod]
        public void CreateRunManualTest()
        {
            var manualTest = TestManualTests.CreateManualTest();
            CreateManualRun(manualTest);
        }

        [TestMethod]
        public void GetAllRunManualTest()
        {
            var manualTest = TestManualTests.CreateManualTest();
            CreateManualRun(manualTest);

            //get as runManual
            EntityListResult<RunManual> runManuals = entityService.Get<RunManual>(workspaceContext, null, null);
            Assert.IsTrue(runManuals.total_count > 0);


            //get as run
            List<QueryPhrase> queries = new List<QueryPhrase>();
            LogicalQueryPhrase byRunManualSubType = new LogicalQueryPhrase(Test.SUBTYPE_FIELD, RunManual.SUBTYPE_RUN_MANUAL);
            queries.Add(byRunManualSubType);

            EntityListResult<Run> runManualsAsRuns = entityService.Get<Run>(workspaceContext, queries, null);
            Assert.AreEqual<int?>(runManuals.total_count, runManualsAsRuns.total_count);
        }

        [TestMethod]
        public void DeleteRunManualTest()
        {
            var manualTest = TestManualTests.CreateManualTest();
            var manualRun = CreateManualRun(manualTest);

            entityService.DeleteById<RunManual>(workspaceContext, manualRun.Id);
        }

        public static RunManual CreateManualRun(TestManual parent, string customName = null)
        {
            var name = customName ?? "ManualRun_" + Guid.NewGuid();

            var status = new BaseEntity();
            status.SetValue("id", "list_node.run_native_status.not_completed");
            status.SetValue("type", "list_node");

            var manualRun = new RunManual
            {
                Name = name,
                Release = ReleaseTests.CreateRelease(),
                Parent = parent,
                NativeStatus = status
            };

            var createdManualRun = entityService.Create(workspaceContext, manualRun, new[] { "name", "subtype" });
            Assert.AreEqual(name, createdManualRun.Name, "Mismatched name for newly created manual run");
            Assert.IsTrue(!string.IsNullOrEmpty(createdManualRun.Id), "Manual run id shouldn't be null or empty");
            return createdManualRun;
        }
    }
}
