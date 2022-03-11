/*!
* (c) Copyright 2016-2021 Micro Focus or one of its affiliates.
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


using MicroFocus.Adm.Octane.Api.Core.Connector;
using MicroFocus.Adm.Octane.Api.Core.Entities;
using MicroFocus.Adm.Octane.Api.Core.Entities.Base;
using MicroFocus.Adm.Octane.Api.Core.Services;
using MicroFocus.Adm.Octane.Api.Core.Services.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MicroFocus.Adm.Octane.Api.Core.Tests
{
    [TestClass]
    public class NonGenericsEntityServiceTests : BaseTest
    {
        protected static NonGenericsEntityService nonGenericsEntityService = new NonGenericsEntityService(restConnector);

        private static JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();

        private static TestManual _testManual;

        private static TestAutomated _testAutomated;

        private static TestSuite _testSuite;

        private static RunManual _runManual;

        private static Attachment _attachment;

        private static RunSuite _runSuite;

        private static Defect _defect;

        private static string FILENAME = "test.txt";
        private static string FILECONTENT = "This is a test for attachments\r\nChecking that it's working";

        private static string TEST_SCRIPT_COLLECTION_NAME = "test_versions";

        static NonGenericsEntityServiceTests()
        {
            jsonSerializer.RegisterConverters(new JavaScriptConverter[] { new EntityJsonConverter(), new EntityIdJsonConverter() });
        }

        [TestMethod]
        public void GetAllTestsManualTest()
        {
            getManualTest();

            string collectionName = EntityTypeRegistry.GetInstance().GetCollectionName(typeof(TestManual));
            EntityListResult<BaseEntity> testsManual = nonGenericsEntityService.Get(workspaceContext, collectionName, "", TestHelper.NameFields);
            Assert.IsTrue(testsManual.total_count > 0);
        }

        [TestMethod]
        public void GetOneTestManualTest()
        {
            TestManual test = getManualTest();

            BaseEntity foundTest = nonGenericsEntityService.GetById(workspaceContext, "test_manual", test.Id, TestHelper.NameFields);
            Assert.IsTrue(!string.IsNullOrEmpty(foundTest.Id));
            Assert.AreEqual<string>(test.Name, foundTest.Name);
        }

        [TestMethod]
        public void GetAllTestsAutomatedTest()
        {
            TestAutomated test = getTestAutomated();

            string collectionName = EntityTypeRegistry.GetInstance().GetCollectionName(typeof(TestAutomated));
            EntityListResult<BaseEntity> testsAutomated = nonGenericsEntityService.Get(workspaceContext, collectionName, "", TestHelper.NameFields);
            Assert.IsTrue(testsAutomated.total_count > 0);
        }

        [TestMethod]
        public void GetOneTestAutomatedTest()
        {
            TestAutomated test = getTestAutomated();

            BaseEntity foundTest = nonGenericsEntityService.GetById(workspaceContext, "test_automated", test.Id, TestHelper.NameFields);
            Assert.IsTrue(!string.IsNullOrEmpty(foundTest.Id));
            Assert.AreEqual<string>(test.Name, foundTest.Name);
        }

        [TestMethod]
        public void GetAllTestSuitesTest()
        {
            getTestSuite();

            string collectionName = EntityTypeRegistry.GetInstance().GetCollectionName(typeof(TestSuite));
            EntityListResult<BaseEntity> testSuites = nonGenericsEntityService.Get(workspaceContext, collectionName, "", TestHelper.NameFields);
            Assert.IsTrue(testSuites.total_count > 0);
        }

        [TestMethod]
        public void GetOneTestSuiteTest()
        {
            TestSuite testSuite = getTestSuite();

            BaseEntity foundTestSuite = nonGenericsEntityService.GetById(workspaceContext, "test_suite", testSuite.Id, TestHelper.NameFields);
            Assert.IsTrue(!string.IsNullOrEmpty(foundTestSuite.Id));
            Assert.AreEqual<string>(testSuite.Name, foundTestSuite.Name);
        }

        [TestMethod]
        public void GetAllManualRunsTest()
        {
            RunManual runManual = getManualRun();

            string collectionName = EntityTypeRegistry.GetInstance().GetCollectionName(typeof(RunManual));
            EntityListResult<BaseEntity> runManuals = nonGenericsEntityService.Get(workspaceContext, collectionName, "", TestHelper.NameFields);
            Assert.IsTrue(runManuals.total_count > 0);
        }

        [TestMethod]
        public void GetOneManualRunTest()
        {
            RunManual runManual = getManualRun();

            BaseEntity foundRunManual = nonGenericsEntityService.GetById(workspaceContext, "run_manual", runManual.Id, TestHelper.NameFields);
            Assert.IsTrue(!string.IsNullOrEmpty(foundRunManual.Id));
            Assert.AreEqual<string>(runManual.Name, foundRunManual.Name);
        }

        [TestMethod]
        public void GetAllAttachmentsTest()
        {
            Attachment attachment = getSimpleAttachment(FILENAME, FILECONTENT);

            string collectionName = EntityTypeRegistry.GetInstance().GetCollectionName(typeof(Attachment));
            EntityListResult<BaseEntity> attachments = nonGenericsEntityService.Get(workspaceContext, collectionName, "", TestHelper.NameFields);
            Assert.IsTrue(attachments.total_count > 0);
        }

        [TestMethod]
        public void GetOneAttachmentTest()
        {
            Attachment attachment = getSimpleAttachment(FILENAME, FILECONTENT);

            BaseEntity foundAttachment = nonGenericsEntityService.GetById(workspaceContext, "attachment", attachment.Id, TestHelper.NameFields);
            Assert.IsTrue(!string.IsNullOrEmpty(attachment.Id));
        }

        [TestMethod]
        public void DownloadAttachmentTest()
        {
            string fileName = FILENAME;
            string fileContents = FILECONTENT;

            Attachment attachment = getSimpleAttachment(fileName, fileContents);

            var relativeUrl = workspaceContext.GetPath() + "/attachments/" + attachment.Id + "/" + fileName;
            var filePath = Path.GetTempPath() + "\\" + Guid.NewGuid() + ".txt";

            nonGenericsEntityService.DownloadAttachment(relativeUrl, filePath);

            using (var sr = new StreamReader(filePath))
            {
                Assert.AreEqual(fileContents, sr.ReadToEnd(), "Mismatched file contents");
            }
        }

        [TestMethod]
        public void GetAllTestScriptsTest()
        {
            TestManual testManual = getManualTest();

            string collectionName = TEST_SCRIPT_COLLECTION_NAME;
            EntityListResult<BaseEntity> testScripts = nonGenericsEntityService.Get(workspaceContext, collectionName, "", new string[] { "script", "test" });
            Assert.IsTrue(testScripts.total_count > 0);
        }

        [TestMethod]
        public void GetOneTestScriptTest()
        {
            TestManual testManual = getManualTest();

            string collectionName = TEST_SCRIPT_COLLECTION_NAME;
            string testKey = "test";
            string query = String.Format("({3}={0}id={2}{1})", "{", "}", testManual.Id, testKey);
            EntityListResult<BaseEntity> testScripts = nonGenericsEntityService.Get(workspaceContext, collectionName, query, new string[] { "script", testKey });
            Assert.IsTrue(testScripts.total_count > 0);
            BaseEntity testScript = testScripts.data[0];
            object testManualObj = testScript.GetValue(testKey);
            string id = testManualObj.GetType().GetProperty("Id").GetValue(testManualObj).ToString();
            Assert.AreEqual(id, testManual.Id.ToString());
        }

        [TestMethod]
        public void GetAllSuiteRunsTest()
        {
            RunSuite runSuite = getSuiteRun();

            string collectionName = EntityTypeRegistry.GetInstance().GetCollectionName(typeof(RunSuite));
            EntityListResult<BaseEntity> runSuites = nonGenericsEntityService.Get(workspaceContext, collectionName, "", TestHelper.NameFields);
            Assert.IsTrue(runSuites.total_count > 0);
        }

        [TestMethod]
        public void GetOneSuiteRunTest()
        {
            RunSuite runSuite = getSuiteRun();

            BaseEntity foundRunSuite = nonGenericsEntityService.GetById(workspaceContext, "run_suite", runSuite.Id, TestHelper.NameFields);
            Assert.IsTrue(!string.IsNullOrEmpty(foundRunSuite.Id));
            Assert.AreEqual<string>(runSuite.Name, foundRunSuite.Name);
        }

        [TestMethod]
        public void GetTestsOfTestSuiteTest()
        {
            TestManual testManual = getManualTest();
            TestSuite testSuite = getTestSuite();
            string url = workspaceContext.GetPath() + "/test_suite_link_to_tests";
            string body = "{\"data\":[{\"subtype\":\"test_suite_link_to_manual\",\"test\":{\"type\":\"test\",\"id\":\"" + testManual.Id + "\"},\"test_suite\":{\"type\":\"test\",\"id\":\"" + testSuite.Id + "\"}}]}";
            EntityListResult<BaseEntity> createResponse = RawQuery(true, url, body, new string[] { BaseEntity.ID_FIELD }).Result;
            Assert.IsTrue(createResponse.total_count > 0);

            string collectionName = "test_suite_link_to_tests";
            string query = "(test_suite={id=" + testSuite.Id + "};!test={null})";
            EntityListResult<BaseEntity> tests = nonGenericsEntityService.Get(workspaceContext, collectionName, query, new string[] { BaseEntity.ID_FIELD });
            Assert.IsTrue(tests.total_count > 0);
        }

        [TestMethod]
        public void GetRunsOfTestTest()
        {
            RunManual rm = getManualRun();
            Assert.IsTrue(!string.IsNullOrEmpty(rm.Id));

            BaseEntity runManualAsBaseEntity = nonGenericsEntityService.GetById(workspaceContext, "run_manual", rm.Id, new string[] { "name", "test" });
            RunManual runManual = new RunManual();
            runManual.SetProperties(runManualAsBaseEntity.GetProperties());
            TestManual testManual = runManual.Parent;

            string collectionName = EntityTypeRegistry.GetInstance().GetCollectionName(typeof(RunManual));
            string query = "((subtype+IN+'run_manual','run_automated');((test={(covered_manual_test={id=" + testManual.Id + "})})||(test={id=" + testManual.Id + "})))";
            EntityListResult<BaseEntity> runs = nonGenericsEntityService.Get(workspaceContext, collectionName, query, new string[] { BaseEntity.ID_FIELD });
            Assert.IsTrue(runs.total_count == 1);
            Assert.AreEqual<string>(runs.data[0].Id, runManual.Id);
        }

        private static Attachment getSimpleAttachment(string fileName, string fileContents)
        {
            if (_attachment == null)
            {
                TestManual testManual = getManualTest();
                byte[] fileContentsBytes = Encoding.UTF8.GetBytes(fileContents);
                string ownerField = string.Format("owner_{0}", testManual.AggregateType);
                Attachment created = entityService.AttachToEntity(workspaceContext, testManual, fileName, fileContentsBytes, "text/plain", new string[] { ownerField });
                Assert.IsNotNull(created.Id);
                // TODO Add parent validation
                _attachment = created;
            }
            return _attachment;
        }

        private static TestManual getManualTest()
        {
            if (_testManual == null)
            {
                _testManual = TestManualTests.CreateManualTest();
            }
            return _testManual;
        }

        private static TestAutomated getTestAutomated()
        {
            if (_testAutomated == null)
            {
                string name = "TestAutomated-" + Guid.NewGuid();
                TestAutomated test = new TestAutomated();
                test.Name = name;

                TestAutomated created = entityService.Create<TestAutomated>(workspaceContext, test, TestHelper.NameFields);
                Assert.AreEqual<string>(name, created.Name);
                Assert.IsTrue(!string.IsNullOrEmpty(created.Id));

                _testAutomated = created;
            }
            return _testAutomated;
        }

        private static TestSuite getTestSuite()
        {
            if (_testSuite == null)
            {
                string name = "TestSuite-" + Guid.NewGuid();
                TestSuite suite = new TestSuite();
                suite.Name = name;

                TestSuite created = entityService.Create<TestSuite>(workspaceContext, suite, TestHelper.NameFields);
                Assert.AreEqual<string>(name, created.Name);
                Assert.IsTrue(!string.IsNullOrEmpty(created.Id));

                _testSuite = created;
            }
            return _testSuite;
        }

        private static RunManual getManualRun()
        {
            if (_runManual == null)
            {
                TestManual testManual = getManualTest();
                _runManual = RunManualTests.CreateManualRun(testManual);
            }
            return _runManual;
        }

        private static RunSuite getSuiteRun()
        {
            if (_runSuite == null)
            {
                _runSuite = RunSuiteTests.CreateSuiteRun();
            }
            return _runSuite;
        }

        private async Task<EntityListResult<BaseEntity>> RawQuery(bool isPost, string url, string body, IList<string> fieldsToReturn = null)
        {
            string queryParams = "";

            if (fieldsToReturn != null && fieldsToReturn.Count > 0)
            {
                queryParams += "fields=" + string.Join(",", fieldsToReturn);
            }

            ResponseWrapper response;
            if (isPost)
            {
                response = await restConnector.ExecutePostAsync(url, queryParams, body).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
            }
            else
            {
                response = await restConnector.ExecutePutAsync(url, queryParams, body).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
            }
            EntityListResult<BaseEntity> result = jsonSerializer.Deserialize<EntityListResult<BaseEntity>>(response.Data);
            return result;
        }
    }
}
