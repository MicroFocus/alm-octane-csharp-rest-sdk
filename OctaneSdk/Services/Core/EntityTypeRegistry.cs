/*
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
using MicroFocus.Adm.Octane.Api.Core.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MicroFocus.Adm.Octane.Api.Core.Services.Core
{
    /// <summary>
    /// Registration of entities and their collection names (used by REST communication)
    /// </summary>
    public class EntityTypeRegistry
    {
        private Dictionary<Type, string> type2collectionNameMap = new Dictionary<Type, string>();
        private Dictionary<string, Type> entityTypeName2Type = new Dictionary<string, Type>();

        #region Singelton

        private static EntityTypeRegistry instance = new EntityTypeRegistry();

        private EntityTypeRegistry()
        {
            var baseEntityType = typeof(BaseEntity);
            IEnumerable<Type> types = Assembly.GetAssembly(baseEntityType).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(baseEntityType));
            foreach (Type type in types)
            {
                RegisterType(type);
            }
        }

        public void RegisterType(Type type)
        {
            var entityPathAttribute = (CustomEntityPathAttribute)Attribute.GetCustomAttribute(type, typeof(CustomEntityPathAttribute));

            string entityTypeName = entityPathAttribute != null ? entityPathAttribute.Path : ExtractEntityTypeName(type);
            entityTypeName2Type[entityTypeName] = type;

            var collectionPathAttribute = (CustomCollectionPathAttribute)Attribute.GetCustomAttribute(type, typeof(CustomCollectionPathAttribute));

            string collectionName;
            if (collectionPathAttribute != null)
            {
                collectionName = collectionPathAttribute.Path;
            }
            else
            {
                collectionName = entityTypeName.EndsWith("y") ? entityTypeName.Substring(0, entityTypeName.Length - 1) + "ies" : entityTypeName + "s";
            }
            type2collectionNameMap[type] = collectionName;
        }


        private string ExtractEntityTypeName(Type type)
        {
            string className = type.Name;
            StringBuilder sb = new StringBuilder();
            sb.Append(className[0]);
            for (int i = 1; i < className.Length; i++)
            {
                if (char.IsUpper(className[i]))
                {
                    sb.Append("_");
                }
                sb.Append(className[i]);
            }

            string result = sb.ToString().ToLower();
            return result;

        }

        public static EntityTypeRegistry GetInstance()
        {
            return instance;
        }

        #endregion

        public List<Type> GetRegisteredTypes()
        {
            return type2collectionNameMap.Keys.ToList<Type>();
        }

        public string GetCollectionName(Type type)
        {
            return type2collectionNameMap[type];
        }

        public Type GetTypeByEntityTypeName(string entityTypeName)
        {
            if (entityTypeName2Type.ContainsKey(entityTypeName))
            {
                return entityTypeName2Type[entityTypeName];
            }
            return null;
        }
    }
}
