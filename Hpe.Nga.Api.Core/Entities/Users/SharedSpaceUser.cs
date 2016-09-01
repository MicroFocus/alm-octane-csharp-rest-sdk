using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services;
using Hpe.Nga.Api.Core.Services.Attributes;

namespace Hpe.Nga.Api.Core.Entities
{
    /// <summary>
    /// Wrapper for SharedspaceUser entity
    /// More fields might be supported by entity that still are not exposed in the class
    /// </summary>
    [CustomCollectionPathAttribute("users")]
    public class SharedspaceUser : BaseUserEntity
    {
        public static string PASSWORD_FIELD = "password";
        public static string WORKSPACE_ROLES_FIELD = "workspace_roles";

        public SharedspaceUser()
            : base()
        {
        }

        public SharedspaceUser(long id)
            : base(id)
        {
        }

        public string Password
        {
            get
            {
                return GetStringValue(PASSWORD_FIELD);
            }
            set
            {
                SetValue(PASSWORD_FIELD, value);
            }
        }

        public EntityList<BaseEntity> WorkspaceRoles
        {
            get
            {
                return (EntityList<BaseEntity>)GetValue(WORKSPACE_ROLES_FIELD);
            }
            set
            {
                SetValue(WORKSPACE_ROLES_FIELD, value);
            }
        }

    }
}
