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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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

        private static TestGherkin CreateGherkinTestInternal()
        {
            string name = "GherkinTest" + Guid.NewGuid();
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
    }
}
