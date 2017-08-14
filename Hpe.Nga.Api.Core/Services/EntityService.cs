// (c) Copyright 2016 Hewlett Packard Enterprise Development LP

// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.

// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,

// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

// See the License for the specific language governing permissions and limitations under the License.

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
using Hpe.Nga.Api.Core.Entities.Base;
using System.Threading.Tasks;

using Task = System.Threading.Tasks.Task;

namespace Hpe.Nga.Api.Core.Services
{
    /// <summary>
    /// Apllication layer class.
    /// Wraps low-level <see cref="RestConnector"/> and exposes method for CRUD manipulating entities.
    /// </summary>
    public class EntityService
    {
        private RestConnector rc;
        private JavaScriptSerializer jsonSerializer;




        public EntityService(RestConnector rc)
        {
            this.rc = rc;
            this.jsonSerializer = new JavaScriptSerializer();
            jsonSerializer.RegisterConverters(new JavaScriptConverter[] { new BaseEntityJsonConverter() });
        }

        public EntityListResult<T> Get<T>(IRequestContext context) where T : BaseEntity
        {
            return GetAsync<T>(context).Result;
        }

        public Task<EntityListResult<T>> GetAsync<T>(IRequestContext context) where T : BaseEntity
        {
            return GetAsync<T>(context, null, null);
        }

        public EntityListResult<T> Get<T>(IRequestContext context, IList<QueryPhrase> queryPhrases, List<String> fields) where T : BaseEntity
        {
            return GetAsync<T>(context, queryPhrases, fields).Result;
        }

        public Task<EntityListResult<T>> GetAsync<T>(IRequestContext context, IList<QueryPhrase> queryPhrases, List<String> fields) where T : BaseEntity
        {
            return GetAsync<T>(context, queryPhrases, fields, null);
        }

        public EntityListResult<T> Get<T>(IRequestContext context, IList<QueryPhrase> queryPhrases, List<String> fields, int? limit) where T : BaseEntity
        {
            return GetAsync<T>(context, queryPhrases, fields, limit).Result;
        }

        public async Task<EntityListResult<T>> GetAsync<T>(IRequestContext context, IList<QueryPhrase> queryPhrases, List<String> fields, int? limit) where T : BaseEntity
        {
            String collectionName = GetCollectionName<T>();
            string url = context.GetPath() + "/" + collectionName;

            String queryString = QueryStringBuilder.BuildQueryString(queryPhrases, fields, null, null, limit, null, null);

            ResponseWrapper response = await rc.ExecuteGetAsync(url, queryString);
            if (response.Data != null)
            {
                EntityListResult<T> result = jsonSerializer.Deserialize<EntityListResult<T>>(response.Data);
                return result;
            }
            return null;
        }

