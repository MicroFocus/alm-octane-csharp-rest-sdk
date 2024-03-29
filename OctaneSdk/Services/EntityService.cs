
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


using MicroFocus.Adm.Octane.Api.Core.Connector;
using MicroFocus.Adm.Octane.Api.Core.Entities;
using MicroFocus.Adm.Octane.Api.Core.Entities.Base;
using MicroFocus.Adm.Octane.Api.Core.Services.Core;
using MicroFocus.Adm.Octane.Api.Core.Services.GroupBy;
using MicroFocus.Adm.Octane.Api.Core.Services.Query;
using MicroFocus.Adm.Octane.Api.Core.Services.RequestContext;
using MicroFocus.Adm.Octane.Api.Core.Services.Version;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Task = System.Threading.Tasks.Task;

namespace MicroFocus.Adm.Octane.Api.Core.Services
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
            jsonSerializer.RegisterConverters(new JavaScriptConverter[] { new EntityJsonConverter(), new EntityIdJsonConverter() });
        }

        public EntityListResult<T> Get<T>(IRequestContext context) where T : BaseEntity
        {
            return GetResultOrThrowInnerException(GetAsync<T>(context));
        }

        public Task<EntityListResult<T>> GetAsync<T>(IRequestContext context) where T : BaseEntity
        {
            return GetAsync<T>(context, null, null);
        }

        public EntityListResult<T> Get<T>(IRequestContext context, IList<QueryPhrase> queryPhrases, List<string> fields) where T : BaseEntity
        {
            return GetResultOrThrowInnerException(GetAsync<T>(context, queryPhrases, fields));
        }

        public Task<EntityListResult<T>> GetAsync<T>(IRequestContext context, IList<QueryPhrase> queryPhrases, List<string> fields) where T : BaseEntity
        {
            return GetAsync<T>(context, queryPhrases, fields, null);
        }

        public EntityListResult<T> Get<T>(IRequestContext context, IList<QueryPhrase> queryPhrases, List<string> fields, int? limit) where T : BaseEntity
        {
            return GetResultOrThrowInnerException(GetAsync<T>(context, queryPhrases, fields, limit));
        }

        public async Task<EntityListResult<T>> GetAsync<T>(IRequestContext context, IList<QueryPhrase> queryPhrases, List<string> fields, int? limit) where T : BaseEntity
        {
            string collectionName = EntityTypeRegistry.GetInstance().GetCollectionName(typeof(T));
            string url = context.GetPath() + "/" + collectionName;

            string queryString = QueryBuilder.Create().SetQueryPhrases(queryPhrases).SetFields(fields).SetLimit(limit).Build(); 

            ResponseWrapper response = await rc.ExecuteGetAsync(url, queryString).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
            if (response.Data != null)
            {
                EntityListResult<T> result = jsonSerializer.Deserialize<EntityListResult<T>>(response.Data);
                return result;
            }
            return null;
        }

        public async Task<EntityListResult<BaseEntity>> GetAsyncReferenceFields(IRequestContext context, string apiEntityName, IList<QueryPhrase> queryPhrases, List<String> fields, string orderBy, int? limit)
        {
            string url = context.GetPath() + "/" + apiEntityName;

            string queryString = QueryBuilder.Create().SetQueryPhrases(queryPhrases).SetFields(fields).SetOrderBy(orderBy).SetLimit(limit).Build();

            ResponseWrapper response = await rc.ExecuteGetAsync(url, queryString).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
            if (response.Data != null)
            {
                EntityListResult<BaseEntity> result = jsonSerializer.Deserialize<EntityListResult<BaseEntity>>(response.Data);
                return result;
            }
            return null;
        }

        public async Task<EntityListResult<BaseEntity>> GetAsyncReferenceFields(IRequestContext context, string apiEntityName, IList<QueryPhrase> queryPhrases, List<string> fields, int? limit)
        {
            return await GetAsyncReferenceFields(context, apiEntityName, queryPhrases, fields, null, limit);
        }

        public GroupResult GetWithGroupBy<T>(IRequestContext context, IList<QueryPhrase> queryPhrases, string groupBy) where T : BaseEntity
        {
            return GetResultOrThrowInnerException(GetWithGroupByAsync<T>(context, queryPhrases, groupBy));
        }

        public async Task<GroupResult> GetWithGroupByAsync<T>(IRequestContext context, IList<QueryPhrase> queryPhrases, string groupBy) where T : BaseEntity
        {
            string collectionName = EntityTypeRegistry.GetInstance().GetCollectionName(typeof(T));
            string url = context.GetPath() + "/" + collectionName + "/groups";

            // Octane group API now return logical name by default as ID field,
            // this parameter change this to return numeric ID.
            var serviceArgs = new Dictionary<string, string>();
            serviceArgs.Add("use_numeric_id", "true");

            string queryString = QueryBuilder.Create().SetQueryPhrases(queryPhrases).SetGroupBy(groupBy).SetServiceArguments(serviceArgs).Build();


            ResponseWrapper response = await rc.ExecuteGetAsync(url, queryString).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
            if (response.Data != null)
            {
                GroupResult result = jsonSerializer.Deserialize<GroupResult>(response.Data);
                return result;
            }
            return null;
        }

        public async Task<TestScript> GetTestScriptAsync(IRequestContext context, EntityId id)
        {
            string url = string.Format("{0}/tests/{1}/script", context.GetPath(), id);
            ResponseWrapper response = await rc.ExecuteGetAsync(url, string.Empty).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);

            TestScript result = jsonSerializer.Deserialize<TestScript>(response.Data);
            return result;
        }

        public T GetById<T>(IRequestContext context, EntityId id, IList<String> fields) where T : BaseEntity
        {
            return GetResultOrThrowInnerException(GetByIdAsync<T>(context, id, fields));
        }

        private T GetResultOrThrowInnerException<T>(Task<T> task)
        {
            try
            {
                return task.Result;
            }
            catch (AggregateException aggrEx)
            {
                // This capture the inner exception stack trace and rethrow it without
                // resetting the stack trace to this line as would have happen if
                // we'll just use `throw aggrEx.InnerException`.
                ExceptionDispatchInfo.Capture(aggrEx.InnerException).Throw();

                // This return never actually happens but it is required to satisfy
                // the compiler check that all code path return a value.
                return default(T);
            }
        }

        public async Task<T> GetByIdAsync<T>(IRequestContext context, EntityId id, IList<string> fields) where T : BaseEntity
        {
            var entity = (T)await GetByIdInternalAsync(context, id, typeof(T), fields).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
            return entity;
        }

        public async Task<BaseEntity> GetByIdAsync(IRequestContext context, EntityId id, string type, IList<string> fields)
        {
            Type entityType = EntityTypeRegistry.GetInstance().GetTypeByEntityTypeName(type);
            return await GetByIdInternalAsync(context, id, entityType, fields).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
        }

        private async Task<BaseEntity> GetByIdInternalAsync(IRequestContext context, EntityId id, Type entityType, IList<string> fields)
        {
            string collectionName = EntityTypeRegistry.GetInstance().GetCollectionName(entityType);
            string url = context.GetPath() + "/" + collectionName + "/" + id;
            string queryString = QueryBuilder.Create().SetFields(fields).Build();

            ResponseWrapper response = await rc.ExecuteGetAsync(url, queryString).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
            BaseEntity result = (BaseEntity)jsonSerializer.Deserialize(response.Data, entityType);
            return result;
        }

        public EntityListResult<T> Create<T>(IRequestContext context, EntityList<T> entityList, IList<string> fieldsToReturn = null) where T : BaseEntity
        {
            return GetResultOrThrowInnerException(CreateAsync<T>(context, entityList, fieldsToReturn));
        }

        public async Task<EntityListResult<T>> CreateAsync<T>(IRequestContext context, EntityList<T> entityList, IList<string> fieldsToReturn = null) where T : BaseEntity
        {
            string collectionName = EntityTypeRegistry.GetInstance().GetCollectionName(typeof(T));

            string queryParams = "";

            if (fieldsToReturn != null && fieldsToReturn.Count > 0)
            {
                queryParams += "fields=" + string.Join(",", fieldsToReturn);
            }

            string url = context.GetPath() + "/" + collectionName;
            string data = jsonSerializer.Serialize(entityList);

            ResponseWrapper response = await rc.ExecutePostAsync(url, queryParams, data).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
            EntityListResult<T> result = jsonSerializer.Deserialize<EntityListResult<T>>(response.Data);
            return result;

        }

        public T Create<T>(IRequestContext context, T entity, IList<string> fieldsToReturn = null) where T : BaseEntity
        {
            return GetResultOrThrowInnerException(CreateAsync<T>(context, entity, fieldsToReturn));
        }

        public async Task<T> CreateAsync<T>(IRequestContext context, T entity, IList<string> fieldsToReturn = null) where T : BaseEntity
        {
            EntityListResult<T> result = await CreateAsync<T>(context, EntityList<T>.Create(entity), fieldsToReturn).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
            return result.data[0];
        }

        public T Update<T>(IRequestContext context, T entity, IList<string> fieldsToReturn = null) where T : BaseEntity
        {
            return GetResultOrThrowInnerException(UpdateAsync<T>(context, entity, fieldsToReturn));
        }

        public Task<T> UpdateAsync<T>(IRequestContext context, T entity, IList<string> fieldsToReturn = null) where T : BaseEntity
        {
            return UpdateAsync<T>(context, entity, null, fieldsToReturn);
        }

        public T Update<T>(IRequestContext context, T entity, Dictionary<string, string> serviceArguments, IList<string> fieldsToReturn)
            where T : BaseEntity
        {
            return GetResultOrThrowInnerException(UpdateAsync<T>(context, entity, serviceArguments, fieldsToReturn));
        }

        public async Task<T> UpdateAsync<T>(IRequestContext context, T entity, Dictionary<string, string> serviceArguments, IList<string> fieldsToReturn)
             where T : BaseEntity
        {
            return (T)await UpdateInternalAsync(context, entity, typeof(T), serviceArguments, fieldsToReturn).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
        }

        public async Task<BaseEntity> UpdateAsync(IRequestContext context, BaseEntity entity, string type, Dictionary<string, string> serviceArguments = null, IList<string> fieldsToReturn = null)
        {
            Type entityType = EntityTypeRegistry.GetInstance().GetTypeByEntityTypeName(type);
            return await UpdateInternalAsync(context, entity, entityType, serviceArguments, fieldsToReturn).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
        }

        private async Task<BaseEntity> UpdateInternalAsync(IRequestContext context, BaseEntity entity, Type entityType, Dictionary<string, string> serviceArguments, IList<string> fieldsToReturn)
        {
            string collectionName = EntityTypeRegistry.GetInstance().GetCollectionName(entityType);
            string queryString = QueryBuilder.Create().SetFields(fieldsToReturn).SetServiceArguments(serviceArguments).Build();

            string url = context.GetPath() + "/" + collectionName + "/" + entity.Id;
            string data = jsonSerializer.Serialize(entity);
            ResponseWrapper response = await rc.ExecutePutAsync(url, queryString, data).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
            BaseEntity result = (BaseEntity)jsonSerializer.Deserialize(response.Data, entityType);
            return result;
        }

        public EntityListResult<T> UpdateEntities<T>(IRequestContext context, EntityList<T> entities)
            where T : BaseEntity
        {
            return GetResultOrThrowInnerException(UpdateEntitiesAsync<T>(context, entities));
        }

        public async Task<EntityListResult<T>> UpdateEntitiesAsync<T>(IRequestContext context, EntityList<T> entities)
            where T : BaseEntity
        {
            string collectionName = EntityTypeRegistry.GetInstance().GetCollectionName(typeof(T));
            string url = context.GetPath() + "/" + collectionName;
            string data = jsonSerializer.Serialize(entities);
            ResponseWrapper response = await rc.ExecutePutAsync(url, null, data).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
            EntityListResult<T> result = jsonSerializer.Deserialize<EntityListResult<T>>(response.Data);
            return result;
        }

        public void DeleteById<T>(IRequestContext context, EntityId entityId)
             where T : BaseEntity
        {
            DeleteByIdAsync<T>(context, entityId).Wait();
        }

        public async Task DeleteByIdAsync<T>(IRequestContext context, EntityId entityId)
             where T : BaseEntity
        {
            string collectionName = EntityTypeRegistry.GetInstance().GetCollectionName(typeof(T));
            string url = context.GetPath() + "/" + collectionName + "/" + entityId;
            ResponseWrapper response = await rc.ExecuteDeleteAsync(url).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
        }

        public void DeleteByFilter<T>(IRequestContext context, IList<QueryPhrase> queryPhrases)
            where T : BaseEntity
        {
            DeleteByFilterAsync<T>(context, queryPhrases).Wait();
        }

        public async Task DeleteByFilterAsync<T>(IRequestContext context, IList<QueryPhrase> queryPhrases)
            where T : BaseEntity
        {
            string collectionName = EntityTypeRegistry.GetInstance().GetCollectionName(typeof(T));
            string queryString = QueryBuilder.Create().SetQueryPhrases(queryPhrases).Build();
            string url = context.GetPath() + "/" + collectionName + "?" + queryString;
            ResponseWrapper response = await rc.ExecuteDeleteAsync(url).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
        }

        public Attachment AttachToEntity(IRequestContext context, BaseEntity entity, string fileName, byte[] content, string contentType, string[] fieldsToReturn)
        {
            return GetResultOrThrowInnerException(AttachToEntityAsync(context, entity, fileName, content, contentType, fieldsToReturn));
        }

        public async Task<Attachment> AttachToEntityAsync(IRequestContext context, BaseEntity entity, string fileName, byte[] content, string contentType, string[] fieldsToReturn)
        {
            string queryString = QueryBuilder.Create().SetFields(fieldsToReturn).Build();
            string url = context.GetPath() + "/attachments?" + queryString;
            string attachmentEntity = null;
            if (entity != null)
            {
                attachmentEntity = string.Format("{0}\"name\":\"{2}\",\"owner_{3}\":{0}\"type\":\"{3}\",\"id\":\"{4}\"{1}{1}",
                    "{", "}", fileName, entity.AggregateType, entity.Id.ToString());
            }
            ResponseWrapper response = await rc.SendMultiPartAsync(url, content, contentType, fileName, attachmentEntity).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
            EntityListResult<Attachment> result = jsonSerializer.Deserialize<EntityListResult<Attachment>>(response.Data);
            return result.data[0];
        }

        /// <summary>
        /// Returns the fields' metadata for the given entity type
        /// </summary>
        public async Task<ListResult<FieldMetadata>> GetFieldsMetadataAsync(IRequestContext context, string entityType)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (string.IsNullOrEmpty(entityType))
            {
                throw new ArgumentException("entityType parameter is null or empty");
            }

            string url = context.GetPath() + "/metadata/fields";

            var query = new List<QueryPhrase>
            {
                new LogicalQueryPhrase(FieldMetadata.ENTITY_NAME_FIELD, entityType)
            };
            var queryString = QueryBuilder.BuildQueryString(query);

            ResponseWrapper response = await rc.ExecuteGetAsync(url, queryString).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
            if (response.Data == null)
            {
                return null;
            }

            return jsonSerializer.Deserialize<ListResult<FieldMetadata>>(response.Data);
        }

        /// <summary>
        /// Returns the label metadata for the entities
        /// </summary>
        public async Task<ListResult<EntityLabelMetadata>> GetLabelMetadataAsync(IRequestContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            string url = context.GetPath() + "/entity_labels";

            ResponseWrapper response = await rc.ExecuteGetAsync(url, "").ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);

            if (response.Data == null)
            {
                return null;
            }

            return jsonSerializer.Deserialize<ListResult<EntityLabelMetadata>>(response.Data);
        }

        /// <summary>
        /// Search for all entities of given type that satify the search criteria
        /// </summary>
        public async Task<EntityListResult<T>> SearchAsync<T>(IRequestContext context, string searchString, List<string> subTypes, int limit = 30)
            where T : BaseEntity
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (string.IsNullOrEmpty(searchString))
            {
                throw new ArgumentException("searchString parameter is null or empty");
            }
            if (limit <= 0)
            {
                throw new ArgumentException("search limit should be greater than 0");
            }

            string url = context.GetPath() + "/" + EntityTypeRegistry.GetInstance().GetCollectionName(typeof(T));

            List<QueryPhrase> query = null;
            if (subTypes != null && subTypes.Count > 0)
            {
                query = new List<QueryPhrase> { new InQueryPhrase("subtype", subTypes) };
            }

            var serviceArguments = new Dictionary<string, string>
            {
                { "text_search", "{\"type\":\"global\",\"text\":\"" + searchString + "\"}" }
            };
            var queryString = QueryBuilder.Create().SetQueryPhrases(query).SetOrderBy("id").SetLimit(limit).SetServiceArguments(serviceArguments).Build();

            ResponseWrapper response = await rc.ExecuteGetAsync(url, queryString).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
            EntityListResult<T> result = jsonSerializer.Deserialize<EntityListResult<T>>(response.Data);

            foreach (var entity in result.data)
            {
                var searchResult = entity.GetValue("global_text_search_result") as BaseEntity;
                if (searchResult == null)
                {
                    continue;
                }

                entity.SetValue("name", searchResult.GetValue("name"));
                entity.SetValue("description", searchResult.GetValue("description"));
            }
            return result;
        }

        /// <summary>
        /// Download the attachment at the url and store it locally at the given location
        /// </summary>
        public void DownloadAttachment(string relativeUrl, string destinationPath)
        {
            DownloadAttachmentAsync(relativeUrl, destinationPath).Wait();
        }

        /// <summary>
        /// Async operation for downloading the attachment at the url and store it locally at the given location
        /// </summary>
        public async Task DownloadAttachmentAsync(string relativeUrl, string destinationPath)
        {
            if (string.IsNullOrEmpty(relativeUrl))
            {
                throw new ArgumentException("relativeUrl parameter is null or empty");
            }
            if (string.IsNullOrEmpty(destinationPath))
            {
                throw new ArgumentException("destinationPath parameter is null or empty");
            }

            await rc.DownloadAttachmentAsync(relativeUrl, destinationPath).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
        }

        /// <summary>
        /// Validate that the given commit message satisfies Octane's commit template
        /// </summary>
        public CommitPattern ValidateCommitMessage(IRequestContext context, string commitMessage)
        {
            return ValidateCommitMessageAsync(context, commitMessage).Result;
        }

        /// <summary>
        /// Async operation for validating that the given commit message satisfies Octane's commit template
        /// </summary>
        public async Task<CommitPattern> ValidateCommitMessageAsync(IRequestContext context, string commitMessage)
        {
            if (context == null)
            {
                throw new ArgumentException("context parameter is null");
            }
            if (string.IsNullOrEmpty(commitMessage))
            {
                throw new ArgumentException("commitMessage parameter is null or empty");
            }

            string url = context.GetPath().Replace("/api", "/internal-api") + "/ali/validateCommitPattern";
            ResponseWrapper response = await rc.ExecuteGetAsync(url, "comment=" + commitMessage).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
            if (response.Data == null)
            {
                return null;
            }

            return jsonSerializer.Deserialize<CommitPattern>(response.Data);
        }

        /// <summary>
        /// Return all transitions for a given entity type
        /// </summary>
        public async Task<EntityListResult<Transition>> GetTransitionsForEntityType(IRequestContext context, string entityType)
        {
            var query = new List<QueryPhrase>
            {
                new LogicalQueryPhrase("entity", entityType)
            };

            var result = await GetAsync<Transition>(context, query, new List<string> { Transition.SOURCE_PHASE_FIELD, Transition.TARGET_PHASE_FIELD, Transition.IS_PRIMARY_FIELD, Phase.ENTITY_FIELD }).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);

            return result;
        }


        /// <summary>
        /// Return the octane version
        /// </summary>
        public async Task<OctaneVersion> GetOctaneVersion()
        {
            string url = "/admin/server/version";
            ResponseWrapper response = await rc.ExecuteGetAsync(url, null).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
            if (response.Data == null)
            {
                return null;
            }

            return new OctaneVersion(jsonSerializer.Deserialize<OctaneVersionMetadata>(response.Data).display_version);
        }

    }
}
