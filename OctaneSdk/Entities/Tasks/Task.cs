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
    /// Wrapper for Task entity.
    /// </summary>
    public class Task : BaseEntity
    {
        public const string TYPE_TASK = "task";

        public const string OWNER_FIELD = "owner";
        public const string AUTHOR_FIELD = "author";
        public const string PHASE_FIELD = "phase";
        public const string INVESTED_HOURS_FIELD = "invested_hours";
        public const string REMAINING_HOURS_FIELD = "remaining_hours";
        public const string ESTIMATED_HOURS_FIELD = "estimated_hours";
        public const string STORY_FIELD = "story";

        public Task()
        {
        }

        public Task(EntityId id)
            : base(id)
        {
        }

		public void SetStory(WorkItem story)
		{
			SetValue(STORY_FIELD, story);
		}

		public WorkItem GetStory()
		{
			return (WorkItem)GetValue(STORY_FIELD);
		}

		public void SetEstimatedHours(int hours)
		{
			SetIntValue(ESTIMATED_HOURS_FIELD, hours);
		}

		public int? GetEstimatedHours()
		{
			return GetIntValue(ESTIMATED_HOURS_FIELD);
		}

		public void SetRemainingHours(int hours)
		{
			SetIntValue(REMAINING_HOURS_FIELD, hours);
		}

		public int? GetRemainingHours()
		{
			return GetIntValue(REMAINING_HOURS_FIELD);
		}

		public void SetInvestedHours(int hours)
		{
			SetIntValue(INVESTED_HOURS_FIELD, hours);
		}

		public int? GetInvestedHours()
		{
			return GetIntValue(INVESTED_HOURS_FIELD);
		}
	}
}
