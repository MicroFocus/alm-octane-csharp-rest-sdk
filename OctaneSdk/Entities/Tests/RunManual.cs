/*
 * Copyright 2016-2023 Open Text.
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

