/*
 * Copyright 2016-2023 Open Text.
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

using MicroFocus.Adm.Octane.Api.Core.Entities;
using MicroFocus.Adm.Octane.Api.Core.Services;
using MicroFocus.Adm.Octane.Api.Core.Services.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MicroFocus.Adm.Octane.Api.Core.Tests
{
    [TestClass]
    public class RunSuiteTests : BaseTest
    {
        [TestMethod]
        public void CreateRunSuiteTest()
        {
            CreateSuiteRun();
        }

        [TestMethod]
        public void GetAllSuiteRunTest()
        {
            CreateSuiteRun();

            //get as RunSuite
            EntityListResult<RunSuite> runSuites = entityService.Get<RunSuite>(workspaceContext, null, null);
            Assert.IsTrue(runSuites.total_count > 0);

            //get as run
            List<QueryPhrase> queries = new List<QueryPhrase>();
            LogicalQueryPhrase byRunSuiteSubType = new LogicalQueryPhrase(Test.SUBTYPE_FIELD, RunSuite.SUBTYPE_RUN_SUITE);
            queries.Add(byRunSuiteSubType);

            EntityListResult<Run> runSuitesAsRuns = entityService.Get<Run>(workspaceContext, queries, null);
            Assert.AreEqual<int?>(runSuites.total_count, runSuitesAsRuns.total_count);
        }

        [TestMethod]
        public void DeleteRunSuiteTest()
        {
            var runSuite = CreateSuiteRun();
            entityService.DeleteById<RunSuite>(workspaceContext, runSuite.Id);
        }

        public static RunSuite CreateSuiteRun(string customName = null)
        {
            var name = customName ?? "SuiteRun_" + Guid.NewGuid();

            var status = new BaseEntity();
            status.SetValue("id", "list_node.run_native_status.not_completed");
            status.SetValue("type", "list_node");

            var suiteRun = new RunSuite
            {
                Name = name,
                Release = ReleaseTests.CreateRelease(),
                Parent = CreateTestSuite(),
                NativeStatus = status
            };

            var createdSuiteRun = entityService.Create(workspaceContext, suiteRun, TestHelper.NameSubtypeFields);
            Assert.AreEqual(name, createdSuiteRun.Name, "Mismatched name for newly created test suite run");
            Assert.IsTrue(!string.IsNullOrEmpty(createdSuiteRun.Id), "Test suite run id shouldn't be null or empty");
            return createdSuiteRun;
        }

        private static TestSuite CreateTestSuite()
        {
            var name = "TestSuite_" + Guid.NewGuid();
            var testSuite = new TestSuite
            {
                Name = name,
            };

            var createdTestSuite = entityService.Create(workspaceContext, testSuite, TestHelper.NameSubtypeFields);
            Assert.AreEqual(name, createdTestSuite.Name, "Mismatched name for newly created test suite");
            Assert.IsTrue(!string.IsNullOrEmpty(createdTestSuite.Id), "Test suite id shouldn't be null or empty");
            return createdTestSuite;
        }
    }
}
