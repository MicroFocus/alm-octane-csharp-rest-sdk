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

namespace MicroFocus.Adm.Octane.Api.Core.Entities
{
    /// <summary>
    /// Wrapper for Comment entity.
    /// </summary>
    public class Comment : BaseEntity
    {
        public const string TYPE_COMMENT = "comment";

        public const string AUTHOR_FIELD = "author";
        public const string OWNER_WORK_FIELD = "owner_work_item";
        public const string OWNER_TEST_FIELD = "owner_test";
        public const string OWNER_RUN_FIELD = "owner_run";
        public const string OWNER_REQUIREMENT_FIELD = "owner_requirement";
        public const string OWNER_TASK = "owner_task";
        public const string OWNER_BDD_SPEC_FIELD = "owner_bdd_spec";
        public const string CREATION_TIME_FIELD = "creation_time";
        public const string TEXT_FIELD = "text";

        public Comment()
        {
        }

        public Comment(EntityId id)
            : base(id)
        {
        }

        public string Author
        {
            get { return GetStringValue(AUTHOR_FIELD); }
        }

        public BaseEntity OwnerWorkItem
        {
            get { return (BaseEntity)GetValue(OWNER_WORK_FIELD); }
            set
            {
                SetValue(OWNER_WORK_FIELD, value);
            }
        }


        public BaseEntity OwnerTest
        {
            get { return (BaseEntity)GetValue(OWNER_TEST_FIELD); }
            set
            {
                SetValue(OWNER_TEST_FIELD, value);
            }
        }

        public BaseEntity OwnerRun
        {
            get { return (BaseEntity)GetValue(OWNER_RUN_FIELD); }
            set
            {
                SetValue(OWNER_RUN_FIELD, value);
            }
        }

        public BaseEntity OwnerRequirement
        {
            get { return (BaseEntity)GetValue(OWNER_REQUIREMENT_FIELD); }
            set
            {
                SetValue(OWNER_REQUIREMENT_FIELD, value);
            }
        }

        public BaseEntity OwnerTask
        {
            get { return (BaseEntity)GetValue(OWNER_TASK); }
            set
            {
                SetValue(OWNER_TASK, value);
            }
        }

        public BaseEntity OwnerBDDSpec
        {
            get { return (BaseEntity)GetValue(OWNER_BDD_SPEC_FIELD); }
            set
            {
                SetValue(OWNER_BDD_SPEC_FIELD, value);
            }
        }

        public string CreationTime
        {
            get { return GetStringValue(CREATION_TIME_FIELD); }
        }

        public string Text
        {
            get { return GetStringValue(TEXT_FIELD); }
            set
            {
                SetValue(TEXT_FIELD, value);
            }
        }
    }
}
