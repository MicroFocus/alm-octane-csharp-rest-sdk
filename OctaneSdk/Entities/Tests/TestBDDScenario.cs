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


using MicroFocus.Adm.Octane.Api.Core.Services.Attributes;

namespace MicroFocus.Adm.Octane.Api.Core.Entities
{
    /// <summary>
    /// Wrapper for Scenario Test entity.
    /// </summary>
    [CustomEntityPath(SUBTYPE_BDD_SCENARIO_TEST)]
    public class TestBDDScenario : Test
    {
        public const string SUBTYPE_BDD_SCENARIO_TEST = "scenario_test";

        public const string BDD_SPEC_FIELD = "bdd_spec";


        public TestBDDScenario()
            : base()
        {
        }

        public TestBDDScenario(EntityId id)
            : base(id)
        {
        }

    }
}

