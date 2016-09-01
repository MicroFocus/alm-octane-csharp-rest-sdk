using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hpe.Nga.Api.Core.Services.RequestContext
{
    /// <summary>
    /// Define URL for specific sharedspace context
    /// </summary>
    public class SharedSpaceContext : IRequestContext
    {
        public long SharedSpaceId { get; set; }

        public SharedSpaceContext(long sharedSpaceId)
        {
            SharedSpaceId = sharedSpaceId;
        }

        public virtual string GetPath()
        {
            return String.Format("/api/shared_spaces/{0}", SharedSpaceId);
        }

        public override string ToString()
        {
            return "Shared Space Id = " + SharedSpaceId;
        }
    }
}
