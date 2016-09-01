using System;
using System.Collections.Generic;
using Hpe.Nga.Api.Core.Connector.Exceptions;
using Hpe.Nga.Api.Core.Entities;
using Hpe.Nga.Api.Core.Services;
using Hpe.Nga.Api.Core.Services.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hpe.Nga.Api.Core.Tests
{
    [TestClass]
    public class DefectTests : BaseTest
    {
        private static Phase PHASE_NEW;
        private static ListNode SEVERITY_HIGH;
        private static WorkItemRoot WORK_ITEM_ROOT;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            PHASE_NEW = TestHelper.GetPhaseForEntityByName(workspaceContext, WorkItem.SUBTYPE_DEFECT, "New");
            SEVERITY_HIGH = TestHelper.GetSeverityByName(workspaceContext, "High");
            WORK_ITEM_ROOT = TestHelper.GetWorkItemRoot(workspaceContext);
        }

        [TestMethod]
        public void GetDefectFieldMetadataTest()
        {
            List<QueryPhrase> queryPhrases = new List<QueryPhrase>();

            LogicalQueryPhrase byEntityNamePhrase = new LogicalQueryPhrase(FieldMetadata.ENTITY_NAME_FIELD, "defect");
            queryPhrases.Add(byEntityNamePhrase);

            EntityListResult<FieldMetadata> result = entityService.Get<FieldMetadata>(workspaceContext, queryPhrases, null);
            Assert.IsTrue(result.total_count >= 50);
        }

        [TestMethod]
        public void GetPhasesForDefectTest()
        {
            List<QueryPhrase> queryPhrases = new List<QueryPhrase>();
            LogicalQueryPhrase byEntityPhrase = new LogicalQueryPhrase(Phase.ENTITY_FIELD, WorkItem.SUBTYPE_DEFECT);
            queryPhrases.Add(byEntityPhrase);
            EntityListResult<Phase> result = entityService.Get<Phase>(workspaceContext, queryPhrases, null);
            Assert.IsTrue(result.total_count >= 1);
        }

        [TestMethod]
        public void GetSeverityListForDefectsTest()
        {
            List<QueryPhrase> queryPhrases = new List<QueryPhrase>();
            LogicalQueryPhrase byLogicalName = new LogicalQueryPhrase(ListNode.LOGICAL_NAME_FIELD, "list_node.severity.*");
            queryPhrases.Add(byLogicalName);

            EntityListResult<ListNode> result = entityService.Get<ListNode>(workspaceContext, queryPhrases, null);
            Assert.IsTrue(result.total_count >= 5);
        }

        [TestMethod]
        public void CreateDefectTest()
        {
            Defect defect = CreateDefect();
        }

        [TestMethod]
        public void UpdateDefectNameTest()
        {
            Defect defect = CreateDefect();
            Defect defectForUpdate = new Defect(defect.Id);
            String newName = defect.Name + "_updated";
            defectForUpdate.Name = newName;
            entityService.Update<Defect>(workspaceContext, defectForUpdate);

            Defect defectAfterUpdate = entityService.GetById<Defect>(workspaceContext, defect.Id, null);
            Assert.AreEqual(newName, defectAfterUpdate.Name);
        }

        [TestMethod]
        public void GetNotCompletedDefectsAssinedToReleaseTest()
        {
            Defect defect = CreateDefect();
            Release release = CreateRelease();

            //assign defect to release
            Defect defectForUpdate = new Defect(defect.Id);
            defectForUpdate.Release = release;
            entityService.Update<Defect>(workspaceContext, defectForUpdate);
            Defect defectAfterUpdate = entityService.GetById<Defect>(workspaceContext, defect.Id, null);
            Assert.AreEqual<long>(release.Id, defectAfterUpdate.Release.Id);

            //Fetch all defects that assigned to release and still not done
            //Fetch defects as work-items 
            List<QueryPhrase> queries = new List<QueryPhrase>();
            LogicalQueryPhrase subtypeQuery = new LogicalQueryPhrase(WorkItem.SUBTYPE_FIELD, WorkItem.SUBTYPE_DEFECT);
            queries.Add(subtypeQuery);

            //condition of release
            QueryPhrase releaseIdPhrase = new LogicalQueryPhrase(WorkItem.ID_FIELD, release.Id);
            QueryPhrase byReleasePhrase = new CrossQueryPhrase(WorkItem.RELEASE_FIELD, releaseIdPhrase);
            queries.Add(byReleasePhrase);

            //condition by metaphase (parent of phase)
            LogicalQueryPhrase phaseNamePhrase = new LogicalQueryPhrase(WorkItem.NAME_FIELD, "Done");
            phaseNamePhrase.NegativeCondition = true;//not Done
            CrossQueryPhrase phaseIdPhrase = new CrossQueryPhrase("metaphase", phaseNamePhrase);
            CrossQueryPhrase byPhasePhrase = new CrossQueryPhrase(WorkItem.PHASE_FIELD, phaseIdPhrase);
            queries.Add(byPhasePhrase);

            EntityListResult<WorkItem> entitiesResult = entityService.Get<WorkItem>(workspaceContext, queries, null);
            Assert.AreEqual<int>(1, entitiesResult.total_count.Value);
            Assert.AreEqual<long>(defect.Id, entitiesResult.data[0].Id);
        }

        [TestMethod]
        public void DeleteDefectsByFilter()
        {
            Defect createdDefect = CreateDefect();


            List<QueryPhrase> queries = new List<QueryPhrase>();
            LogicalQueryPhrase byName = new LogicalQueryPhrase(Defect.NAME_FIELD, createdDefect.Name);
            queries.Add(byName);

            entityService.DeleteByFilter<WorkItem>(workspaceContext, queries);

            try
            {
                Defect defect = entityService.GetById<Defect>(workspaceContext, createdDefect.Id, null);
                Assert.Fail("Should not get here");
            }
            catch (MqmRestException e)
            {
                Assert.AreEqual<string>("platform.entity_not_found", e.ErrorCode);
            }

        }

        private static Defect CreateDefect()
        {
            String name = "Defect" + Guid.NewGuid();
            Defect defect = new Defect();
            defect.Name = name;
            defect.Phase = PHASE_NEW;
            defect.Severity = SEVERITY_HIGH;
            defect.Parent = WORK_ITEM_ROOT;
            Defect created = entityService.Create<Defect>(workspaceContext, defect);
            Assert.AreEqual<String>(name, created.Name);
            Assert.IsTrue(created.Id > 0);
            return created;

        }

        private static Release CreateRelease()
        {
            String name = "Release_" + Guid.NewGuid();
            Release release = new Release();
            release.Name = name;
            release.StartDate = DateTime.Now;
            release.EndDate = DateTime.Now.AddDays(10);
            release.SprintDuration = 7;


            Release created = entityService.Create<Release>(workspaceContext, release);
            Assert.AreEqual<String>(name, created.Name);
            return created;
        }

    }

}
