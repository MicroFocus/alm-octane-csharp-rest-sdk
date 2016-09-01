using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Hpe.Nga.Api.Core.Entities;

namespace Hpe.Nga.Api.Core.Services.Core
{
    /// <summary>
    /// Registration of entities and their collection names (used by REST communication)
    /// </summary>
    public class EntityTypeRegistry
    {
        private Dictionary<Type, String> type2collectionNameMap = new Dictionary<Type, String>();
        private Dictionary<String, Type> entityTypeName2Type = new Dictionary<String, Type>();

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
            string entityTypeName = ExtractEntityTypeName(type);
            entityTypeName2Type[entityTypeName] = type;


            String collectionName = entityTypeName.EndsWith("y") ? entityTypeName.Substring(0, entityTypeName.Length - 1) + "ies" : entityTypeName + "s";
            type2collectionNameMap[type] = collectionName;
        }


        private string ExtractEntityTypeName(Type type)
        {
            String className = type.Name;
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

            String result = sb.ToString().ToLower();
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

        public String GetCollectionName(Type type)
        {
            return type2collectionNameMap[type];
        }

        public Type GetTypeByEntityTypeName(String entityTypeName)
        {
            if (entityTypeName2Type.ContainsKey(entityTypeName))
            {
                return entityTypeName2Type[entityTypeName];
            }
            return null;
        }
    }
}
