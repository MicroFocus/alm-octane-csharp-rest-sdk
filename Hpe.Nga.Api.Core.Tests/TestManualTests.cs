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
            PHASE_NEW = TestHelper.GetPhaseForEntityByName(workspaceContext, Test.SUBTYPE_MANUAL_TEST, "New");
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
            EntityListResult<ManualTest> testManuals = entityService.Get<ManualTest>(workspaceContext, null, null);
            Assert.IsTrue(testManuals.total_count > 0);


            //get as test
            List<QueryPhrase> queries = new List<QueryPhrase>();
            LogicalQueryPhrase byTestManualSubType = new LogicalQueryPhrase(Test.SUBTYPE_FIELD, Test.SUBTYPE_MANUAL_TEST);
            queries.Add(byTestManualSubType);

            EntityListResult<Test> testManualsAsTests = entityService.Get<Test>(workspaceContext, queries, null);
            Assert.AreEqual<int?>(testManuals.total_count, testManualsAsTests.total_count);

        }

        private static ManualTest CreateManualTest()
        {
            String name = "Test" + Guid.NewGuid();
            ManualTest test = new ManualTest();
            test.Name = name;
            test.Phase = PHASE_NEW;


            ManualTest created = entityService.Create<ManualTest>(workspaceContext, test);
            Assert.AreEqual<String>(name, created.Name);
            Assert.IsTrue(created.Id > 0);
            return created;
        }
    }
}
