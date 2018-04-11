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


using MicroFocus.Adm.Octane.Api.Core.Connector.Exceptions;
using MicroFocus.Adm.Octane.Api.Core.Entities;
using MicroFocus.Adm.Octane.Api.Core.Services;
using MicroFocus.Adm.Octane.Api.Core.Services.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MicroFocus.Adm.Octane.Api.Core.Tests
{
    [TestClass]
    public class ReleaseTests : BaseTest
    {

        [TestMethod]
        public void GetReleaseFieldMetadata()
        {
            ListResult<FieldMetadata> result = entityService.GetFieldsMetadataAsync(workspaceContext, "release").Result;
            Assert.IsTrue(result.total_count >= 1);
        }

        [TestMethod]
        public void CreateReleaseTest()
        {
            Release created = CreateRelease();
            Assert.IsNotNull(created);
        }

        [TestMethod]
        public void GetReleaseByIdTest()
        {
            Release created = CreateRelease();
            List<String> fields = new List<string>();
            fields.Add(Release.NAME_FIELD);
            Release release = entityService.GetById<Release>(workspaceContext, created.Id, fields);
            Assert.AreEqual(release.Id, created.Id);
        }

        [TestMethod]
        public void GetReleasesByNameTest()
        {
            Release release1 = CreateRelease();
            Release release2 = CreateRelease();


            LogicalQueryPhrase namePhrase = new LogicalQueryPhrase(Release.NAME_FIELD);
            namePhrase.AddExpression(release1.Name, ComparisonOperator.Equal);
            namePhrase.AddExpression(release2.Name, ComparisonOperator.Equal);

            List<QueryPhrase> queryPhrases = new List<QueryPhrase>();
            queryPhrases.Add(namePhrase);

            EntityListResult<Release> result = entityService.Get<Release>(workspaceContext, queryPhrases, null);
            Assert.AreEqual<int>(2, result.data.Count);

        }

        [TestMethod]
        public void GetAllReleasesTest()
        {
            Release created = CreateRelease();

            List<String> fields = new List<string>();
            fields.Add(Release.NAME_FIELD);
            fields.Add(Release.START_DATE_FIELD);
            fields.Add(Release.END_DATE_FIELD);


            EntityListResult<Release> result = entityService.Get<Release>(workspaceContext, null, fields);
            Assert.IsTrue(result.data.Count >= 1);
        }


        [TestMethod]
        public void UpdateReleaseNameTest()
        {
            Release created = CreateRelease();

            //Prepare for update
            String name = "ReleaseUpdated_" + Guid.NewGuid();
            Release release = new Release(created.Id);
            release.Name = name;

            Release updated = entityService.Update<Release>(workspaceContext, release, TestHelper.NameFields);
            Assert.AreEqual<String>(updated.Name, name);
        }

        [TestMethod]
        public void DeleteReleaseTest()
        {
            Release created = CreateRelease();
            entityService.DeleteById<Release>(workspaceContext, created.Id);
            try
            {
                //try read release
                Release release = entityService.GetById<Release>(workspaceContext, created.Id, null);
                Assert.Fail("Shouldnot get here");
            }
            catch (MqmRestException e)
            {
                Assert.AreEqual<string>("platform.entity_not_found", e.ErrorCode);
            }

        }



        [TestMethod]
        public void CreateMilestoneTest()
        {
            Release release = CreateRelease();
            Milestone created = CreateMilestone(release);
            Assert.IsNotNull(created);
        }

        [TestMethod]
        public void GetMilestonesByReleaseTest()
        {
            Release release = CreateRelease();
            Milestone created1 = CreateMilestone(release);
            Milestone created2 = CreateMilestone(release);


            List<QueryPhrase> queryPhrases = new List<QueryPhrase>();
            QueryPhrase releaseIdPhrase = new LogicalQueryPhrase(Release.ID_FIELD, release.Id);
            QueryPhrase byReleasePhrase = new CrossQueryPhrase(Milestone.RELEASES_FIELD, releaseIdPhrase);

            queryPhrases.Add(byReleasePhrase);
            EntityListResult<Milestone> result = entityService.Get<Milestone>(workspaceContext, queryPhrases, null);
            Assert.AreEqual<int>(2, result.data.Count);
        }

        [TestMethod]
        public void GetSprintsByReleaseTest()
        {
            Release release = CreateRelease();

            List<String> fields = new List<string>();
            fields.Add(Sprint.NAME_FIELD);
            fields.Add(Sprint.START_DATE_FIELD);
            fields.Add(Sprint.END_DATE_FIELD);
            fields.Add(Sprint.RELEASE_FIELD);

            List<QueryPhrase> queryPhrases = new List<QueryPhrase>();
            QueryPhrase releaseIdPhrase = new LogicalQueryPhrase(Release.ID_FIELD, release.Id);
            QueryPhrase byReleasePhrase = new CrossQueryPhrase(Sprint.RELEASE_FIELD, releaseIdPhrase);

            queryPhrases.Add(byReleasePhrase);

            EntityListResult<Sprint> result = entityService.Get<Sprint>(workspaceContext, queryPhrases, fields);
            Assert.IsTrue(result.data.Count >= 1);
            Release releaseFromSprint = result.data[0].Release;
            Assert.AreEqual(release.Id, releaseFromSprint.Id);
        }


        private static Release CreateRelease()
        {
            String name = "Release_" + Guid.NewGuid();
            Release release = new Release();
            release.Name = name;
            release.StartDate = DateTime.Now;
            release.EndDate = DateTime.Now.AddDays(10);
            release.SprintDuration = 7;


            Release created = entityService.Create<Release>(workspaceContext, release, TestHelper.NameFields);
            Assert.AreEqual<String>(name, created.Name);
            return created;
        }

        private static Milestone CreateMilestone(Release release)
        {
            String name = "Milestone_" + Guid.NewGuid();
            Milestone milestone = new Milestone();
            milestone.Name = name;
            milestone.Date = DateTime.Now.AddDays(7);
            milestone.SetRelease(new EntityList<Release>(release));


            Milestone created = entityService.Create<Milestone>(workspaceContext, milestone, TestHelper.NameFields);
            Assert.AreEqual<String>(name, created.Name);
            return created;
        }
    }

}
