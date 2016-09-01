using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Hpe.Nga.Api.Core.Connector;
using Hpe.Nga.Api.Core.Entities;
using Hpe.Nga.Api.Core.Services.Attributes;
using Hpe.Nga.Api.Core.Services.Core;
using Hpe.Nga.Api.Core.Services.GroupBy;
using Hpe.Nga.Api.Core.Services.Query;
using Hpe.Nga.Api.Core.Services.RequestContext;

namespace Hpe.Nga.Api.Core.Services
{
    /// <summary>
    /// Apllication layer class.
    /// Wraps low-level <see cref="RestConnector"/> and exposes method for CRUD manipulating entities.
    /// </summary>
    public class EntityService
    {
        private RestConnector rc = RestConnector.GetInstance();
        private JavaScriptSerializer jsonSerializer;


        #region Singelton

        private static EntityService instance = new EntityService();

        private EntityService()
        {
            jsonSerializer = new JavaScriptSerializer();
            jsonSerializer.RegisterConverters(new JavaScriptConverter[] { new BaseEntityJsonConverter() });
        }

        public static EntityService GetInstance()
        {
            return instance;
        }

        #endregion

        public EntityListResult<T> Get<T>(IRequestContext context)
            where T : BaseEntity
        {
            return Get<T>(context, null, null);
        }

        public EntityListResult<T> Get<T>(IRequestContext context, IList<QueryPhrase> queryPhrases, List<String> fields)
        where T : BaseEntity
        {
            return Get<T>(context, queryPhrases, fields, null);
        }

        public EntityListResult<T> Get<T>(IRequestContext context, IList<QueryPhrase> queryPhrases, List<String> fields, int? limit)
            where T : BaseEntity
        {
            String collectionName = GetCollectionName<T>();
            string url = context.GetPath() + "/" + collectionName;

            String queryString = QueryStringBuilder.BuildQueryString(queryPhrases, fields, null, null, limit, null);
            if (!String.IsNullOrEmpty(queryString))
            {
                url = url + "?" + queryString;
            }

            ResponseWrapper response = rc.ExecuteGet(url);
            if (response.Data != null)
            {
                EntityListResult<T> result = jsonSerializer.Deserialize<EntityListResult<T>>(response.Data);
                return result;
            }
            return null;
        }

        private string GetCollectionName<T>() where T : BaseEntity
        {
            CustomCollectionPathAttribute customCollectionPathAttribute =
            (CustomCollectionPathAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(CustomCollectionPathAttribute));

            String collectionName = null;
            if (customCollectionPathAttribute != null)
            {
                collectionName = customCollectionPathAttribute.Path;
            }
            else
            {
                collectionName = EntityTypeRegistry.GetInstance().GetCollectionName(typeof(T));
            }

            return collectionName;
        }

        public GroupResult GetWithGroupBy<T>(IRequestContext context, IList<QueryPhrase> queryPhrases, String groupBy)
            where T : BaseEntity
        {

            String collectionName = GetCollectionName<T>();
            string url = context.GetPath() + "/" + collectionName + "/groups";


            String queryString = QueryStringBuilder.BuildQueryString(queryPhrases, null, null, null, null, groupBy);
            if (!String.IsNullOrEmpty(queryString))
            {
                url = url + "?" + queryString;
            }

            ResponseWrapper response = rc.ExecuteGet(url);
            if (response.Data != null)
            {
                GroupResult result = jsonSerializer.Deserialize<GroupResult>(response.Data);
                return result;
            }
            return null;
        }


        public T GetById<T>(IRequestContext context, long id, IList<String> fields)
           where T : BaseEntity
        {
            String collectionName = GetCollectionName<T>();
            string url = context.GetPath() + "/" + collectionName + "/" + id;
            String queryString = QueryStringBuilder.BuildQueryString(null, fields, null, null, null, null);
            if (!String.IsNullOrEmpty(queryString))
            {
                url = url + "?" + queryString;
            }

            ResponseWrapper response = rc.ExecuteGet(url);
            T result = jsonSerializer.Deserialize<T>(response.Data);
            return result;
        }

        public EntityListResult<T> Create<T>(IRequestContext context, EntityList<T> entityList)
             where T : BaseEntity
        {
            String collectionName = GetCollectionName<T>();
            string url = context.GetPath() + "/" + collectionName;
            String data = jsonSerializer.Serialize(entityList);
            ResponseWrapper response = rc.ExecutePost(url, data);
            EntityListResult<T> result = jsonSerializer.Deserialize<EntityListResult<T>>(response.Data);
            return result;
        }

        public T Create<T>(IRequestContext context, T entity)
            where T : BaseEntity
        {

            EntityListResult<T> result = Create<T>(context, EntityList<T>.Create(entity));
            return result.data[0];
        }

        public T Update<T>(IRequestContext context, T entity)
             where T : BaseEntity
        {
            String collectionName = GetCollectionName<T>();
            string url = context.GetPath() + "/" + collectionName + "/" + entity.Id;
            String data = jsonSerializer.Serialize(entity);
            ResponseWrapper response = rc.ExecutePut(url, data);
            T result = jsonSerializer.Deserialize<T>(response.Data);
            return result;
        }


        public void DeleteById<T>(IRequestContext context, long entityId)
             where T : BaseEntity
        {
            String collectionName = GetCollectionName<T>();
            string url = context.GetPath() + "/" + collectionName + "/" + entityId;
            ResponseWrapper response = rc.ExecuteDelete(url);
        }

        public void DeleteByFilter<T>(IRequestContext context, IList<QueryPhrase> queryPhrases)
            where T : BaseEntity
        {
            String collectionName = GetCollectionName<T>();
            String queryString = QueryStringBuilder.BuildQueryString(queryPhrases, null, null, null, null, null);
            string url = context.GetPath() + "/" + collectionName + "?" + queryString;
            ResponseWrapper response = rc.ExecuteDelete(url);
        }


    }
}
