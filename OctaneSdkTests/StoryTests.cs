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
using MicroFocus.Adm.Octane.Api.Core.Services.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MicroFocus.Adm.Octane.Api.Core.Tests
{
    [TestClass]
    public class StoryTests : BaseTest
    {
        [TestMethod]
        public void GetStoryFieldMetadataTest()
        {
            ListResult<FieldMetadata> result = entityService.GetFieldsMetadataAsync(workspaceContext, WorkItem.SUBTYPE_STORY).Result;
            Assert.IsTrue(result.total_count >= 1);
        }

        [TestMethod]
        public void CreateStoryTest()
        {
            CreateStory();
        }

        [TestMethod]
        public void GetAllStories()
        {
            CreateStory();

            //get as stories
            EntityListResult<Story> stories = entityService.Get<Story>(workspaceContext, null, null);
            Assert.IsTrue(stories.total_count > 0);


            //get as work-items
            List<QueryPhrase> queries = new List<QueryPhrase>();
            LogicalQueryPhrase byStorySubType = new LogicalQueryPhrase(WorkItem.SUBTYPE_FIELD, WorkItem.SUBTYPE_STORY);
            queries.Add(byStorySubType);

            EntityListResult<WorkItem> storiesAsWorkItems = entityService.Get<WorkItem>(workspaceContext, queries, null);
            Assert.AreEqual<int?>(stories.total_count, storiesAsWorkItems.total_count);

        }

        private static Phase _phaseNew;
        private static WorkItemRoot _workItemRoot;

        private static Phase GetPhaseNew()
        {
            if (_phaseNew == null)
                _phaseNew = TestHelper.GetPhaseForEntityByLogicalName(entityService, workspaceContext, WorkItem.SUBTYPE_STORY, "phase.story.new");

            return _phaseNew;
        }

        private static WorkItemRoot GetWorkItemRoot()
        {
            if (_workItemRoot == null)
                _workItemRoot = TestHelper.GetWorkItemRoot(entityService, workspaceContext);

            return _workItemRoot;
        }

        /// <summary>
        /// Create a new user story entity
        /// </summary>
        public static Story CreateStory(string customName = null)
        {
            var name = customName ?? "Story_" + Guid.NewGuid();
            var story = new Story
            {
                Name = name,
                Phase = GetPhaseNew(),
                Parent = GetWorkItemRoot()
            };

            var createdStory = entityService.Create(workspaceContext, story, TestHelper.NameSubtypeFields);
            Assert.AreEqual<string>(name, createdStory.Name, "Newly created story doesn't have the expected name");
            Assert.IsFalse(string.IsNullOrEmpty(createdStory.Id), "Newly created story should have a valid ID");
            return createdStory;
        }

        [TestCategory("LongTest")]
        [TestMethod]
        public void SearchStories()
        {
            EntityListResult<WorkItem> searchResult = null;
            SpinWait.SpinUntil(() =>
            {
                Thread.Sleep(1000);

                searchResult = entityService.SearchAsync<WorkItem>(workspaceContext, "story", new List<string> { WorkItem.SUBTYPE_STORY }, 10).Result;

                return searchResult.data.Count > 0;
            }, new TimeSpan(0, 2, 0));

            Assert.IsTrue(searchResult.data.Count > 0);
            Assert.IsTrue(searchResult.data.Count <= 10);
        }

        [TestMethod]
        public void GetPossibleTransitionsForStoryEntity()
        {
            var result = entityService.GetTransitionsForEntityType(workspaceContext, "story").Result;
            Assert.IsTrue(result.data.Count > 0);
        }
    }
}
