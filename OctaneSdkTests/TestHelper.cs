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
using MicroFocus.Adm.Octane.Api.Core.Services;
using MicroFocus.Adm.Octane.Api.Core.Services.Query;
using MicroFocus.Adm.Octane.Api.Core.Services.RequestContext;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MicroFocus.Adm.Octane.Api.Core.Tests
{
	public static class TestHelper
    {

        public static Phase GetPhaseForEntityByLogicalName(EntityService entityService, WorkspaceContext workspaceContext, string entityTypeName, string logicalName)
        {
            List<QueryPhrase> queryPhrases = new List<QueryPhrase>();
            LogicalQueryPhrase byEntityPhrase = new LogicalQueryPhrase(Phase.ENTITY_FIELD, entityTypeName);
            LogicalQueryPhrase byNamePhrase = new LogicalQueryPhrase(Phase.LOGICAL_NAME_FIELD, logicalName);
            queryPhrases.Add(byEntityPhrase);
            queryPhrases.Add(byNamePhrase);
            EntityListResult<Phase> result = entityService.Get<Phase>(workspaceContext, queryPhrases, null);
            Assert.AreEqual(1, result.total_count);
            Phase phase = result.data[0];
            return phase;
        }

        public static WorkItemRoot GetWorkItemRoot(EntityService entityService, WorkspaceContext workspaceContext)
        {
            List<string> fields = new List<string>() { Phase.NAME_FIELD };
            EntityListResult<WorkItemRoot> result = entityService.Get<WorkItemRoot>(workspaceContext, null, fields);
            Assert.AreEqual(1, result.total_count);
            WorkItemRoot root = result.data[0];
            return root;
        }

        public static ListNode GetSeverityByName(EntityService entityService, WorkspaceContext workspaceContext, String name)
        {
            string suffix = name.ToLower().Replace(" ", "_");
            string logicalName = "list_node.severity." + suffix;
            List<QueryPhrase> queryPhrases = new List<QueryPhrase>();
            LogicalQueryPhrase byLogicalName = new LogicalQueryPhrase(ListNode.LOGICAL_NAME_FIELD, logicalName);
            queryPhrases.Add(byLogicalName);

           // LogicalQueryPhrase byName = new LogicalQueryPhrase(ListNode.NAME_FIELD, name);
            //queryPhrases.Add(byName);

            List<string> fields = new List<string>() { Phase.NAME_FIELD, Phase.LOGICAL_NAME_FIELD };

            EntityListResult<ListNode> result = entityService.Get<ListNode>(workspaceContext, queryPhrases, fields);
            Assert.AreEqual(1, result.total_count);
            ListNode listNode = result.data[0];
            return listNode;
        }

        public static string[] NameFields = new string[] { "name" };
        public static string[] NameSubtypeFields = new string[] { "name", "subtype" };
    }
}
