﻿/*
 * Copyright 2016-2024 Open Text.
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
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace MicroFocus.Adm.Octane.Api.Core.Services.Core
{
    /// <summary>
    /// Serialization and deserialization to JSON of classes that inherit <see cref="DictionaryBasedEntity"/>
    /// </summary>
    public class EntityJsonConverter : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                List<Type> types = new List<Type>(EntityTypeRegistry.GetInstance().GetRegisteredTypes());
                types.Add(typeof(BaseEntity));

                types.Add(typeof(DictionaryBasedEntity));

                return types;
            }
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (obj == null) return result;
            DictionaryBasedEntity entity = ((DictionaryBasedEntity)obj);
            return entity.GetProperties();
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            DictionaryBasedEntity entity = (DictionaryBasedEntity)Activator.CreateInstance(type);
            entity.SetProperties(dictionary);
            OverrideReferenceFields(entity);

            return entity;
        }

        private static void OverrideReferenceFields(DictionaryBasedEntity entity)
        {
            ICollection<string> keys = new List<string>(entity.GetProperties().Keys);
            foreach (string key in keys)
            {
                object value = entity.GetValue(key);
                if (value is Dictionary<string, object>)
                {
                    Dictionary<string, object> pairValue = (Dictionary<string, object>)value;
                    if (pairValue.ContainsKey("total_count"))//list of entities
                    {
                        EntityList<BaseEntity> entityList = new EntityList<BaseEntity>();
                        IList data = (IList)((Dictionary<string, object>)value)["data"];
                        for (int i = 0; i < data.Count; i++)
                        {
                            Dictionary<string, object> rawEntity = (Dictionary<string, object>)data[i];
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

        private static BaseEntity ConvertToBaseEntity(Dictionary<string, object> rawEntity)
        {
            BaseEntity baseEntity = null;
            if (rawEntity.ContainsKey("type"))
            {
                string entityTypeName = (string)rawEntity["type"];
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
