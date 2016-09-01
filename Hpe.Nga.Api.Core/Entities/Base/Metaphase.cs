using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services;

namespace Hpe.Nga.Api.Core.Entities
{
    /// <summary>
    /// Wrapper for Metaphase entity. Metaphase contains one or more phases. 
    /// For example , metaphase "Done" contains of "Closed", "Duplicated", "Rejected" phases.
    /// More fields might be supported by entity that still are not exposed in the class
    /// </summary>
    public class Metaphase : BaseEntity
    {



    }
}
