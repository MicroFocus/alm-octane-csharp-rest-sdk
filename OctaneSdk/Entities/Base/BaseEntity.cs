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


using MicroFocus.Adm.Octane.Api.Core.Services.Core;
using System;
using System.Collections.Generic;

namespace MicroFocus.Adm.Octane.Api.Core.Entities
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    public class BaseEntity : DictionaryBasedEntity
    {
        public static string ID_FIELD = "id";
        public static string TYPE_FIELD = "type";
        public static string NAME_FIELD = "name";
        public static string LOGICAL_NAME_FIELD = "logical_name";

        public static string DATE_TIME_FORMAT = "yyyy-MM-ddTHH:mm:ssZ";

        #region Ctors

        public BaseEntity()
            : base()
        {
        }

        public BaseEntity(EntityId id)
            : base()
        {
            Id = id;
        }



        public BaseEntity(IDictionary<string, object> properties)
            : base(properties)
        {
        }

        #endregion

        #region Base Properties

        public EntityId Id
        {
            get
            {
                object id = GetValue(ID_FIELD);
                if (id is EntityId)
                {
                    return (EntityId)id;
                }
                else
                {
                    return new EntityId(id.ToString());
                }
            }
            set
            {
                SetValue(ID_FIELD, value);
            }

        }

        public String Name
        {
            get
            {
                return GetStringValue(NAME_FIELD);
            }
            set
            {
                SetValue(NAME_FIELD, value);
            }

        }

        public string TypeName
        {
            get
            {
                return GetStringValue(TYPE_FIELD);
            }
            set
            {
                SetValue(TYPE_FIELD, value);
            }

        }

        public string AggregateType
        {
            get; protected set;
        }

        #endregion

        public DateTime? GetDateTimeValue(string propertyName)
        {
            Object obj = GetValue(propertyName);
            if (obj == null)
            {
                return null;
            }
            else if (obj is DateTime)
            {
                return (DateTime)obj;
            }
            else
            {
                return DateTime.ParseExact((String)obj, DATE_TIME_FORMAT, null);
            }
        }

        public void SetDateTimeValue(string propertyName, DateTime date)
        {
            String value = date.ToString(DATE_TIME_FORMAT);
            SetValue(propertyName, value);
        }

        public DateTime? GetDateTimeUTCValue(string propertyName)
        {
            Object obj = GetValue(propertyName);
            if (obj == null)
            {
                return null;
            }
            else if (obj is DateTime)
            {
                return ((DateTime)obj).ToUniversalTime();
            }
            else
            {
                return DateTime.ParseExact((String)obj, DATE_TIME_FORMAT, null).ToUniversalTime();
            }
        }

        //2016-06-01T05:26:02Z
        //DateTime date = DateTime.Now;
        //
        //String formatterdDate = date.ToString(format);

        public override string ToString()
        {
            String name = Name == null ? "No name" : Name.Substring(0, Math.Min(20, Name.Length)); //Show upto 20 characters in name
            return m_properties == null ? "No properties" : String.Format("{0} #{1} - {2} , {3} properties", TypeName, Id, name, m_properties.Count);
        }
    }
}
