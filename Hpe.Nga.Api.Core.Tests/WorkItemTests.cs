using System;
using System.Collections.Generic;
using Hpe.Nga.Api.Core.Entities;
using Hpe.Nga.Api.Core.Services;
using Hpe.Nga.Api.Core.Services.GroupBy;
using Hpe.Nga.Api.Core.Services.Query;
using Hpe.Nga.Api.Core.Services.RequestContext;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hpe.Nga.Api.Core.Tests
{
    /// <summary>
    /// Defect, Stories,Features might be handled generally by using their aggregated type - WorkItem
    /// </summary>
    [TestClass]
    public class WorkItemTests : BaseTest
    {
        private static WorkItemRoot WORK_ITEM_ROOT;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {

            WORK_ITEM_ROOT = TestHelper.GetWorkItemRoot(workspaceContext);
        }

        [TestMethod]
        public void CrudDefectAsWorkItemTest()
        {
            //CREATE
            Phase DEFECT_PHASE_NEW = TestHelper.GetPhaseForEntityByName(workspaceContext, WorkItem.SUBTYPE_DEFECT, "New");
            ListNode SEVERITY_HIGH = TestHelper.GetSeverityByName(workspaceContext, "High");
            String name = "Defect" + Guid.NewGuid();
            WorkItem workItem = new WorkItem();
            workItem.Name = name;
            workItem.Phase = DEFECT_PHASE_NEW;
            workItem.Severity = SEVERITY_HIGH;
            workItem.Parent = WORK_ITEM_ROOT;
            workItem.SubType = WorkItem.SUBTYPE_DEFECT;//For workItems - SUBTYPE have to be set
            WorkItem created = entityService.Create<WorkItem>(workspaceContext, workItem);

            Assert.AreEqual<String>(WorkItem.SUBTYPE_DEFECT, created.SubType);
            Assert.AreEqual<String>(name, created.Name);

            //UPDATE
            WorkItem workItemForUpdate = new WorkItem(created.Id);
            workItemForUpdate.Name = created.Name + "_updated";
            WorkItem updated = entityService.Update<WorkItem>(workspaceContext, workItemForUpdate);
            Assert.AreEqual<String>(workItemForUpdate.Name, updated.Name);


            //DELETE BY ID
            entityService.DeleteById<WorkItem>(workspaceContext, created.Id);
        }


        [TestMethod]
        public void CrudStoryAsWorkItemTest()
        {
            Phase STORY_PHASE_NEW = TestHelper.GetPhaseForEntityByName(workspaceContext, WorkItem.SUBTYPE_STORY, "New");
            String name = "Story" + Guid.NewGuid();
            WorkItem workItem = new WorkItem();
            workItem.Name = name;
            workItem.Phase = STORY_PHASE_NEW;
            workItem.Parent = WORK_ITEM_ROOT;
            workItem.SubType = WorkItem.SUBTYPE_STORY;//For workItems - SUBTYPE have to be set

            WorkItem created = entityService.Create<WorkItem>(workspaceContext, workItem);
            Assert.AreEqual<String>(WorkItem.SUBTYPE_STORY, created.SubType);
            Assert.AreEqual<String>(name, created.Name);


            //UPDATE
            WorkItem workItemForUpdate = new WorkItem(created.Id);
            workItemForUpdate.Name = created.Name + "_updated";
            WorkItem updated = entityService.Update<WorkItem>(workspaceContext, workItemForUpdate);
            Assert.AreEqual<String>(workItemForUpdate.Name, updated.Name);


            //DELETE BY FILTER
            entityService.DeleteById<WorkItem>(workspaceContext, created.Id);
        }

        [TestMethod]
        public void CreateEpicAndFeatureAsWorkItemsTest()
        {
            //create epic
            Phase EPIC_PHASE_NEW = TestHelper.GetPhaseForEntityByName(workspaceContext, WorkItem.SUBTYPE_EPIC, "New");
            String epicName = "Epic" + Guid.NewGuid();
            WorkItem epicToCreate = new WorkItem();
            epicToCreate.Name = epicName;
            epicToCreate.Phase = EPIC_PHASE_NEW;
            epicToCreate.Parent = WORK_ITEM_ROOT;
            epicToCreate.SubType = WorkItem.SUBTYPE_EPIC;//For workItems - SUBTYPE have to be set

            WorkItem createdEpic = entityService.Create<WorkItem>(workspaceContext, epicToCreate);
            Assert.AreEqual<String>(WorkItem.SUBTYPE_EPIC, createdEpic.SubType);
            Assert.AreEqual<String>(epicName, createdEpic.Name);


            //parent of feature can be only epic, workItemRoot cannot be parent of feature
            Phase FEATURE_PHASE_NEW = TestHelper.GetPhaseForEntityByName(workspaceContext, WorkItem.SUBTYPE_FEATURE, "New");
            String featureName = "Feature" + Guid.NewGuid();
            WorkItem featureToCreate = new WorkItem();
            featureToCreate.Name = featureName;
            featureToCreate.Phase = FEATURE_PHASE_NEW;
            featureToCreate.Parent = createdEpic;
            featureToCreate.SubType = WorkItem.SUBTYPE_FEATURE; //For workItems - SUBTYPE have to be set

            WorkItem createdFeature = entityService.Create<WorkItem>(workspaceContext, featureToCreate);
            Assert.AreEqual<String>(WorkItem.SUBTYPE_FEATURE, createdFeature.SubType);
            Assert.AreEqual<String>(featureName, createdFeature.Name);
        }

        [TestMethod]
        public void GetOnlyDefectsWithLimit1()
        {
            List<String> fields = new List<string>();
            fields.Add(WorkItem.NAME_FIELD);
            fields.Add(WorkItem.SUBTYPE_FIELD);

            List<QueryPhrase> queries = new List<QueryPhrase>();
            LogicalQueryPhrase subtypeQuery = new LogicalQueryPhrase(WorkItem.SUBTYPE_FIELD, WorkItem.SUBTYPE_DEFECT);
            queries.Add(subtypeQuery);

            EntityListResult<WorkItem> result = entityService.Get<WorkItem>(workspaceContext, queries, fields, 1);
            Assert.IsTrue(result.data.Count <= 1);

        }

        [TestMethod]
        public void GetOnlyDefectWithGroupSeverityTest(WorkspaceContext context)
        {
            List<String> fields = new List<string>();
            fields.Add(WorkItem.NAME_FIELD);
            fields.Add(WorkItem.SUBTYPE_FIELD);


            List<QueryPhrase> queries = new List<QueryPhrase>();
            LogicalQueryPhrase subtypeQuery = new LogicalQueryPhrase(WorkItem.SUBTYPE_FIELD, WorkItem.SUBTYPE_DEFECT);
            queries.Add(subtypeQuery);

            //~work_items/groups?group_by=severity&query="(subtype='defect');
            GroupResult result = entityService.GetWithGroupBy<WorkItem>(context, queries, Defect.SEVERITY_FIELD);
        }

    }

}
