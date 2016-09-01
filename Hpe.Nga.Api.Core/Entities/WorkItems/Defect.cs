using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services;

namespace Hpe.Nga.Api.Core.Entities
{
    /// <summary>
    /// Wrapper for defect entity. Acutally defect is subtype of work_item.
    /// </summary>
    public class Defect : WorkItem
    {

        public Defect()
            : base()
        {
        }

        public Defect(long id)
            : base(id)
        {
        }

    }
}
