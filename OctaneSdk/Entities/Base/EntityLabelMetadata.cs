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

namespace MicroFocus.Adm.Octane.Api.Core.Entities
{
    
    /// <summary>
    /// Wrapper for entity label metadata
    /// </summary>
    public class EntityLabelMetadata : DictionaryBasedEntity
    {
        public static string ENTITY_TYPE_FIELD = "entity_type";

        public const string NAME_FIELD = "name";
        public const string INITIALS_FIELD = "initials";
        public const string LANGUAGE = "language";
        
        public string Name
        {
            get { return GetStringValue(NAME_FIELD); }
        }

        public string Type
        {
            get { return GetStringValue(ENTITY_TYPE_FIELD); }
        }

        public string Initials
        {
            get { return GetStringValue(INITIALS_FIELD); }
        }

        public string Language
        {
            get { return GetStringValue(LANGUAGE); }
        }
    }
}
