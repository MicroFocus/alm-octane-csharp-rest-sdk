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
    public class StoryTests: BaseTest
    {
        private static Phase PHASE_NEW;
        private static WorkItemRoot WORK_ITEM_ROOT;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            PHASE_NEW = TestHelper.GetPhaseForEntityByLogicalName(entityService, workspaceContext, WorkItem.SUBTYPE_STORY, "phase.story.new");
            WORK_ITEM_ROOT = TestHelper.GetWorkItemRoot(entityService, workspaceContext);
        }

        [TestMethod]
        public void GetStoryFieldMetadataTest()
        {
            List<QueryPhrase> queryPhrases = new List<QueryPhrase>();

            LogicalQueryPhrase byEntityNamePhrase = new LogicalQueryPhrase(FieldMetadata.ENTITY_NAME_FIELD, "story");
            queryPhrases.Add(byEntityNamePhrase);

            EntityListResult<FieldMetadata> result = entityService.Get<FieldMetadata>(workspaceContext, queryPhrases, null);
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

        private static Story CreateStory()
        {
            String name = "Story" + Guid.NewGuid();
            Story story = new Story();
            story.Name = name;
            story.Phase = PHASE_NEW;

            story.Parent = WORK_ITEM_ROOT;
            Story created = entityService.Create<Story>(workspaceContext, story, TestHelper.NameSubtypeFields);
            Assert.AreEqual<String>(name, created.Name);
            Assert.IsTrue(!string.IsNullOrEmpty(created.Id));
            return created;
        }
    }
}
