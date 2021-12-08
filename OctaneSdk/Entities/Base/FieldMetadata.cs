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

using MicroFocus.Adm.Octane.Api.Core.Services.Core;

namespace MicroFocus.Adm.Octane.Api.Core.Entities
{
    /// <summary>
    /// Wrapper for Field Metadata entity
    /// More fields might be supported by entity that still are not exposed in the class
    /// </summary>
    public class FieldMetadata : DictionaryBasedEntity
    {
        public static string ENTITY_NAME_FIELD = "entity_name";

        public const string NAME_FIELD = "name";
        public const string LABEL_FIELD = "label";
        public const string VISIBLE_IN_UI_FIELD = "visible_in_ui";
        public const string FIELD_TYPE_FIELD = "field_type";

        public string Name
        {
            get { return GetStringValue(NAME_FIELD); }
        }

        public string Label
        {
            get { return GetStringValue(LABEL_FIELD); }
        }

        public bool VisibleInUI
        {
            get { return (bool)GetValue(VISIBLE_IN_UI_FIELD); }
        }

        public string FieldType
        {
            get { return GetStringValue(FIELD_TYPE_FIELD); }
        }
    }
}
