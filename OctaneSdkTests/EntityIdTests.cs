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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace MicroFocus.Adm.Octane.Api.Core.Tests
{
	[TestClass]
    public class EntityIdTests
    {
        [TestMethod]
        public void EqualsEntityId()
        {
            EntityId a = new EntityId("hello");
            EntityId b = new EntityId("hello");

            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void NotEqualsEntityId()
        {
            EntityId a = new EntityId("hello");
            EntityId b = new EntityId("goodbye");

            Assert.AreNotEqual(a, b);
        }

        [TestMethod]
        public void EntityIdToString()
        {
            EntityId entityId = new EntityId("123");
            string stringValue = entityId.ToString();

            Assert.AreEqual("123", stringValue);
        }

        [TestMethod]
        public void EqualsOperator()
        {
            EntityId a = new EntityId("1abc2");
            EntityId b = new EntityId("1abc2");

            Assert.IsTrue(a == b);
        }

        [TestMethod]
        public void NotEqualsOperator()
        {
            EntityId a = new EntityId("1abc2");
            EntityId b = new EntityId("9xyz8");

            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void KeyInMap()
        {
            EntityId id1 = new EntityId("1111");
            EntityId id2 = new EntityId("2222");

            var map = new Dictionary<EntityId, int>
            {
                { id1, 20 }
            };

            // Retrieve by EntityId
            Assert.AreEqual(20, map[id1]);

            // Ensure key is not found
            Assert.IsFalse(map.ContainsKey(id2));
        }

        [TestMethod]
        public void ConvertIEnumerableOfEntityIdToListOfLong()
        {
            List<Test> l = new List<Test>
            {
                new Test("123"),
                new Test("456")
            };

            List<long> assignedWorkspaceRoles = l.Select(p => p.Id).ToList<long>();

            Assert.AreEqual(assignedWorkspaceRoles.Count, 2);
        }

        [TestMethod]
        public void ConvertIEnumerableOfEntityIdToListOfString()
        {
            List<Test> l = new List<Test>
            {
                new Test("123"),
                new Test("456")
            };

            List<string> assignedWorkspaceRoles = l.Select(p => p.Id).ToList<string>();

            Assert.AreEqual(assignedWorkspaceRoles.Count, 2);
        }
    }
}
