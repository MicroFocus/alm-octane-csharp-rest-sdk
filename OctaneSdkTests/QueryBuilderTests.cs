/*!
* (c) Copyright 2021 Micro Focus or one of its affiliates.
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
using MicroFocus.Adm.Octane.Api.Core.Services.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MicroFocus.Adm.Octane.Api.Core.Tests
{
    [TestClass]
    public class QueryBuilderTests
    {
        [TestMethod]
        public void BuildCrossFieldNullPhrase()
        {
            var phrase = new CrossQueryPhrase("parent_suite", NullQueryPhrase.Null);
            var phrases = new List<QueryPhrase>(new[] { phrase });
            string raw = QueryBuilder.BuildQueryString(phrases);

            Assert.AreEqual("query=\"parent_suite={null}\"", raw);
        }

        [TestMethod]
        public void BuildQueryWithOrderBy()
        {
            var query = QueryBuilder.Create().SetOrderBy("-id").Build();

            Assert.IsTrue(query.Contains("order_by=-id"));
        }

        [TestMethod]
        public void BuildCrossQuery1()
        {
            var queryPhrases = new List<QueryPhrase>
            {
                new CrossQueryPhrase("linked_items1",
                     new LogicalQueryPhrase(WorkItem.SUBTYPE_FIELD, WorkItem.SUBTYPE_DEFECT)),
                 new CrossQueryPhrase("linked_items1",
                     new CrossQueryPhrase("defect_type",
                         new LogicalQueryPhrase("name", "Clone"))),
                 new CrossQueryPhrase("linked_items1",
                     new CrossQueryPhrase("target_release_list_udf",
                         new LogicalQueryPhrase("id", 1)))
            };



            var query = QueryBuilder.Create().SetQueryPhrases(queryPhrases).Build();
            var expected = "&query=\"linked_items1={subtype='defect'};linked_items1={defect_type={name='Clone'}};linked_items1={target_release_list_udf={id=1}}\"";
            Assert.AreEqual(expected, query);

        }

        [TestMethod]
        public void BuildCrossQuery2()
        {
            var queryPhrases = new List<QueryPhrase>
            {
                new CrossQueryPhrase("linked_items1",
                    new  List<QueryPhrase>(){
                     new LogicalQueryPhrase(WorkItem.SUBTYPE_FIELD, WorkItem.SUBTYPE_DEFECT),
                     new CrossQueryPhrase("defect_type", new LogicalQueryPhrase("name", "Clone")),
                     new CrossQueryPhrase("target_release_list_udf",new LogicalQueryPhrase("id", 1))
                    })
            };

            var query = QueryBuilder.Create().SetQueryPhrases(queryPhrases).Build();
            var expected = "&query=\"linked_items1={subtype='defect';defect_type={name='Clone'};target_release_list_udf={id=1}}\"";
            Assert.AreEqual(expected, query);

        }

        [TestMethod]
        public void BuildExpandFields()
        {
            List<string> fields = new List<string>() { "abc", "def", "egh", "frt" };
            List<ExpandField> expandFields = new List<ExpandField>() {
                ExpandField.Create("def", new List<string>() { "a1", "a2","a3" }),
                ExpandField.Create("frt", new List<string>() { "b1" })};

            var query = QueryBuilder.Create().SetFields(fields).SetExpandFields(expandFields).Build();
            var expected = "&fields=abc,def{a1,a2,a3},egh,frt{b1}";
            Assert.AreEqual(expected, query);

        }
    }
}
