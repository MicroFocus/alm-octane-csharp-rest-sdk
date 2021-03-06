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



namespace MicroFocus.Adm.Octane.Api.Core.Entities
{
	/// <summary>
	/// Wrapper for Role entity
	/// More fields might be supported by entity that still are not exposed in the class
	/// </summary>
	public class Role : BaseEntity
    {
        // public static string LOGICAL_NAME_FIELD = "logical_name";

        public string LogicalName
        {
            get
            {
                return GetStringValue(LOGICAL_NAME_FIELD);
            }
            set
            {
                SetValue(LOGICAL_NAME_FIELD, value);
            }
        }
    }
}
