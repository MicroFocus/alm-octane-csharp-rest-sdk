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


using MicroFocus.Adm.Octane.Api.Core.Connector.Exceptions;
using MicroFocus.Adm.Octane.Api.Core.Entities;
using MicroFocus.Adm.Octane.Api.Core.Entities.Teams;
using MicroFocus.Adm.Octane.Api.Core.Services;
using MicroFocus.Adm.Octane.Api.Core.Services.GroupBy;
using MicroFocus.Adm.Octane.Api.Core.Services.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MicroFocus.Adm.Octane.Api.Core.Tests
{
	[TestClass]
	public class TeamTests : BaseTest
	{

		[TestMethod]
		public void CreateTeamTest()
		{
			EntityListResult<WorkspaceUser> wsUsers = WorkspaceUserTests.GetAllWorkspaceUsers();

			var team = new Team
			{
				Name = "Team_" + Guid.NewGuid(),
				EstimatedVelocity = 50,
				Releases = EntityList<Release>.Create(ReleaseTests.CreateRelease()),
				TeamLead = wsUsers.data[0]

			};

			string[] fields = new string[] { Team.NUMBER_OF_MEMBERS_FIELD, Team.ESTIMATED_VELOCITY_FIELD, Team.TEAM_LEAD_FIELD, Team.TEAM_MEMBERS_FIELD, Team.NAME_FIELD, Team.RELEASES_FIELD };
			var createdTeam = entityService.Create(workspaceContext, team, fields);

			var r = createdTeam.Releases;
			var r2 = createdTeam.Releases;
			Assert.AreEqual(team.Name, createdTeam.Name);
			Assert.AreEqual(team.EstimatedVelocity, createdTeam.EstimatedVelocity);
			Assert.AreEqual(1, createdTeam.NumberOfMembers);
			Assert.AreEqual(team.Releases.data[0].Id, createdTeam.Releases.data[0].Id);
		}
	}

}
