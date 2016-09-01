using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services;

namespace Hpe.Nga.Api.Core.Entities
{
    /// <summary>
    /// Wrapper for WorkspaceRole entity
    /// More fields might be supported by entity that still are not exposed in the class
    /// </summary>
    public class WorkspaceRole : BaseEntity
    {
        public static string WORKSPACE_FIELD = "workspace";
        public static string ROLE_FIELD = "role";

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
