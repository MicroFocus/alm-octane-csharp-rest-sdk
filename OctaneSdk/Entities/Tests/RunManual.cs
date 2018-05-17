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
    /// Wrapper for manual test run entity.
    /// </summary>
    [CustomCollectionPath("manual_runs")]
    public class RunManual : Run
    {
        public const string SUBTYPE_RUN_MANUAL = "run_manual";

        public const string HAS_VISUAL_COVERAGE_FIELD = "has_visual_coverage";
        public const string RUN_BY_FIELD = "run_by";
        public const string RELEASE_FIELD = "release";
        public const string NATIVE_STATUS_FIELD = "native_status";

        public RunManual()
            : base()
        {
        }

        public RunManual(EntityId id)
            : base(id)
        {
        }

        public WorkspaceUser RunBy
        {
            get
            {
                return (WorkspaceUser)GetValue(RUN_BY_FIELD);
            }
            set
            {
                SetValue(RUN_BY_FIELD, value);
            }
        }

        public Release Release
        {
            get
            {
                return (Release)GetValue(RELEASE_FIELD);
            }
            set
            {
                SetValue(RELEASE_FIELD, value);
            }
        }

        public TestManual Parent
        {
            get
            {
                return (TestManual)GetValue(TEST_FIELD);
            }
            set
            {
                SetValue(TEST_FIELD, value);
            }
        }

        public BaseEntity NativeStatus
        {
            get
            {
                return (BaseEntity)GetValue(NATIVE_STATUS_FIELD);
            }
            set
            {
                SetValue(NATIVE_STATUS_FIELD, value);
            }
        }
    }
}

