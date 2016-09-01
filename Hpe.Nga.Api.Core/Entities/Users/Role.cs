using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services;

namespace Hpe.Nga.Api.Core.Entities
{
    /// <summary>
    /// Wrapper for Role entity
    /// More fields might be supported by entity that still are not exposed in the class
    /// </summary>
    public class Role : BaseEntity
    {
        // public static string LOGICAL_NAME_FIELD = "logical_name";

        public string LogicalName
        {
            get
            {
                return GetStringValue(LOGICAL_NAME_FIELD);
            }
            set
            {
                SetValue(LOGICAL_NAME_FIELD, value);
            }
        }
    }
}
