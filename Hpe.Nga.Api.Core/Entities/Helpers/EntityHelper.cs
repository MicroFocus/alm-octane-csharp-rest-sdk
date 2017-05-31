using Hpe.Nga.Api.Core.Connector;
using Hpe.Nga.Api.Core.Services.RequestContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hpe.Nga.Api.Core.Entities.Helpers
{
    public class EntityHelper
    {
        public static string GetEntityLink(RestConnector connector, WorkspaceContext context, BaseEntity entity)
        {
            string entityType = entity.AggregateType != null ? entity.AggregateType : entity.TypeName;
            return string.Format("{0}/ui/entity-navigation?p={1}/{2}&entityType={3}&id={4}",
                connector.Host, context.SharedSpaceId, context.WorkspaceId, entityType, entity.Id.ToString()
                );
        }
    }
}
