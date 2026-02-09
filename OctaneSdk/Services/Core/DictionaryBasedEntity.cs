/*
 * Copyright 2016-2026 Open Text.
 *
 * The only warranties for products and services of Open Text and
 * its affiliates and licensors (“Open Text”) are as may be set forth
 * in the express warranty statements accompanying such products and services.
 * Nothing herein should be construed as constituting an additional warranty.
 * Open Text shall not be liable for technical or editorial errors or
 * omissions contained herein. The information contained herein is subject
 * to change without notice.
 *
 * Except as specifically indicated otherwise, this document contains
 * confidential information and a valid license is required for possession,
 * use or copying. If this work is provided to the U.S. Government,
 * consistent with FAR 12.211 and 12.212, Commercial Computer Software,
 * Computer Software Documentation, and Technical Data for Commercial Items are
 * licensed to the U.S. Government under vendor's standard commercial license.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *   http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


using MicroFocus.Adm.Octane.Api.Core.Entities;
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

        public void SetValue(string propertyName, object value)
        {
            m_properties[propertyName] = value; ;
        }

        public object GetValue(string propertyName)
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

        public int? GetIntValue(string propertyName)
        {
            object obj = GetValue(propertyName);
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
                return int.Parse((string)obj);
            }
        }

        #region Types handling Properties

        public long? GetLongValue(string propertyName)
        {
            object obj = GetValue(propertyName);
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
                return long.Parse((string)obj);
            }
        }

        public long GetLongValue(string propertyName, long defaultValue)
        {
            long? value = GetLongValue(propertyName);
            return (value.HasValue) ? value.Value : defaultValue;
        }

        public bool? GetBooleanValue(string propertyName)
        {
            object obj = GetValue(propertyName);
            if (obj == null)
            {
                return null;
            }
            else if (obj is bool)
            {
                return (bool)obj;
            }
            else
            {
                return bool.Parse((string)obj);
            }
        }

        public bool GetBooleanValue(string propertyName, bool defaultValue)
        {
            bool? value = GetBooleanValue(propertyName);
            return (value.HasValue) ? value.Value : defaultValue;
        }

        public void SetLongValue(string propertyName, long value)
        {
            SetValue(propertyName, value);
        }

        public int GetIntValue(string propertyName, int defaultValue)
        {
            int? value = GetIntValue(propertyName);
            return (value.HasValue) ? value.Value : defaultValue;
        }

        public void SetIntValue(string propertyName, int value)
        {
            SetValue(propertyName, value);
        }

        public EntityList<T> GetEntityList<T>(string propertyName) where T : BaseEntity
        {
            object value = GetValue(propertyName);
            if (value == null)
            {
                return null;
            }
            else if (value is EntityList<BaseEntity>)
            {
                EntityList<BaseEntity> entities = (EntityList<BaseEntity>)value;
                EntityList<T> myList = new EntityList<T>();
                foreach (var item in entities.data)
                {
                    myList.data.Add((T)item);
                }
                return myList;
            }
            else // (value is EntityList<T>)
            {
                return (EntityList<T>)value;
            }
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
