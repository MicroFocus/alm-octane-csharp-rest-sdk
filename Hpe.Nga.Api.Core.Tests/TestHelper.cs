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
using Hpe.Nga.Api.Core.Services.RequestContext;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hpe.Nga.Api.Core.Tests
{
    public static class TestHelper
    {

        public static Phase GetPhaseForEntityByName(EntityService entityService, WorkspaceContext workspaceContext, String entityTypeName, String name)
        {
            List<QueryPhrase> queryPhrases = new List<QueryPhrase>();
            LogicalQueryPhrase byEntityPhrase = new LogicalQueryPhrase(Phase.ENTITY_FIELD, entityTypeName);
            LogicalQueryPhrase byNamePhrase = new LogicalQueryPhrase(Phase.NAME_FIELD, name);
            queryPhrases.Add(byEntityPhrase);
            queryPhrases.Add(byNamePhrase);

            List<String> fields = new List<String>() { Phase.NAME_FIELD, Phase.LOGICAL_NAME_FIELD };
            EntityListResult<Phase> result = entityService.Get<Phase>(workspaceContext, queryPhrases, fields);
            Assert.AreEqual(1, result.total_count);
            Phase phase = result.data[0];
            return phase;
        }

        public static WorkItemRoot GetWorkItemRoot(EntityService entityService, WorkspaceContext workspaceContext)
        {
            List<String> fields = new List<String>() { Phase.NAME_FIELD };
            EntityListResult<WorkItemRoot> result = entityService.Get<WorkItemRoot>(workspaceContext, null, fields);
            Assert.AreEqual(1, result.total_count);
            WorkItemRoot root = result.data[0];
            return root;
        }

        public static ListNode GetSeverityByName(EntityService entityService, WorkspaceContext workspaceContext, String name)
        {
            String suffix = name.ToLower().Replace(" ", "_");
            String logicalName = "list_node.severity." + suffix;
            List<QueryPhrase> queryPhrases = new List<QueryPhrase>();
            LogicalQueryPhrase byLogicalName = new LogicalQueryPhrase(ListNode.LOGICAL_NAME_FIELD, logicalName);
            queryPhrases.Add(byLogicalName);

           // LogicalQueryPhrase byName = new LogicalQueryPhrase(ListNode.NAME_FIELD, name);
            //queryPhrases.Add(byName);

            List<String> fields = new List<String>() { Phase.NAME_FIELD, Phase.LOGICAL_NAME_FIELD };

            EntityListResult<ListNode> result = entityService.Get<ListNode>(workspaceContext, queryPhrases, fields);
            Assert.AreEqual(1, result.total_count);
            ListNode listNode = result.data[0];
            return listNode;
        }
    }
}
