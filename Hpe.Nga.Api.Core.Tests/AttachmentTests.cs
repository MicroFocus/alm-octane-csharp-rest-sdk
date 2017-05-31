// (c) Copyright 2016 Hewlett Packard Enterprise Development LP

// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.

// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,

// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Hpe.Nga.Api.Core.Connector.Exceptions;
using Hpe.Nga.Api.Core.Entities;
using Hpe.Nga.Api.Core.Services;
using Hpe.Nga.Api.Core.Services.GroupBy;
using Hpe.Nga.Api.Core.Services.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;
using Hpe.Nga.Api.Core.Entities.Base;

namespace Hpe.Nga.Api.Core.Tests
{
    [TestClass]
    public class AttachmentTests : BaseTest
    {
        private static Phase phaseNew;
        private static ListNode severityHigh;
        private static WorkItemRoot workItemRoot;

        private static ListNode getSeverityHigh()
        {
            if (severityHigh == null)
            {
                severityHigh = TestHelper.GetSeverityByName(entityService, workspaceContext, "High");
            }
            return severityHigh;
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
        public void AttachToDefect()
        {
            string fileName = "test.txt";
            string fileContents = "This is a test for attachments\r\nChecking that it's working";
            byte[] fileContentsBytes = Encoding.UTF8.GetBytes(fileContents);
            
            Defect createdDefect = CreateDefect();
            Attachment attachment = entityService.AttachToEntity(workspaceContext, createdDefect, fileName, fileContentsBytes, "text/plain");
            Assert.IsNotNull(attachment.Id);
            Assert.AreEqual(attachment.owner_work_item.TypeName, "work_item");
            Assert.AreEqual(attachment.owner_work_item.Id, createdDefect.Id);
        }

        private static Defect CreateDefect()
        {
            return CreateDefect(getPhaseNew());

        }

        private static Defect CreateDefect(Phase phase)
        {
            String name = "Defect" + Guid.NewGuid();
            Defect defect = new Defect();
            defect.Name = name;
            defect.Phase = phase;
            defect.Severity = getSeverityHigh();
            defect.Parent = getWorkItemRoot();
            Defect created = entityService.Create<Defect>(workspaceContext, defect);
            Assert.AreEqual<String>(name, created.Name);
            Assert.IsTrue(created.Id > 0);
            return created;

        }
    }
}
