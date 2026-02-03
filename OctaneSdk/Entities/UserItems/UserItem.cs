/*
 * Copyright 2016-2024 Open Text.
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
    /// Wrapper for Run entity
    /// More fields might be supported by entity that still are not exposed in the class
    /// </summary>
    public class UserItem : BaseEntity
    {
        public static string ENTITY_TYPE_FIELD = "entity_type";
        public static string USER_FIELD = "user";
        public static string REASON_FIELD = "reason";
        public static string ORIGIN = "origin";
        public static string WORK_ITEM_REFERENCE = "my_follow_items_work_item";
        public static string MODEL_ITEM_REFERENCE = "my_follow_items_model_item";
        public static string PROCESS_REFERENCE = "my_follow_items_process";
        public static string TASK_REFERENCE = "my_follow_items_task";
        public static string TEST_REFERENCE = "my_follow_items_test";
        public static string RUN_REFERENCE = "my_follow_items_run";
        public static string REQUIREMENT_REFERENCE = "my_follow_items_requirement";
        public static string SUITE_RUN_SCHEDULER_REFERENCE = "my_follow_items_suite_run_scheduler";
        public static string SUITE_RUN_SCHEDULER_RUN_REFERENCE = "my_follow_items_suite_run_scheduler_run";

        public UserItem()
        {
        }

        public UserItem(EntityId id)
            : base(id)
        {
        }

        public WorkspaceUser User
        {
            get
            {
                return (WorkspaceUser)GetValue(USER_FIELD);
            }
        }

        public string Reason
        {
            get { return GetStringValue(REASON_FIELD); }
        }

        public BaseEntity WorkItem
        {
            get
            {
                return (BaseEntity)GetValue(WORK_ITEM_REFERENCE);
            }
            set
            {
                SetValue(WORK_ITEM_REFERENCE, value);
            }
        }

        public BaseEntity ModelItem
        {
            get
            {
                return (BaseEntity)GetValue(MODEL_ITEM_REFERENCE);
            }
            set
            {
                SetValue(MODEL_ITEM_REFERENCE, value);
            }
        }

        public BaseEntity Process
        {
            get
            {
                return (BaseEntity)GetValue(PROCESS_REFERENCE);
            }
            set
            {
                SetValue(PROCESS_REFERENCE, value);
            }
        }

        public BaseEntity SuiteRunScheduler
        {
            get
            {
                return (BaseEntity)GetValue(SUITE_RUN_SCHEDULER_REFERENCE);
            }
            set 
            {
                SetValue(SUITE_RUN_SCHEDULER_REFERENCE, value); 
            }
        }

        public BaseEntity SuiteRunSchedulerRun
        {
            get
            {
                return (BaseEntity)GetValue(SUITE_RUN_SCHEDULER_RUN_REFERENCE);
            }
            set 
            { 
                SetValue(SUITE_RUN_SCHEDULER_RUN_REFERENCE, value); 
            }
        }

        public BaseEntity Test
        {
            get
            {
                return (BaseEntity)GetValue(TEST_REFERENCE);
            }
            set
            {
                SetValue(TEST_REFERENCE, value);
            }
        }

        public BaseEntity Run
        {
            get
            {
                return (BaseEntity)GetValue(RUN_REFERENCE);
            }
            set
            {
                SetValue(RUN_REFERENCE, value);
            }
        }

        public BaseEntity Requirement
        {
            get
            {
                return (BaseEntity)GetValue(REQUIREMENT_REFERENCE);
            }
            set
            {
                SetValue(REQUIREMENT_REFERENCE, value);
            }
        }

        public BaseEntity Task
        {
            get
            {
                return (BaseEntity)GetValue(TASK_REFERENCE);
            }
            set
            {
                SetValue(TASK_REFERENCE, value);
            }
        }
    }
}
