﻿/*!
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
	/// Serailization and deserialization to JSON of classes that inherit <see cref="BaseEntity"/>
	/// </summary>
	public class BaseEntityJsonConverter : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                List<Type> types = new List<Type>(EntityTypeRegistry.GetInstance().GetRegisteredTypes());
                types.Add(typeof(BaseEntity));


                return types;
            }
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (obj == null) return result;
            BaseEntity entity = ((BaseEntity)obj);
            return entity.GetProperties();
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            BaseEntity entity = (BaseEntity)Activator.CreateInstance(type);
            entity.SetProperties(dictionary);
            OverrideReferenceFields(entity);

            return entity;
        }

        private static void OverrideReferenceFields(BaseEntity entity)
        {
            ICollection<String> keys = new List<String>(entity.GetProperties().Keys);
            foreach (String key in keys)
            {
                object value = entity.GetValue(key);
                if (value is Dictionary<String, Object>)
                {
                    Dictionary<String, Object> pairValue = (Dictionary<String, Object>)value;
                    if (pairValue.ContainsKey("total_count"))//list of entities
                    {
                        EntityList<BaseEntity> entityList = new EntityList<BaseEntity>();
                        IList data = (IList)((Dictionary<String, Object>)value)["data"];
                        for (int i = 0; i < data.Count; i++)
                        {
                            Dictionary<String, Object> rawEntity = (Dictionary<String, Object>)data[i];
                            BaseEntity baseEntity = ConvertToBaseEntity(rawEntity);
                            OverrideReferenceFields(baseEntity);
                            entityList.data.Add(baseEntity);
                        }
                        entity.SetValue(key, entityList);
                    }
                    else //single entity
                    {
                        BaseEntity baseEntity = ConvertToBaseEntity(pairValue);
                        OverrideReferenceFields(baseEntity);
                        entity.SetValue(key, baseEntity);
                    }
                }
            }
        }

        private static BaseEntity ConvertToBaseEntity(Dictionary<String, Object> rawEntity)
        {
            BaseEntity baseEntity = null;
            if (rawEntity.ContainsKey("type"))
            {
                String entityTypeName = (String)rawEntity["type"];
                if (entityTypeName != null)
                {
                    Type entityType = EntityTypeRegistry.GetInstance().GetTypeByEntityTypeName(entityTypeName);
                    if (entityType != null)
                    {
                        baseEntity = (BaseEntity)Activator.CreateInstance(entityType);
                    }
                }
            }

            if (baseEntity == null)
            {
                baseEntity = new BaseEntity();
            }

            baseEntity.SetProperties(rawEntity);
            return baseEntity;
        }

    }
}
