
/*!
* (c) Copyright 2016-2021 Micro Focus or one of its affiliates.
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


using MicroFocus.Adm.Octane.Api.Core.Connector;
using MicroFocus.Adm.Octane.Api.Core.Entities;
using MicroFocus.Adm.Octane.Api.Core.Services.Core;
using MicroFocus.Adm.Octane.Api.Core.Services.Query;
using MicroFocus.Adm.Octane.Api.Core.Services.RequestContext;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MicroFocus.Adm.Octane.Api.Core.Services
{
    /// <summary>
    /// Apllication layer class.
    /// Wraps low-level <see cref="RestConnector"/> and exposes method for CRUD manipulating entities.
    /// The main point is non usage of generic methods, but with passing entity type as parameter.
    /// </summary>
    public class NonGenericsEntityService
    {
        private RestConnector rc;
        private EntityService es;
        private JavaScriptSerializer jsonSerializer;

        public NonGenericsEntityService(RestConnector rc)
        {
            this.rc = rc;
            this.es = new EntityService(rc);
            this.jsonSerializer = new JavaScriptSerializer();
            jsonSerializer.RegisterConverters(new JavaScriptConverter[] { new EntityJsonConverter(), new EntityIdJsonConverter() });
        }

        /// <summary>
        /// Returns the list of entities (only defined fields) for the given entity type filtered with query phrase
        /// </summary>
        public EntityListResult<BaseEntity> Get(IRequestContext context, string type, string queryPhrase, Object[] fields)
        {
            return GetListAsync(context, type, queryPhrase, fields, null, null, null).Result;
        }

        /// <summary>
        /// Returns the list of entities (only defined fields) for the given entity type filtered with query phrase and limit and offset
        /// </summary>
        public EntityListResult<BaseEntity> Get(IRequestContext context, string type, string queryPhrase, Object[] fields, int limit, int offset)
        {
            return GetListAsync(context, type, queryPhrase, fields, null, limit, offset).Result;
        }

        /// <summary>
        /// Returns the list of entities (only defined fields) for the given entity type filtered with query phrase and limit and offset.
        /// All entities will be sorted by given field (orderBy parameter).
        /// </summary>
        public EntityListResult<BaseEntity> Get(IRequestContext context, string type, string queryPhrase, Object[] fields, string orderBy, int limit, int offset)
        {
            return GetListAsync(context, type, queryPhrase, fields, orderBy, limit, offset).Result;
        }

        /// <summary>
        /// Returns the single entity (only defined fields) with the given type and id.
        /// </summary>
        public BaseEntity GetById(IRequestContext context, string type, string id, Object[] fields)
        {
            List<String> fieldsCasted = new List<String>(Array.ConvertAll<Object, String>(fields, Convert.ToString));
            return this.es.GetByIdAsync(context, id, type, fieldsCasted).Result;
        }

        private async Task<EntityListResult<BaseEntity>> GetListAsync(IRequestContext context, string apiEntityName, string queryStringValue, Object[] fields, string orderBy, int? limit, int? offset)
        {
            string url = context.GetPath() + "/" + apiEntityName;

            List<String> fieldsCasted = new List<String>(Array.ConvertAll<Object, String>(fields, Convert.ToString));

            string queryString = QueryBuilder.Create().SetQueryStringValue(queryStringValue).SetFields(fieldsCasted).SetOrderBy(orderBy).SetLimit(limit).SetOffset(offset).Build();
            ResponseWrapper response = await rc.ExecuteGetAsync(url, queryString).ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
            if (response.Data != null)
            {
                EntityListResult<BaseEntity> result = jsonSerializer.Deserialize<EntityListResult<BaseEntity>>(response.Data);
                return result;
            }
            return null;
        }

        /// <summary>
        /// Download the attachment at the url and store it locally at the given location
        /// </summary>
        public void DownloadAttachment(string relativeUrl, string destinationPath)
        {
            this.es.DownloadAttachmentAsync(relativeUrl, destinationPath).Wait();
        }
    }
}
