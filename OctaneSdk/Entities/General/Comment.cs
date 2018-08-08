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
    /// Wrapper for Comment entity.
    /// </summary>
    public class Comment : BaseEntity
    {
        public const string AUTHOR_FIELD = "author";
        public const string OWNER_WORK_FIELD = "owner_work_item";
        public const string OWNER_TEST_FIELD = "owner_test";
        public const string OWNER_RUN_FIELD = "owner_run";
        public const string OWNER_REQUIREMENT_FIELD = "owner_requirement";
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
