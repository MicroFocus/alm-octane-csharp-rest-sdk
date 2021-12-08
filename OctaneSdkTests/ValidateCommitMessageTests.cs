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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Web;

namespace MicroFocus.Adm.Octane.Api.Core.Tests
{
    [TestClass]
    public class ValidateCommitMessageTests : BaseTest
    {
        [TestMethod]
        public void ValidateCommitMessageForStory()
        {
            var pattern = entityService.ValidateCommitMessage(workspaceContext, HttpUtility.UrlEncode("user story #1234", Encoding.UTF8));
            Assert.IsTrue(pattern.story.Contains("1234"));
            Assert.AreEqual(1, pattern.story.Count);
            Assert.AreEqual(0, pattern.quality_story.Count);
            Assert.AreEqual(0, pattern.defect.Count);
        }

        [TestMethod]
        public void InvalidCommitMessage()
        {
            var pattern = entityService.ValidateCommitMessage(workspaceContext, HttpUtility.UrlEncode("invalid #1234", Encoding.UTF8));
            Assert.AreEqual(0, pattern.story.Count);
            Assert.AreEqual(0, pattern.quality_story.Count);
            Assert.AreEqual(0, pattern.defect.Count);
        }
    }
}
