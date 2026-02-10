/*
 * Copyright 2016-2026 Open Text.
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
using MicroFocus.Adm.Octane.Api.Core.Services.GroupBy;
using MicroFocus.Adm.Octane.Api.Core.Services.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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

        public static WorkItemRoot getWorkItemRoot()
        {
            if (workItemRoot == null)
            {
                workItemRoot = TestHelper.GetWorkItemRoot(entityService, workspaceContext);
            }
            return workItemRoot;
        }

        [TestMethod]
        public void CrudDefect()
        {
            //CREATE
            Phase DEFECT_PHASE_NEW = TestHelper.GetPhaseForEntityByLogicalName(entityService, workspaceContext, WorkItem.SUBTYPE_DEFECT, "phase.defect.new");
            ListNode SEVERITY_HIGH = TestHelper.GetSeverityByName(entityService, workspaceContext, "High");
            string name = "Defect" + Guid.NewGuid();
            Defect workItem = new Defect();
            workItem.Name = name;
            workItem.Phase = DEFECT_PHASE_NEW;
            workItem.Severity = SEVERITY_HIGH;
            workItem.Parent = getWorkItemRoot();
            //workItem.SubType = WorkItem.SUBTYPE_DEFECT;//For workItems - SUBTYPE have to be set
            Defect created = entityService.Create<Defect>(workspaceContext, workItem, TestHelper.NameSubtypeFields);

            // Assert.AreEqual<String>(WorkItem.SUBTYPE_DEFECT, created.SubType);
            Assert.AreEqual<String>(name, created.Name);

            //UPDATE
            Defect workItemForUpdate = new Defect(created.Id);
            workItemForUpdate.Name = created.Name + "_updated";
            Defect updated = entityService.Update<Defect>(workspaceContext, workItemForUpdate, TestHelper.NameFields);
            Assert.AreEqual<String>(workItemForUpdate.Name, updated.Name);


            //DELETE BY ID
            entityService.DeleteById<Defect>(workspaceContext, created.Id);
        }


        [TestMethod]
        public void CrudStory()
        {
            Phase STORY_PHASE_NEW = TestHelper.GetPhaseForEntityByLogicalName(entityService, workspaceContext, WorkItem.SUBTYPE_STORY, "phase.story.new");
            string name = "Story" + Guid.NewGuid();
            Story workItem = new Story();
            workItem.Name = name;
            workItem.Phase = STORY_PHASE_NEW;
            workItem.Parent = getWorkItemRoot();
            //workItem.SubType = WorkItem.SUBTYPE_STORY;//For workItems - SUBTYPE have to be set

            WorkItem created = entityService.Create<Story>(workspaceContext, workItem, TestHelper.NameSubtypeFields);
            //Assert.AreEqual<String>(WorkItem.SUBTYPE_STORY, created.SubType);
            Assert.AreEqual<string>(name, created.Name);


            //UPDATE
            Story workItemForUpdate = new Story(created.Id);
            workItemForUpdate.Name = created.Name + "_updated";
            Story updated = entityService.Update<Story>(workspaceContext, workItemForUpdate, TestHelper.NameFields);
            Assert.AreEqual<string>(workItemForUpdate.Name, updated.Name);


            //DELETE BY FILTER
            entityService.DeleteById<Story>(workspaceContext, created.Id);
        }

        [TestMethod]
        public void CreateEpicAndFeature()
        {
            var epic = CreateEpic();
            CreateFeature(epic);
        }

        private static Epic CreateEpic(string customName = null)
        {
            Phase EPIC_PHASE_NEW = TestHelper.GetPhaseForEntityByLogicalName(entityService, workspaceContext, WorkItem.SUBTYPE_EPIC, "phase.epic.new");
            string epicName = customName ?? "Epic" + Guid.NewGuid();
            Epic epicToCreate = new Epic();
            epicToCreate.Name = epicName;
            epicToCreate.Phase = EPIC_PHASE_NEW;
            epicToCreate.Parent = getWorkItemRoot();
            //epicToCreate.SubType = WorkItem.SUBTYPE_EPIC;//For workItems - SUBTYPE have to be set
            string[] fields = new string[] { "subtype", "name" };

            Epic createdEpic = entityService.Create<Epic>(workspaceContext, epicToCreate, fields);
            Assert.AreEqual(epicName, createdEpic.Name, "Mismatched epic name");

            return createdEpic;
        }

        private static Feature CreateFeature(Epic parentEpic, string customName = null)
        {
            //parent of feature can be only epic, workItemRoot cannot be parent of feature
            Phase FEATURE_PHASE_NEW = TestHelper.GetPhaseForEntityByLogicalName(entityService, workspaceContext, WorkItem.SUBTYPE_FEATURE, "phase.feature.new");
            string featureName = customName ?? "Feature" + Guid.NewGuid();
            Feature featureToCreate = new Feature();
            featureToCreate.Name = featureName;
            featureToCreate.Phase = FEATURE_PHASE_NEW;
            featureToCreate.Parent = parentEpic;
            //epicToCreate.SubType = WorkItem.SUBTYPE_EPIC;//For workItems - SUBTYPE have to be set
            string[] fields = new string[] { "subtype", "name" };

            Feature createdFeature = entityService.Create<Feature>(workspaceContext, featureToCreate, fields);
            //Assert.AreEqual<String>(WorkItem.SUBTYPE_FEATURE, createdFeature.SubType);
            Assert.AreEqual<string>(featureName, createdFeature.Name);

            return createdFeature;
        }

        [TestMethod]
        public void GetOnlyDefectsWithLimit1()
        {
            List<string> fields = new List<string>();
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
            List<string> fields = new List<string>();
            fields.Add(WorkItem.NAME_FIELD);
            fields.Add(WorkItem.SUBTYPE_FIELD);


            List<QueryPhrase> queries = new List<QueryPhrase>();
            LogicalQueryPhrase subtypeQuery = new LogicalQueryPhrase(WorkItem.SUBTYPE_FIELD, WorkItem.SUBTYPE_DEFECT);
            queries.Add(subtypeQuery);

            //~work_items/groups?group_by=severity&query="(subtype='defect');
            GroupResult result = entityService.GetWithGroupBy<WorkItem>(workspaceContext, queries, Defect.SEVERITY_FIELD);
        }

        [TestCategory("LongTest")]
        [TestMethod]
        public void SearchWorkItems()
        {
            var guid = Guid.NewGuid();
            var nameSuffix = "CustomName_" + guid;
            var story = StoryTests.CreateStory("Story " + nameSuffix);
            var epic = CreateEpic("Epic " + nameSuffix);
            var feature = CreateFeature(epic, "Feature " + nameSuffix);
            var defect = DefectTests.CreateDefect("Defect " + nameSuffix);

            var expectedWorkItems = new List<WorkItem>
            {
                story, epic, feature
            };

            EntityListResult<WorkItem> searchResult = null;
            SpinWait.SpinUntil(() =>
            {
                Thread.Sleep(1000);

                searchResult = entityService.SearchAsync<WorkItem>(workspaceContext, nameSuffix,
                    new List<string> { WorkItem.SUBTYPE_STORY, WorkItem.SUBTYPE_EPIC, WorkItem.SUBTYPE_FEATURE }).Result;

                return searchResult.data.Count == 3;
            }, new TimeSpan(0, 2, 0));

            Assert.IsNotNull(searchResult, "search operation should have returned something");
            Assert.AreEqual(3, searchResult.data.Count, "Mismatched number of entities returned by the search operation");

            int actualTestsFoundCount = 0;
            foreach (var workItem in expectedWorkItems)
            {
                actualTestsFoundCount += searchResult.data.Count(t => t.Id == workItem.Id);
            }

            Assert.AreEqual(searchResult.data.Count, actualTestsFoundCount, "Search request didn't return expected results");
        }
    }
}
