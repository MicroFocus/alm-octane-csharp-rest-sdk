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


namespace MicroFocus.Adm.Octane.Api.Core.Entities.Base
{
    public class Attachment : BaseEntity
    {

        public static string OWNER_WORK_ITEM_FIELD = "owner_work_item";
        public static string OWNER_MILESTONE_FIELD = "owner_milestone";
        public static string OWNER_RELEASE_FIELD = "owner_release";

        #region ctors

        public Attachment()
        {

        }

        public Attachment(EntityId id)
        {
            this.Id = id;
        }
        #endregion

        #region members
        public string Description { get; set; }

        public long size { get; set; }

        public BaseEntity owner_milestone {
            get
            {
                return (BaseEntity)GetValue(OWNER_MILESTONE_FIELD);
            }
            set
            {
                SetValue(OWNER_MILESTONE_FIELD, value);
            }
        }

        public BaseEntity owner_release {
            get
            {
                return (BaseEntity)GetValue(OWNER_RELEASE_FIELD);
            }
            set
            {
                SetValue(OWNER_RELEASE_FIELD, value);
            }
        }

        public BaseEntity owner_requirement {
            get
            {
                return (BaseEntity)GetValue(OWNER_WORK_ITEM_FIELD);
            }
            set
            {
                SetValue(OWNER_WORK_ITEM_FIELD, value);
            }
        }

        public BaseEntity owner_run {
            get
            {
                return (BaseEntity)GetValue(OWNER_WORK_ITEM_FIELD);
            }
            set
            {
                SetValue(OWNER_WORK_ITEM_FIELD, value);
            }
        }

        public BaseEntity owner_run_set
        {
            get
            {
                return (BaseEntity)GetValue(OWNER_WORK_ITEM_FIELD);
            }
            set
            {
                SetValue(OWNER_WORK_ITEM_FIELD, value);
            }
        }

        public BaseEntity owner_task {
            get
            {
                return (BaseEntity)GetValue(OWNER_WORK_ITEM_FIELD);
            }
            set
            {
                SetValue(OWNER_WORK_ITEM_FIELD, value);
            }
        }

        public BaseEntity owner_test {
            get
            {
                return (BaseEntity)GetValue(OWNER_WORK_ITEM_FIELD);
            }
            set
            {
                SetValue(OWNER_WORK_ITEM_FIELD, value);
            }
        }

        public BaseEntity owner_work_item {
            get
            {
                return (BaseEntity)GetValue(OWNER_WORK_ITEM_FIELD);
            }
            set
            {
                SetValue(OWNER_WORK_ITEM_FIELD, value);
            }
        }

    }
}

#endregion
