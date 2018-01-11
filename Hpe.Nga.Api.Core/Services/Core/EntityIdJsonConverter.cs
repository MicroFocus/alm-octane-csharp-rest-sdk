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


using MicroFocus.Adm.Octane.Api.Core.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace MicroFocus.Adm.Octane.Api.Core.Services.Core
{
	/// <summary>
	/// This class allow EntityId property to be serialized as plan string instead of complex JSON object.
	/// </summary>
	/// <remarks>
	/// Workaround to serialize <see cref="EntityId"/> as simple string instead of a dictionary of keys and values.
	/// For more details see http://blog.calyptus.eu/seb/2011/12/custom-datetime-json-serialization/.
	/// </remarks>
	internal partial class EntityIdJsonConverter : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new[] { typeof(EntityId) };
            }
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new JavaScriptSerializer().ConvertToType(dictionary, type);
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            if (!(obj is EntityId)) return null;

            var entityId = (EntityId)obj;
            return new EntityIdAsString(entityId.ToString());
        }

        private class EntityIdAsString : Uri, IDictionary<string, object>
        {
            public EntityIdAsString(string value) : base(value, UriKind.Relative)
            {

            }

            public object this[string key] { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

            public ICollection<string> Keys
            {
                get { throw new NotImplementedException(); }
            }

            public ICollection<object> Values
            {
                get { throw new NotImplementedException(); }
            }

            public int Count
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsReadOnly
            {
                get { throw new NotImplementedException(); }
            }

            public void Add(string key, object value)
            {
                throw new NotImplementedException();
            }

            public void Add(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException();
            }

            public bool ContainsKey(string key)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public bool Remove(string key)
            {
                throw new NotImplementedException();
            }

            public bool Remove(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException();
            }

            public bool TryGetValue(string key, out object value)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}
