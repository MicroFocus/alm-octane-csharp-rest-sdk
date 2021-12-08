/*!
* (c) Copyright 2016-2021 Micro Focus or one of its affiliates.
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
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MicroFocus.Adm.Octane.Api.Core.Tests
{
    /// <summary>
    /// Test class for the gherkin test entity
    /// </summary>
    [TestClass]
    public class TestGherkinTests : BaseTest
    {
        private static Phase PHASE_NEW;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            PHASE_NEW = TestHelper.GetPhaseForEntityByLogicalName(entityService, workspaceContext, TestGherkin.SUBTYPE_GHERKIN_TEST, "phase.gherkin_test.new");
        }

        /// <summary>
        /// Sanity check for creating and retrieving a gherkin test
        /// </summary>
        [TestMethod]
        public void CreateGherkinTest()
        {
            var expectedTest = CreateGherkinTestInternal();
            var test = entityService.GetByIdAsync(workspaceContext, expectedTest.Id, expectedTest.TypeName, TestHelper.NameSubtypeFields).Result;
            Assert.AreEqual(expectedTest.Name, test.Name, "Mismatched name for retrieved gherkin test");
        }

        private static TestGherkin CreateGherkinTestInternal(string customName = null)
        {
            string name = customName ?? "GherkinTest" + Guid.NewGuid();
            var test = new TestGherkin
            {
                Name = name,
                Phase = PHASE_NEW
            };

            TestGherkin createdTest = entityService.Create(workspaceContext, test, TestHelper.NameSubtypeFields);
            Assert.AreEqual(name, createdTest.Name, "Mismatched name for newly created gherkin test");
            Assert.IsTrue(!string.IsNullOrEmpty(createdTest.Id), "Gherking test id shouldn't be null or empty");
            return createdTest;
        }

        [TestCategory("LongTest")]
        [TestMethod]
        public void SearchGherkinTests()
        {
            var guid = Guid.NewGuid();
            var gherkinTest1 = CreateGherkinTestInternal("GherkinTest1_CustomName_" + guid);
            var gherkinTest2 = CreateGherkinTestInternal("GherkinTest2_CustomName_" + guid);
            var gherkinTest3 = CreateGherkinTestInternal("GherkinTest3_CustomName_" + guid);
            var gherkinTest4 = CreateGherkinTestInternal("GherkinTest4_CustomName_" + guid);
            var gherkinTest5 = CreateGherkinTestInternal("GherkinTest5_CustomName_" + guid);

            var possibleExpectedTests = new List<TestGherkin>
            {
                gherkinTest1, gherkinTest2, gherkinTest3, gherkinTest4, gherkinTest5
            };

            EntityListResult<Test> searchResult = null;
            SpinWait.SpinUntil(() =>
            {
                Thread.Sleep(1000);
                searchResult = entityService.SearchAsync<Test>(workspaceContext, "_CustomName_" + guid,
                    new List<string> { TestGherkin.SUBTYPE_GHERKIN_TEST }, 4).Result;
                return searchResult.data.Count == 4;
            }, new TimeSpan(0, 2, 0));

            Assert.IsNotNull(searchResult, "search operation should have returned something");
            Assert.AreEqual(4, searchResult.data.Count, "Mismatched number of entities returned by the search operation");

            int actualTestsFoundCount = 0;
            foreach (var test in possibleExpectedTests)
            {
                actualTestsFoundCount += searchResult.data.Count(t => t.Id == test.Id);
            }

            Assert.AreEqual(searchResult.data.Count, actualTestsFoundCount, "Search request didn't return expected results");
        }
    }
}
