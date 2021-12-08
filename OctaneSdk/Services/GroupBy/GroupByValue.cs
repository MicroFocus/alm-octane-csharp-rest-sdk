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


using System;

namespace MicroFocus.Adm.Octane.Api.Core.Services.GroupBy
{
	public class GroupByValue
    {
        public static string ID_FIELD = "id";
        public static string NAME_FIELD = "name";
        public static string TYPE_FIELD = "type";
        public static string LOGICAL_NAME_FIELD = "logical_name";

        public int id { get; set; }

        public string name { get; set; }

        public string type { get; set; }

        public string logical_name { get; set; }

    }
}
