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


using System;
using MicroFocus.Adm.Octane.Api.Core.Services;

namespace MicroFocus.Adm.Octane.Api.Core.Entities.Teams
{
	/// <summary>
	/// Wrapper for Team entity
	/// More fields might be supported by entity that still are not exposed in the class
	/// </summary>
	public class Team : BaseEntity
	{
		public static string ESTIMATED_VELOCITY_FIELD = "estimated_velocity";
		public static string NUMBER_OF_MEMBERS_FIELD = "number_of_members";
		public static string TEAM_LEAD_FIELD = "team_lead";
		public static string TEAM_MEMBERS_FIELD = "team_members";
		public static string RELEASES_FIELD = "releases";
		

		public int? EstimatedVelocity
		{
			get
			{
				return GetIntValue(ESTIMATED_VELOCITY_FIELD);
			}
			set
			{
				SetIntValue(ESTIMATED_VELOCITY_FIELD, value.Value);
			}
		}

		public WorkspaceUser TeamLead
		{
			get
			{
				return (WorkspaceUser)GetValue(TEAM_LEAD_FIELD);
			}
			set
			{
				SetValue(TEAM_LEAD_FIELD, value);
			}
		}

		public int? NumberOfMembers
		{
			get
			{
				return GetIntValue(NUMBER_OF_MEMBERS_FIELD);
			}

		}

		public EntityList<Release> Releases
		{
			get
			{
				return GetEntityList<Release>(RELEASES_FIELD);
				
			}
			set
			{
				SetValue(RELEASES_FIELD, value);
			}
		}



		public EntityList<TeamMember> TeamMembers
		{
			get
			{
				return GetEntityList<TeamMember>(TEAM_MEMBERS_FIELD);
			}
			set
			{
				SetValue(TEAM_MEMBERS_FIELD, value);
			}
		}
	}
}
