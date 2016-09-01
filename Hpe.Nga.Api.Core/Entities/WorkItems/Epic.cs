using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services;

namespace Hpe.Nga.Api.Core.Entities
{
    /// <summary>
    /// Wrapper for Epic entity
    /// More fields might be supported by entity that still are not exposed in the class
    /// </summary>
    public class Epic : BaseEntity
    {

        public Epic()
        {

        }

        public Epic(long id)
            : base(id)
        {
        }



    }
}
