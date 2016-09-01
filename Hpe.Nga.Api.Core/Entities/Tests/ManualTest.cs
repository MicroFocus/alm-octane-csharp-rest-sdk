using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services;

namespace Hpe.Nga.Api.Core.Entities
{
    /// <summary>
    /// Wrapper for ManualTest entity. Acutally Feature is subtype of work_item.
    /// </summary>
    public class ManualTest : Test
    {

        public ManualTest()
            : base()
        {
        }

        public ManualTest(long id)
            : base(id)
        {
        }

    }
}
