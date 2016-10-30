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
        private static Phase phaseNew;
        private static ListNode severityHigh;
        private static WorkItemRoot workItemRoot;

        private static ListNode getSeverityHigh()
        {
            if (severityHigh == null)
            {
                severityHigh = TestHelper.GetSeverityByName(entityService, workspaceContext, "High");
            }
            return severityHigh;
        }

        private static Phase getPhaseNew()
        {
            if (phaseNew == null)
            {
                phaseNew = TestHelper.GetPhaseForEntityByName(entityService, workspaceContext, WorkItem.SUBTYPE_DEFECT, "New");
            }
            return phaseNew;
        }

        private static WorkItemRoot getWorkItemRoot()
        {
            if (workItemRoot == null)
            {
                workItemRoot = TestHelper.GetWorkItemRoot(entityService, workspaceContext);
            }
            return workItemRoot;
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
        public void GetNotDoneDefectsAssinedToReleaseTest()
        {
            Phase PHASE_CLOSED = TestHelper.GetPhaseForEntityByName(entityService, workspaceContext, WorkItem.SUBTYPE_DEFECT, "Closed");

            Defect defect1 = CreateDefect();
            Defect defect2 = CreateDefect(PHASE_CLOSED);
            Defect defect3 = CreateDefect();
            Release release = CreateRelease();

            //assign defect to release
            Defect defectForUpdate1 = new Defect(defect1.Id);
            defectForUpdate1.Release = release;
            Defect defectForUpdate2 = new Defect(defect2.Id);
            defectForUpdate2.Release = release;
            Defect defectForUpdate3 = new Defect(defect3.Id);
            defectForUpdate3.Release = release;
            //defectForUpdate3.Phase

            EntityList<Defect> listForUpdate = new EntityList<Defect>();
            listForUpdate.data.AddRange(new Defect[] { defectForUpdate1, defectForUpdate2, defectForUpdate3 });

            EntityListResult<Defect> updated = entityService.UpdateEntities<Defect>(workspaceContext, listForUpdate);
            Assert.AreEqual<int?>(3, updated.total_count);

            //Fetch all defects that assigned to release and still not done
            //Fetch defects as work-items 
            List<QueryPhrase> queries = new List<QueryPhrase>();
            LogicalQueryPhrase subtypeQuery = new LogicalQueryPhrase(WorkItem.SUBTYPE_FIELD, WorkItem.SUBTYPE_DEFECT);
            queries.Add(subtypeQuery);

            //condition of release
            QueryPhrase releaseIdPhrase = new LogicalQueryPhrase(WorkItem.ID_FIELD, release.Id);
            QueryPhrase byReleasePhrase = new CrossQueryPhrase(WorkItem.RELEASE_FIELD, releaseIdPhrase);
            queries.Add(byReleasePhrase);

            //There are several phased in "Done" metaphase - there are we are doing condition on metaphase and not on phase
            //condition by metaphase (parent of phase)
            LogicalQueryPhrase donePhaseNamePhrase = new LogicalQueryPhrase(Metaphase.NAME_FIELD, "Done");
            NegativeQueryPhrase notDonePhrase = new NegativeQueryPhrase(donePhaseNamePhrase);
            CrossQueryPhrase phaseIdPhrase = new CrossQueryPhrase("metaphase", notDonePhrase);
            CrossQueryPhrase byPhasePhrase = new CrossQueryPhrase(WorkItem.PHASE_FIELD, phaseIdPhrase);

            queries.Add(byPhasePhrase);

            EntityListResult<WorkItem> entitiesResult = entityService.Get<WorkItem>(workspaceContext, queries, null);
            Assert.AreEqual<int>(2, entitiesResult.total_count.Value);
            Assert.IsTrue(entitiesResult.data[0].Id == defect1.Id || entitiesResult.data[0].Id == defect3.Id);
            Assert.IsTrue(entitiesResult.data[1].Id == defect1.Id || entitiesResult.data[1].Id == defect3.Id);
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
            return CreateDefect(getPhaseNew());

        }

        private static Defect CreateDefect(Phase phase)
        {
            String name = "Defect" + Guid.NewGuid();
            Defect defect = new Defect();
            defect.Name = name;
            defect.Phase = phase;
            defect.Severity = getSeverityHigh();
            defect.Parent = getWorkItemRoot();
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