        private string GetCollectionName<T>() where T : BaseEntity
        {
            var customCollectionPathAttribute = (CustomCollectionPathAttribute)
                Attribute.GetCustomAttribute(typeof(T), typeof(CustomCollectionPathAttribute));

            string collectionName;
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

        public GroupResult GetWithGroupBy<T>(IRequestContext context, IList<QueryPhrase> queryPhrases, String groupBy) where T : BaseEntity
        {
            return GetWithGroupByAsync<T>(context, queryPhrases, groupBy).Result;
        }

        public async Task<GroupResult> GetWithGroupByAsync<T>(IRequestContext context, IList<QueryPhrase> queryPhrases, String groupBy) where T : BaseEntity
        {
            string collectionName = GetCollectionName<T>();
            string url = context.GetPath() + "/" + collectionName + "/groups";

            string queryString = QueryStringBuilder.BuildQueryString(queryPhrases, null, null, null, null, groupBy, null);

            ResponseWrapper response = await rc.ExecuteGetAsync(url, queryString);
            if (response.Data != null)
            {
                GroupResult result = jsonSerializer.Deserialize<GroupResult>(response.Data);
                return result;
            }
            return null;
        }

        public async Task<TestScript> GetTestScriptAsync(IRequestContext context, long id)
        {
            string url = string.Format("{0}/tests/{1}/script", context.GetPath(), id);
            ResponseWrapper response = await rc.ExecuteGetAsync(url, string.Empty);

            TestScript result = jsonSerializer.Deserialize<TestScript>(response.Data);
            return result;
        }

        public T GetById<T>(IRequestContext context, long id, IList<String> fields) where T : BaseEntity
        {
            return GetByIdAsync<T>(context, id, fields).Result;
        }

        public async Task<T> GetByIdAsync<T>(IRequestContext context, long id, IList<String> fields) where T : BaseEntity
        {
            String collectionName = GetCollectionName<T>();
            string url = context.GetPath() + "/" + collectionName + "/" + id;
            String queryString = QueryStringBuilder.BuildQueryString(null, fields, null, null, null, null, null);

            ResponseWrapper response = await rc.ExecuteGetAsync(url, queryString);
            T result = jsonSerializer.Deserialize<T>(response.Data);
            return result;
        }

        public EntityListResult<T> Create<T>(IRequestContext context, EntityList<T> entityList, IList<string> fieldsToReturn = null) where T : BaseEntity
        {
            return CreateAsync<T>(context, entityList, fieldsToReturn).Result;
        }

        public async Task<EntityListResult<T>> CreateAsync<T>(IRequestContext context, EntityList<T> entityList, IList<string> fieldsToReturn = null) where T : BaseEntity
        {
            string collectionName = GetCollectionName<T>();

            string queryParams = "";

            if (fieldsToReturn != null && fieldsToReturn.Count > 0)
            {
                queryParams += "fields=" + string.Join(",", fieldsToReturn);
            }

            string url = context.GetPath() + "/" + collectionName;
            String data = jsonSerializer.Serialize(entityList);
            ResponseWrapper response = await rc.ExecutePostAsync(url, queryParams, data);
            EntityListResult<T> result = jsonSerializer.Deserialize<EntityListResult<T>>(response.Data);
            return result;
        }

        public T Create<T>(IRequestContext context, T entity, IList<string> fieldsToReturn = null) where T : BaseEntity
        {
            return CreateAsync<T>(context, entity, fieldsToReturn).Result;
        }

        public async Task<T> CreateAsync<T>(IRequestContext context, T entity, IList<string> fieldsToReturn = null) where T : BaseEntity
        {
            EntityListResult<T> result = await CreateAsync<T>(context, EntityList<T>.Create(entity), fieldsToReturn);
            return result.data[0];
        }

        public T Update<T>(IRequestContext context, T entity, IList<string> fieldsToReturn = null) where T : BaseEntity
        {
            return UpdateAsync<T>(context, entity, fieldsToReturn).Result;
        }

        public Task<T> UpdateAsync<T>(IRequestContext context, T entity, IList<string> fieldsToReturn = null) where T : BaseEntity
        {
            return UpdateAsync<T>(context, entity, null, fieldsToReturn);
        }

        public T Update<T>(IRequestContext context, T entity, Dictionary<String, String> serviceArguments, IList<string> fieldsToReturn)
            where T : BaseEntity
        {
            return UpdateAsync<T>(context, entity, serviceArguments, fieldsToReturn).Result;
        }

        public async Task<T> UpdateAsync<T>(IRequestContext context, T entity, Dictionary<String, String> serviceArguments, IList<string> fieldsToReturn)
             where T : BaseEntity
        {
            String collectionName = GetCollectionName<T>();
            String queryString = QueryStringBuilder.BuildQueryString(null, fieldsToReturn, null, null, null, null, serviceArguments);


            string url = context.GetPath() + "/" + collectionName + "/" + entity.Id;
            String data = jsonSerializer.Serialize(entity);
            ResponseWrapper response = await rc.ExecutePutAsync(url, queryString, data);
            T result = jsonSerializer.Deserialize<T>(response.Data);
            return result;
        }

        public EntityListResult<T> UpdateEntities<T>(IRequestContext context, EntityList<T> entities)
            where T : BaseEntity
        {
            return UpdateEntitiesAsync<T>(context, entities).Result;
        }

        public async Task<EntityListResult<T>> UpdateEntitiesAsync<T>(IRequestContext context, EntityList<T> entities)
            where T : BaseEntity
        {
            String collectionName = GetCollectionName<T>();
            string url = context.GetPath() + "/" + collectionName;
            String data = jsonSerializer.Serialize(entities);
            ResponseWrapper response = await rc.ExecutePutAsync(url, null, data);
            EntityListResult<T> result = jsonSerializer.Deserialize<EntityListResult<T>>(response.Data);
            return result;
        }

        public void DeleteById<T>(IRequestContext context, long entityId)
             where T : BaseEntity
        {
            DeleteByIdAsync<T>(context, entityId).RunSynchronously();
        }

        public async Task DeleteByIdAsync<T>(IRequestContext context, long entityId)
             where T : BaseEntity
        {
            String collectionName = GetCollectionName<T>();
            string url = context.GetPath() + "/" + collectionName + "/" + entityId;
            ResponseWrapper response = await rc.ExecuteDeleteAsync(url);
        }

        public void DeleteByFilter<T>(IRequestContext context, IList<QueryPhrase> queryPhrases)
            where T : BaseEntity
        {
            DeleteByFilterAsync<T>(context, queryPhrases).RunSynchronously();
        }

        public async Task DeleteByFilterAsync<T>(IRequestContext context, IList<QueryPhrase> queryPhrases)
            where T : BaseEntity
        {
            String collectionName = GetCollectionName<T>();
            String queryString = QueryStringBuilder.BuildQueryString(queryPhrases, null, null, null, null, null, null);
            string url = context.GetPath() + "/" + collectionName + "?" + queryString;
            ResponseWrapper response = await rc.ExecuteDeleteAsync(url);
        }

        public Attachment AttachToEntity(IRequestContext context, BaseEntity entity, string fileName, byte[] content, string contentType, string[] fieldsToReturn)
        {
            return AttachToEntityAsync(context, entity, fileName, content, contentType, fieldsToReturn).Result;
        }

        public async Task<Attachment> AttachToEntityAsync(IRequestContext context, BaseEntity entity, string fileName, byte[] content, string contentType, string[] fieldsToReturn)
        {
            String queryString = QueryStringBuilder.BuildQueryString(null, fieldsToReturn, null, null, null, null, null);
            string url = context.GetPath() + "/attachments?" + queryString;
            string attachmentEntity = null;
            if (entity != null)
            {
                attachmentEntity = string.Format("{0}\"name\":\"{2}\",\"owner_{3}\":{0}\"type\":\"{3}\",\"id\":\"{4}\"{1}{1}",
                    "{", "}", fileName, entity.AggregateType, entity.Id.ToString());
            }
            ResponseWrapper response = await rc.SendMultiPartAsync(url, content, contentType, fileName, attachmentEntity);
            EntityListResult<Attachment> result = jsonSerializer.Deserialize<EntityListResult<Attachment>>(response.Data);
            return (Attachment)result.data[0];
        }

    }
}
