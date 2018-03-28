﻿/*!
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
using System.Collections.Generic;

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
            StoryUtilities.CreateStory(entityService, workspaceContext);
        }

        [TestMethod]
        public void GetAllStories()
        {
            StoryUtilities.CreateStory(entityService, workspaceContext);

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
    }
}
