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


using MicroFocus.Adm.Octane.Api.Core.Services;
using MicroFocus.Adm.Octane.Api.Core.Services.Attributes;

namespace MicroFocus.Adm.Octane.Api.Core.Entities
{
	/// <summary>
	/// Wrapper for SharedspaceUser entity
	/// More fields might be supported by entity that still are not exposed in the class
	/// </summary>
	[CustomCollectionPathAttribute("users")]
    public class SharedspaceUser : BaseUserEntity
    {
        public static string WORKSPACE_ROLES_FIELD = "workspace_roles";

        public SharedspaceUser()
            : base()
        {
        }

        public SharedspaceUser(EntityId id)
            : base(id)
        {
        }

        public EntityList<WorkspaceRole> WorkspaceRoles
        {
            get
            {
                return GetEntityList<WorkspaceRole>(WORKSPACE_ROLES_FIELD);
            }
            set
            {
                SetValue(WORKSPACE_ROLES_FIELD, value);
            }
        }

    }
}
