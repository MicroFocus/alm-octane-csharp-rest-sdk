using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services;

namespace Hpe.Nga.Api.Core.Entities
{

    public abstract class BaseUserEntity : BaseEntity
    {
        public static string UID_FIELD = "uid";
        public static string FULL_NAME_FIELD = "full_name";
        public static string FIRST_NAME_FIELD = "first_name";
        public static string LAST_NAME_FIELD = "last_name";
        public static string EMAIL_FIELD = "email";
        public static string PHONE1_FIELD = "phone1";

        public BaseUserEntity()
            : base()
        {
        }

        public BaseUserEntity(long id)
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
    }
}
