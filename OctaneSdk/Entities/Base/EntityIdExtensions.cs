/*!
* (c) 2016-2018 EntIT Software LLC, a Micro Focus company
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/


using System.Linq;
using System.Collections.Generic;

namespace MicroFocus.Adm.Octane.Api.Core.Entities
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
