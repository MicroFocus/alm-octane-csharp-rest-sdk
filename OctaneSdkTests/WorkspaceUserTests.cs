/*!
* (c) Copyright 2021 Micro Focus or one of its affiliates.
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


using MicroFocus.Adm.Octane.Api.Core.Entities;
using MicroFocus.Adm.Octane.Api.Core.Services;
using MicroFocus.Adm.Octane.Api.Core.Services.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MicroFocus.Adm.Octane.Api.Core.Tests
{
	[TestClass]
	public class WorkspaceUserTests : BaseTest
	{

		[TestMethod]
		public void GetAllWorkspaceUsersTest()
		{
			EntityListResult<WorkspaceUser> users = GetAllWorkspaceUsers();
			Assert.IsTrue(users.total_count >= 1);
		}


		public static EntityListResult<WorkspaceUser> GetAllWorkspaceUsers()
		{
			return entityService.Get<WorkspaceUser>(workspaceContext, null, null); 
		}
		/*[TestMethod]
		public void CreateUserTest()
		{
			CreateUser();

		}


		private static int USER_CREATOR_COUNTER = 0;
		public static WorkspaceUser CreateUser()
		{
			String first = "ws_user_" + USER_CREATOR_COUNTER++;
			String last = Guid.NewGuid().ToString();
			string name = first + "." + last + "@microfocus.com";


			var user = new WorkspaceUser
			{
				Language = "lang.en",
				Name = name,
				FirstName = first,
				LastName = last,
				Email = name,
				Password = "Welcome1",
				Roles = EntityList<WorkspaceRole>.Create(GetWorkspaceAdminRole())
			};



			string[] fieldsToFetch = new string[] { WorkspaceUser.FIRST_NAME_FIELD, WorkspaceUser.LAST_NAME_FIELD, WorkspaceUser.ROLES_FIELD, WorkspaceUser.NAME_FIELD };
			WorkspaceUser createdUser = entityService.Create<WorkspaceUser>(workspaceContext, user, fieldsToFetch);
			Assert.AreEqual<String>(first, createdUser.FirstName);
			return createdUser;

			//{"data":[{"language":"lang.en","name":"a1","first_name":"a1","last_name":"a1","email":"a1@a1","password":"Welcome1","roles":{"data":[{"type":"user_role","id":"1005"}]}}]}
		}*/

		

	}
}
