using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services;

namespace Hpe.Nga.Api.Core.Entities
{
    /// <summary>
    /// Wrapper for Workspace entity
    /// More fields might be supported by entity that still are not exposed in the class
    /// </summary>
    public class Workspace : BaseEntity
    {
        public static string USERS_FIELD = "users";
    }
}
