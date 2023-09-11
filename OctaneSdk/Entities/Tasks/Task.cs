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

		public WorkItem Story
		{
			get
			{
				return (WorkItem)GetValue(STORY_FIELD);
			}
			set

			{
				SetValue(STORY_FIELD, value);
			}
		}

		public int? EstimatedHours
		{
			get
			{
				return GetIntValue(ESTIMATED_HOURS_FIELD);
			}
			set

			{
				SetIntValue(ESTIMATED_HOURS_FIELD, value.Value);
			}
		}

		public int? RemainingHours
		{
			get
			{
				return GetIntValue(REMAINING_HOURS_FIELD);
			}
			set

			{
				SetIntValue(REMAINING_HOURS_FIELD, value.Value);
			}
		}

		public int? InvestedHours
		{
			get
			{
				return GetIntValue(INVESTED_HOURS_FIELD);
			}
			set

			{
				SetIntValue(INVESTED_HOURS_FIELD, value.Value);
			}
		}
	}
}
