using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hpe.Nga.Api.Core.Services.RequestContext
{
    /// <summary>
    /// Define URL for specific workspace context
    /// </summary>
    public class WorkspaceContext : SharedSpaceContext
    {
        public long WorkspaceId { get; set; }

        public WorkspaceContext(long sharedSpaceId, long workspaceId)
            : base(sharedSpaceId)
        {
            WorkspaceId = workspaceId;
        }

        public override string GetPath()
        {
            return String.Format("/api/shared_spaces/{0}/workspaces/{1}", SharedSpaceId, WorkspaceId);
        }

        public override string ToString()
        {
            return String.Format("Shared Space Id = {0}; Workspace Id : {1}", SharedSpaceId, WorkspaceId);
        }
    }
}
