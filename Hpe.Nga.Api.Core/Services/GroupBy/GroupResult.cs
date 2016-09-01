using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services.Core;

namespace Hpe.Nga.Api.Core.Services.GroupBy
{
    /// <summary>
    /// Object that returns on GroupBy request
    /// </summary>
    public class GroupResult 
    {
        public int groupsTotalCount { get; set; }

        public List<Group> groups { get; set; }
    }
}
