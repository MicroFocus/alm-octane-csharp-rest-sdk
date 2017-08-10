// (c) Copyright 2016 Hewlett Packard Enterprise Development LP

// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.

// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,

// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Hpe.Nga.Api.Core.Entities;
using Hpe.Nga.Api.Core.Services;
using Hpe.Nga.Api.Core.Services.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hpe.Nga.Api.Core.Tests
{
    [TestClass]
    public class TestManualTests : BaseTest
    {
        private static Phase PHASE_NEW;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            PHASE_NEW = TestHelper.GetPhaseForEntityByLogicalName(entityService, workspaceContext, Test.SUBTYPE_MANUAL_TEST, "phase.test_manual.new");
        }


        [TestMethod]
        public void GetManualTestFieldMetadataTest()
        {
            List<QueryPhrase> queryPhrases = new List<QueryPhrase>();

            LogicalQueryPhrase byEntityNamePhrase = new LogicalQueryPhrase(FieldMetadata.ENTITY_NAME_FIELD, Test.SUBTYPE_MANUAL_TEST);
            queryPhrases.Add(byEntityNamePhrase);

            EntityListResult<FieldMetadata> result = entityService.Get<FieldMetadata>(workspaceContext, queryPhrases, null);
            Assert.IsTrue(result.total_count >= 1);
        }

        [TestMethod]
        public void GetPhasesForManualTestTest()
        {
            List<QueryPhrase> queryPhrases = new List<QueryPhrase>();
            LogicalQueryPhrase byEntityPhrase = new LogicalQueryPhrase(Phase.ENTITY_FIELD, Test.SUBTYPE_MANUAL_TEST);
            queryPhrases.Add(byEntityPhrase);
            EntityListResult<Phase> result = entityService.Get<Phase>(workspaceContext, queryPhrases, null);
            Assert.IsTrue(result.total_count >= 1);
        }

        [TestMethod]
        public void CreateTestManualTest()
        {
            CreateManualTest();
        }

        [TestMethod]
        public void GetAllTestManualTest()
        {
            CreateManualTest();

            //get as testManual
            EntityListResult<TestManual> testManuals = entityService.Get<TestManual>(workspaceContext, null, null);
            Assert.IsTrue(testManuals.total_count > 0);


            //get as test
            List<QueryPhrase> queries = new List<QueryPhrase>();
            LogicalQueryPhrase byTestManualSubType = new LogicalQueryPhrase(Test.SUBTYPE_FIELD, Test.SUBTYPE_MANUAL_TEST);
            queries.Add(byTestManualSubType);

            EntityListResult<Test> testManualsAsTests = entityService.Get<Test>(workspaceContext, queries, null);
            Assert.AreEqual<int?>(testManuals.total_count, testManualsAsTests.total_count);

        }

        private static TestManual CreateManualTest()
        {
            String name = "Test" + Guid.NewGuid();
            TestManual test = new TestManual();
            test.Name = name;
            test.Phase = PHASE_NEW;


            TestManual created = entityService.Create<TestManual>(workspaceContext, test, TestHelper.NameSubtypeFields);
            Assert.AreEqual<String>(name, created.Name);
            Assert.IsTrue(created.Id > 0);
            return created;
        }
    }
}
