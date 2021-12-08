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


using MicroFocus.Adm.Octane.Api.Core.Entities;
using MicroFocus.Adm.Octane.Api.Core.Entities.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace MicroFocus.Adm.Octane.Api.Core.Tests
{
    [TestClass]
    public class AttachmentTests : BaseTest
    {


        [TestMethod]
        public void AttachToDefect()
        {
            string fileName = "test.txt";
            string fileContents = "This is a test for attachments\r\nChecking that it's working";
            byte[] fileContentsBytes = Encoding.UTF8.GetBytes(fileContents);

            Defect createdDefect = DefectTests.CreateDefect();
            Attachment attachment = entityService.AttachToEntity(workspaceContext, createdDefect, fileName, fileContentsBytes, "text/plain", new string[] { "owner_work_item" });
            Assert.IsNotNull(attachment.Id);
            Assert.AreEqual(attachment.owner_work_item.TypeName, "defect");
            Assert.AreEqual(attachment.owner_work_item.Id, createdDefect.Id);

            var relativeUrl = workspaceContext.GetPath() + "/attachments/" + attachment.Id + "/" + fileName;
            var filePath = Path.GetTempPath() + "\\" + Guid.NewGuid() + ".txt";
            entityService.DownloadAttachmentAsync(relativeUrl, filePath).Wait();

            using (var sr = new StreamReader(filePath))
            {
                Assert.AreEqual(fileContents, sr.ReadToEnd(), "Mismatched file contents");
            }
        }
    }
}
