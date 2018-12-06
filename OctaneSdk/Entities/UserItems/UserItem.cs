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
        public static string TASK_REFERENCE = "my_follow_items_task";
        public static string TEST_REFERENCE = "my_follow_items_test";
        public static string RUN_REFERENCE = "my_follow_items_run";
        public static string REQUIREMENT_REFERENCE = "my_follow_items_requirement";

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
