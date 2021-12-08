/*!
* (c) Copyright 2021 Micro Focus or one of its affiliates.
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


using MicroFocus.Adm.Octane.Api.Core.Connector.Exceptions;
using MicroFocus.Adm.Octane.Api.Core.Entities;
using MicroFocus.Adm.Octane.Api.Core.Services;
using MicroFocus.Adm.Octane.Api.Core.Services.GroupBy;
using MicroFocus.Adm.Octane.Api.Core.Services.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MicroFocus.Adm.Octane.Api.Core.Tests
{
    [TestClass]
    public class DefectTests : BaseTest
    {
        private static Phase phaseNew;
        private static ListNode severityHigh, severityCritical;
        private static WorkItemRoot workItemRoot;

        private static ListNode getSeverityHigh()
        {
            if (severityHigh == null)
            {
                severityHigh = TestHelper.GetSeverityByName(entityService, workspaceContext, "High");
            }
            return severityHigh;
        }

        private static ListNode getSeverityCritical()
        {
            if (severityCritical == null)
            {
                severityCritical = TestHelper.GetSeverityByName(entityService, workspaceContext, "Urgent");
            }
            return severityCritical;
        }

        private static Phase getPhaseNew()
        {
            if (phaseNew == null)
            {
                phaseNew = TestHelper.GetPhaseForEntityByLogicalName(entityService, workspaceContext, WorkItem.SUBTYPE_DEFECT, "phase.defect.new");
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
            ListResult<FieldMetadata> result = entityService.GetFieldsMetadataAsync(workspaceContext, WorkItem.SUBTYPE_DEFECT).Result;
            Assert.IsTrue(result.total_count >= 41);
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
        public void CreateDefectWithTaskTest()
        {
            Defect defect = CreateDefect();
            Task task = CreateTask(defect);

            WorkItem parent = (WorkItem)task.GetValue(Task.STORY_FIELD);
            Assert.AreEqual(defect.Id, parent.Id);
        }

        [TestMethod]
        public void UpdateDefectNameTest()
        {
            Defect defect = CreateDefect();
            Defect defectForUpdate = new Defect(defect.Id);
            string newName = defect.Name + "_updated";
            defectForUpdate.Name = newName;
            entityService.Update<Defect>(workspaceContext, defectForUpdate);

            Defect defectAfterUpdate = entityService.GetById<Defect>(workspaceContext, defect.Id, null);
            Assert.AreEqual(newName, defectAfterUpdate.Name);
        }

        [TestMethod]
        public void UpdateDefectDetectedByTest()
        {
            Defect defect = CreateDefect();
            Defect defectForUpdate = new Defect(defect.Id);

            defectForUpdate.DetectedBy = defect.Author;
            entityService.Update(workspaceContext, defectForUpdate);

            Defect defectAfterUpdate = entityService.GetById<Defect>(workspaceContext, defect.Id, new string[] { Defect.NAME_FIELD, Defect.DETECTED_BY_FIELD, Defect.AUTHOR_FIELD });
            Assert.AreEqual(defectForUpdate.DetectedBy.Id, defectAfterUpdate.DetectedBy.Id);
        }

        [TestMethod]
        public void CreateGetAndUpdateTest()
        {
            Defect defect = CreateDefect();
            EntityListResult<Defect> list = entityService.Get<Defect>(workspaceContext);
            Defect firstDefect = list.data[0];
            Defect defectForUpdate = new Defect(firstDefect.Id);
            defectForUpdate.SetValue("name", defectForUpdate.Name + " updated " + new Guid());
            Defect updatedDefect = entityService.Update<Defect>(workspaceContext, defectForUpdate);
            Defect updatedDefect2 = entityService.GetById<Defect>(workspaceContext, firstDefect.Id, null);
            Assert.AreEqual(defectForUpdate.Name, updatedDefect2.Name);
        }

        [TestMethod]
        public void GetNotDoneDefectsAssinedToReleaseTest()
        {
            Phase PHASE_CLOSED = TestHelper.GetPhaseForEntityByLogicalName(entityService, workspaceContext, WorkItem.SUBTYPE_DEFECT, "phase.defect.closed");

            Defect defect1 = CreateDefect();
            Defect defect2 = CreateDefect(PHASE_CLOSED);
            Defect defect3 = CreateDefect();
            Defect defect4 = CreateDefect();
            Release release = ReleaseTests.CreateRelease();

            //assign defect to release
            Defect defectForUpdate1 = new Defect(defect1.Id);
            defectForUpdate1.Release = release;
            Defect defectForUpdate2 = new Defect(defect2.Id);
            defectForUpdate2.Release = release;
            Defect defectForUpdate3 = new Defect(defect3.Id);
            defectForUpdate3.Release = release;
            defectForUpdate3.Severity = getSeverityCritical();

            Defect defectForUpdate4 = new Defect(defect4.Id);
            defectForUpdate4.Release = release;


            EntityList<Defect> listForUpdate = new EntityList<Defect>();
            listForUpdate.data.AddRange(new Defect[] { defectForUpdate1, defectForUpdate2, defectForUpdate3, defectForUpdate4 });

            EntityListResult<Defect> updated = entityService.UpdateEntities<Defect>(workspaceContext, listForUpdate);
            Assert.AreEqual<int?>(4, updated.total_count);

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
            LogicalQueryPhrase donePhaseNamePhrase = new LogicalQueryPhrase(Metaphase.LOGICAL_NAME_FIELD, "metaphase.work_item.done");
            NegativeQueryPhrase notDonePhrase = new NegativeQueryPhrase(donePhaseNamePhrase);
            CrossQueryPhrase phaseIdPhrase = new CrossQueryPhrase("metaphase", notDonePhrase);
            CrossQueryPhrase byPhasePhrase = new CrossQueryPhrase(WorkItem.PHASE_FIELD, phaseIdPhrase);

            queries.Add(byPhasePhrase);

            EntityListResult<WorkItem> entitiesResult = entityService.Get<WorkItem>(workspaceContext, queries, null);
            Assert.AreEqual<int>(3, entitiesResult.total_count.Value);
            Assert.IsTrue(entitiesResult.data[0].Id == defect1.Id || entitiesResult.data[0].Id == defect3.Id || entitiesResult.data[0].Id == defect4.Id);
            Assert.IsTrue(entitiesResult.data[1].Id == defect1.Id || entitiesResult.data[1].Id == defect3.Id || entitiesResult.data[1].Id == defect4.Id);
            Assert.IsTrue(entitiesResult.data[2].Id == defect1.Id || entitiesResult.data[2].Id == defect3.Id || entitiesResult.data[2].Id == defect4.Id);


            //check group by
            GroupResult groupResult = entityService.GetWithGroupBy<WorkItem>(workspaceContext, queries, "severity");
            Assert.AreEqual(2, groupResult.groupsTotalCount);
            Group group1 = groupResult.groups[0];
            Group group2 = groupResult.groups[1];
            if (group1.count == 1)
            {
                Group temp = group1;
                group1 = group2;
                group2 = temp;
            }

            Assert.AreEqual<int>(2, group1.count);
            Assert.AreEqual<string>("list_node.severity.high", group1.value.logical_name);
            Assert.AreEqual<int>(1, group2.count);
            Assert.AreEqual<string>("list_node.severity.urgent", group2.value.logical_name);
        }

        [TestMethod]
        public void DeleteDefectsByFilter()
        {
            Defect createdDefect = CreateDefect();


            List<QueryPhrase> queries = new List<QueryPhrase>();
            LogicalQueryPhrase byName = new LogicalQueryPhrase(Defect.NAME_FIELD, createdDefect.Name);
            queries.Add(byName);

            entityService.DeleteByFilter<Defect>(workspaceContext, queries);

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

        [TestMethod]
        public void GetDefectsOrderedBy()
        { 
            Defect defect1 = CreateDefect();
            Thread.Sleep(1000);
            Defect defect2 = CreateDefect();
            Thread.Sleep(1000);
            Defect defect3 = CreateDefect();

            var entitiesResult = entityService.GetAsyncReferenceFields(workspaceContext, "work_items", null, null, "-creation_time", null).Result;

            Assert.IsTrue(entitiesResult.data[0].Id == defect3.Id && entitiesResult.data[1].Id == defect2.Id && entitiesResult.data[2].Id == defect1.Id);
        }

        public static Defect CreateDefect(string customName = null)
        {
            return CreateDefect(getPhaseNew(), customName);

        }

        public static Defect CreateDefect(Phase phase, string customName = null)
        {
            string name = customName ?? "Defect" + Guid.NewGuid();
            Defect defect = new Defect();
            defect.Name = name;
            defect.Phase = phase;
            defect.Severity = getSeverityHigh();
            defect.Parent = getWorkItemRoot();
            Defect created = entityService.Create(workspaceContext, defect, new string[] { Defect.NAME_FIELD, Defect.AUTHOR_FIELD });
            Assert.AreEqual(name, created.Name);
            Assert.IsTrue(!string.IsNullOrEmpty(created.Id));
            return created;

        }


        private static Task CreateTask(WorkItem story)
        {
            string name = "Task" + Guid.NewGuid();
            int estimatedHours = 5;
            Task task = new Task();
            task.Name = name;
            task.Story = story;
            task.EstimatedHours = estimatedHours;

            string[] fieldsToFetch = new string[] { Task.STORY_FIELD, Task.NAME_FIELD, Task.ESTIMATED_HOURS_FIELD, Task.OWNER_FIELD, Task.PHASE_FIELD, Task.REMAINING_HOURS_FIELD };
            Task created = entityService.Create<Task>(workspaceContext, task, fieldsToFetch);
            Assert.AreEqual<String>(name, created.Name);
            Assert.AreEqual<int?>(estimatedHours, created.EstimatedHours);
            return created;
        }

    }

}
