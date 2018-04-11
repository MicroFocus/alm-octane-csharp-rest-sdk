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


using System;
using System.Collections.Generic;

namespace MicroFocus.Adm.Octane.Api.Core.Services.Core
{
    /// <summary>
    /// Base class for all objects that contains dynamic fields, for example <see cref="BaseEntity"/>
    /// </summary>
    public class DictionaryBasedEntity
    {
        protected IDictionary<string, object> m_properties;

        #region Ctors

        public DictionaryBasedEntity()
        {
            this.m_properties = new Dictionary<string, object>();
        }

        public DictionaryBasedEntity(IDictionary<string, object> properties)
        {
            this.m_properties = new Dictionary<string, object>(properties);
        }

        #endregion

        public void SetValue(String propertyName, Object value)
        {
            m_properties[propertyName] = value; ;
        }

        public Object GetValue(String propertyName)
        {
            if (Contains(propertyName))
            {
                return m_properties[propertyName];
            }
            return null;
        }

        public IDictionary<string, object> GetProperties()
        {
            return m_properties;
        }

        public void SetProperties(IDictionary<string, object> properties)
        {
            this.m_properties = new Dictionary<string, object>(properties);
        }

        public string GetStringValue(String propertyName)
        {
            object value = GetValue(propertyName);
            if (value == null)
            {
                return null;
            }
            else if (value is string)
            {
                return (string)value;
            }
            else
            {
                return value.ToString();
            }
        }

        public int? GetIntValue(String propertyName)
        {
            Object obj = GetValue(propertyName);
            if (obj == null)
            {
                return null;
            }
            else if (obj is int)
            {
                return (int)obj;
            }
            else
            {
                return int.Parse((String)obj);
            }
        }

        #region Types handling Properties

        public long? GetLongValue(String propertyName)
        {
            Object obj = GetValue(propertyName);
            if (obj == null)
            {
                return null;
            }
            else if (obj is long)
            {
                return (long)obj;
            }
            else if (obj is int)
            {
                return (int)obj;
            }
            else
            {
                return long.Parse((String)obj);
            }
        }

        public long GetLongValue(String propertyName, long defaultValue)
        {
            long? value = GetLongValue(propertyName);
            return (value.HasValue) ? value.Value : defaultValue;
        }

        public void SetLongValue(String propertyName, long value)
        {
            SetValue(propertyName, value);
        }

        public int GetIntValue(String propertyName, int defaultValue)
        {
            int? value = GetIntValue(propertyName);
            return (value.HasValue) ? value.Value : defaultValue;
        }

        public void SetIntValue(String propertyName, int value)
        {
            SetValue(propertyName, value);
        }

        #endregion

        public bool Contains(string property)
        {
            return m_properties.ContainsKey(property);
        }

        public override string ToString()
        {
            return m_properties == null ? "No properties" : String.Format("{0} properties", m_properties.Count);
        }


    }
}
