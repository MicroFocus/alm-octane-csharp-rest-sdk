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
	/// Wrapper for WorkspaceRole entity
	/// More fields might be supported by entity that still are not exposed in the class
	/// </summary>
	public class WorkspaceRole : BaseEntity
    {
        public static string WORKSPACE_FIELD = "workspace";
        public static string ROLE_FIELD = "role";

		
		public static string SHARED_SPACE_ADMIN_ROLE_LOGICAL_NAME = "role.shared.space.admin";

		//WORKSPACE
		public static string WORKSPACE_ADMIN_ROLE_LOGICAL_NAME = "role.workspace.admin";
		public static string WORKSPACE_LEADER_ROLE_LOGICAL_NAME = "role.workspace.leader";
		public static string WORKSPACE_TEAM_MEMBER_ROLE_LOGICAL_NAME = "role.workspace.team.member";
		public static string WORKSPACE_TESTER_ROLE_LOGICAL_NAME = "role.workspace.tester";

		public Workspace Workspace
        {
            get
            {
                return (Workspace)GetValue(WORKSPACE_FIELD);
            }
            set
            {
                SetValue(WORKSPACE_FIELD, value);
            }
        }

        public Role Role
        {
            get
            {
                return (Role)GetValue(ROLE_FIELD);
            }
            set
            {
                SetValue(ROLE_FIELD, value);
            }
        }
    }
}
