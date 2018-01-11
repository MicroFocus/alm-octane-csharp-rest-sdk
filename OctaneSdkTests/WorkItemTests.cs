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
using MicroFocus.Adm.Octane.Api.Core.Services.GroupBy;
using MicroFocus.Adm.Octane.Api.Core.Services.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MicroFocus.Adm.Octane.Api.Core.Tests
{
	/// <summary>
	/// Defect, Stories,Features might be handled generally by using their aggregated type - WorkItem
	/// </summary>
	[TestClass]
    public class WorkItemTests : BaseTest
    {
        private static WorkItemRoot workItemRoot;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {

           
        }

        private WorkItemRoot getWorkItemRoot()
        {
            if (workItemRoot == null)
            {
                workItemRoot = TestHelper.GetWorkItemRoot(entityService, workspaceContext);
            }
            return workItemRoot;
        }

        [TestMethod]
        public void CrudDefectAsWorkItemTest()
        {
            //CREATE
            Phase DEFECT_PHASE_NEW = TestHelper.GetPhaseForEntityByLogicalName(entityService, workspaceContext, WorkItem.SUBTYPE_DEFECT, "phase.defect.new");
            ListNode SEVERITY_HIGH = TestHelper.GetSeverityByName(entityService, workspaceContext, "High");
            String name = "Defect" + Guid.NewGuid();
            WorkItem workItem = new WorkItem();
            workItem.Name = name;
            workItem.Phase = DEFECT_PHASE_NEW;
            workItem.Severity = SEVERITY_HIGH;
            workItem.Parent = getWorkItemRoot();
            workItem.SubType = WorkItem.SUBTYPE_DEFECT;//For workItems - SUBTYPE have to be set
            WorkItem created = entityService.Create<WorkItem>(workspaceContext, workItem, TestHelper.NameSubtypeFields);

            Assert.AreEqual<String>(WorkItem.SUBTYPE_DEFECT, created.SubType);
            Assert.AreEqual<String>(name, created.Name);

            //UPDATE
            WorkItem workItemForUpdate = new WorkItem(created.Id);
            workItemForUpdate.Name = created.Name + "_updated";
            WorkItem updated = entityService.Update<WorkItem>(workspaceContext, workItemForUpdate, TestHelper.NameFields);
            Assert.AreEqual<String>(workItemForUpdate.Name, updated.Name);


            //DELETE BY ID
            entityService.DeleteById<WorkItem>(workspaceContext, created.Id);
        }


        [TestMethod]
        public void CrudStoryAsWorkItemTest()
        {
            Phase STORY_PHASE_NEW = TestHelper.GetPhaseForEntityByLogicalName(entityService, workspaceContext, WorkItem.SUBTYPE_STORY, "phase.story.new");
            String name = "Story" + Guid.NewGuid();
            WorkItem workItem = new WorkItem();
            workItem.Name = name;
            workItem.Phase = STORY_PHASE_NEW;
            workItem.Parent = getWorkItemRoot();
            workItem.SubType = WorkItem.SUBTYPE_STORY;//For workItems - SUBTYPE have to be set

            WorkItem created = entityService.Create<WorkItem>(workspaceContext, workItem, TestHelper.NameSubtypeFields);
            Assert.AreEqual<String>(WorkItem.SUBTYPE_STORY, created.SubType);
            Assert.AreEqual<String>(name, created.Name);


            //UPDATE
            WorkItem workItemForUpdate = new WorkItem(created.Id);
            workItemForUpdate.Name = created.Name + "_updated";
            WorkItem updated = entityService.Update<WorkItem>(workspaceContext, workItemForUpdate, TestHelper.NameFields);
            Assert.AreEqual<String>(workItemForUpdate.Name, updated.Name);


            //DELETE BY FILTER
            entityService.DeleteById<WorkItem>(workspaceContext, created.Id);
        }

        [TestMethod]
        public void CreateEpicAndFeatureAsWorkItemsTest()
        {
            //create epic
            Phase EPIC_PHASE_NEW = TestHelper.GetPhaseForEntityByLogicalName(entityService, workspaceContext, WorkItem.SUBTYPE_EPIC, "phase.epic.new");
            String epicName = "Epic" + Guid.NewGuid();
            WorkItem epicToCreate = new WorkItem();
            epicToCreate.Name = epicName;
            epicToCreate.Phase = EPIC_PHASE_NEW;
            epicToCreate.Parent = getWorkItemRoot();
            epicToCreate.SubType = WorkItem.SUBTYPE_EPIC;//For workItems - SUBTYPE have to be set
            string[] fields = new string[] { "subtype", "name" };

            WorkItem createdEpic = entityService.Create<WorkItem>(workspaceContext, epicToCreate, fields);
            Assert.AreEqual<String>(WorkItem.SUBTYPE_EPIC, createdEpic.SubType);
            Assert.AreEqual<String>(epicName, createdEpic.Name);


            //parent of feature can be only epic, workItemRoot cannot be parent of feature
            Phase FEATURE_PHASE_NEW = TestHelper.GetPhaseForEntityByLogicalName(entityService, workspaceContext, WorkItem.SUBTYPE_FEATURE, "phase.feature.new");
            String featureName = "Feature" + Guid.NewGuid();
            WorkItem featureToCreate = new WorkItem();
            featureToCreate.Name = featureName;
            featureToCreate.Phase = FEATURE_PHASE_NEW;
            featureToCreate.Parent = createdEpic;
            featureToCreate.SubType = WorkItem.SUBTYPE_FEATURE; //For workItems - SUBTYPE have to be set

            WorkItem createdFeature = entityService.Create<WorkItem>(workspaceContext, featureToCreate, fields);
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
        public void GetOnlyDefectWithGroupSeverityTest()
        {
            List<String> fields = new List<string>();
            fields.Add(WorkItem.NAME_FIELD);
            fields.Add(WorkItem.SUBTYPE_FIELD);


            List<QueryPhrase> queries = new List<QueryPhrase>();
            LogicalQueryPhrase subtypeQuery = new LogicalQueryPhrase(WorkItem.SUBTYPE_FIELD, WorkItem.SUBTYPE_DEFECT);
            queries.Add(subtypeQuery);

            //~work_items/groups?group_by=severity&query="(subtype='defect');
            GroupResult result = entityService.GetWithGroupBy<WorkItem>(workspaceContext, queries, Defect.SEVERITY_FIELD);
        }

    }

}
