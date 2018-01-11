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
            string raw = QueryStringBuilder.BuildQueryString(phrases);

            Assert.AreEqual("query=\"parent_suite={null}\"", raw);
        }
    }
}
