using System.Linq;
using System.Collections.Generic;

namespace Hpe.Nga.Api.Core.Entities
{
    public static class EntityIdExtensions
    {
        public static List<T> ToList<T>(this IEnumerable<EntityId> entityIds)
        {
            if (typeof(T).IsAssignableFrom(typeof(long)))
            {
#pragma warning disable CS0618 // Type or member is obsolete
                return entityIds.Select(id => (long)id).ToList() as List<T>;
#pragma warning restore CS0618 // Type or member is obsolete
            }
            else if (typeof(T).IsAssignableFrom(typeof(string)))
            {
                return entityIds.Select(id => (string)id).ToList() as List<T>;
            }
            else
            {
                throw new System.Exception("IEnumerable<EntityId> cannot be converted to List of " + typeof(T));
            }
        }
    }
}
