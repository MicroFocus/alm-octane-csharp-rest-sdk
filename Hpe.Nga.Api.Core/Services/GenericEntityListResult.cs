using Hpe.Nga.Api.Core.Entities;
using System.Collections.Generic;

namespace Hpe.Nga.Api.Core.Services
{
    /// <summary>
    /// Allow access to any EntityListResult without knowning the entity type.
    /// </summary>
    public interface GenericEntityListResult
    {
        /// <summary>
        /// Get a generic list of entities
        /// </summary>
        IEnumerable<BaseEntity> BaseEntities { get; }
    }
}
