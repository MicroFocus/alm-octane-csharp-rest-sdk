using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services;

namespace Hpe.Nga.Api.Core.Entities
{
    /// <summary>
    /// Wrapper for feature entity. Acutally Feature is subtype of work_item.
    /// </summary>
    public class Feature : WorkItem
    {

        public Feature()
            : base()
        {
        }

        public Feature(long id)
            : base(id)
        {
        }

    }
}
