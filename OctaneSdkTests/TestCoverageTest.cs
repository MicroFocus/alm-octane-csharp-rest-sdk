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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroFocus.Adm.Octane.Api.Core.Tests
{
    [TestClass]
    public class TestCoverageTest : BaseTest
    {
        [TestMethod]
        public void  DefectCoverageTest()
        {
            Test createdTest1 = TestManualTests.CreateManualTest();
            Test createdTest2 = TestManualTests.CreateManualTest();
            Test createdTest3 = TestManualTests.CreateManualTest();
            Defect createdDefect = DefectTests.CreateDefect();

            //create coverage
            Defect defectForUpdate = new Defect(createdDefect.Id);

            //add tests

            EntityList<Test> testList = new EntityList<Test>();
            Test testForLink1 = new Test(createdTest1.Id);
            testForLink1.TypeName = "test";
            testList.data.Add(testForLink1);

            Test testForLink2 = new Test(createdTest3.Id);
            testForLink2.TypeName = "test";
            testList.data.Add(testForLink2);

            defectForUpdate.SetValue("test", testList);

            //define service arg, its required to add new relation instead of replace existing ones
            Dictionary<string, string> serviceArgs = new Dictionary<string, string>();
            serviceArgs.Add("reference_update_mode", "append");

            //actial update
            Defect updated = entityService.Update(workspaceContext, defectForUpdate, serviceArgs, null);


            //get test coverage
            List<QueryPhrase> queryPhrases = new List<QueryPhrase>();
            QueryPhrase defectIdPhrase = new LogicalQueryPhrase(Release.ID_FIELD, createdDefect.Id);
            QueryPhrase coveredContentPhrase = new CrossQueryPhrase("covered_content", defectIdPhrase);

            queryPhrases.Add(coveredContentPhrase);
            EntityListResult<Test> coveredTests = entityService.Get<Test>(workspaceContext, queryPhrases, null);
            Assert.AreEqual(2, coveredTests.data.Count);

            //validation
            List<EntityId> coveredTestIds = coveredTests.data.ConvertAll<EntityId>(a => a.Id);
            Assert.IsTrue(coveredTestIds.Contains(testForLink1.Id));
            Assert.IsTrue(coveredTestIds.Contains(testForLink2.Id));
        }
    }
}
