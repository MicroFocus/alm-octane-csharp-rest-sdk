using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services;

namespace Hpe.Nga.Api.Core.Entities
{
    /// <summary>
    /// Wrapper for Phase entity. One or more phases might be grouped to on phase. 
    /// For example , metaphase "Done" contains of "Closed", "Duplicated", "Rejected" phases.
    /// Each entity type has its own phases, that means phases of defects are different from phases of story and tests.
    /// More fields might be supported by entity that still are not exposed in the class
    /// </summary>
    public class Phase : BaseEntity
    {
        public static string ENTITY_FIELD = "entity";

    }
}
