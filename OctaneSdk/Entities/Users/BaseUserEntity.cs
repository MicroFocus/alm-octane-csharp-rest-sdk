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



using MicroFocus.Adm.Octane.Api.Core.Services;
using System.Collections.Generic;

namespace MicroFocus.Adm.Octane.Api.Core.Entities
{

	public abstract class BaseUserEntity : BaseEntity
    {
        public static string UID_FIELD = "uid";
        public static string FULL_NAME_FIELD = "full_name";
        public static string FIRST_NAME_FIELD = "first_name";
        public static string LAST_NAME_FIELD = "last_name";
        public static string EMAIL_FIELD = "email";
        public static string PHONE1_FIELD = "phone1";
		public static string LANGUAGE_FIELD = "language";
		public static string PASSWORD_FIELD = "password";

		public BaseUserEntity()
            : base()
        {
        }

        public BaseUserEntity(EntityId id)
            : base(id)
        {
        }

        public string UID
        {
            get
            {
                return GetStringValue(UID_FIELD);
            }
            set
            {
                SetValue(UID_FIELD, value);
            }
        }

        public string FullName
        {
            get
            {
                return GetStringValue(FULL_NAME_FIELD);
            }
            set
            {
                SetValue(FULL_NAME_FIELD, value);
            }
        }

        public string FirstName
        {
            get
            {
                return GetStringValue(FIRST_NAME_FIELD);
            }
            set
            {
                SetValue(FIRST_NAME_FIELD, value);
            }
        }

        public string LastName
        {
            get
            {
                return GetStringValue(LAST_NAME_FIELD);
            }
            set
            {
                SetValue(LAST_NAME_FIELD, value);
            }
        }

        public string Email
        {
            get
            {
                return GetStringValue(EMAIL_FIELD);
            }
            set
            {
                SetValue(EMAIL_FIELD, value);
            }
        }

        public string Phone1
        {
            get
            {
                return GetStringValue(PHONE1_FIELD);
            }
            set
            {
                SetValue(PHONE1_FIELD, value);
            }
        }

		public string Language
		{
			get
			{
				return GetStringValue(LANGUAGE_FIELD);
			}
			set
			{
				SetValue(LANGUAGE_FIELD, value);
			}
		}

		public string Password
		{
			get
			{
				return GetStringValue(PASSWORD_FIELD);
			}
			set
			{
				SetValue(PASSWORD_FIELD, value);
			}
		}
    }
}
