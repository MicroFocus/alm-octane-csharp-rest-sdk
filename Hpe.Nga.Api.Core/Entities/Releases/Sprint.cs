using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services;

namespace Hpe.Nga.Api.Core.Entities
{
    /// <summary>
    /// Wrapper for Spring entity
    /// More fields might be supported by entity that still are not exposed in the class
    /// </summary>
    public class Sprint : BaseEntity
    {
        public static string START_DATE_FIELD = "start_date";
        public static string END_DATE_FIELD = "end_date";
        public static string RELEASE_FIELD = "release";

        public Sprint()
        {

        }

        public Sprint(long id)
            : base(id)
        {
        }

        public DateTime StartDate
        {
            get
            {
                return GetDateTimeValue(START_DATE_FIELD).Value;
            }
            set
            {
                SetDateTimeValue(START_DATE_FIELD, value);
            }

        }

        public DateTime EndDate
        {
            get
            {
                return GetDateTimeValue(END_DATE_FIELD).Value;
            }
            set
            {
                SetDateTimeValue(END_DATE_FIELD, value);
            }

        }

        public Release Release
        {
            get
            {
                return (Release)GetValue(RELEASE_FIELD);
            }
            set
            {
                SetValue(RELEASE_FIELD, value);
            }

        }

    }
}
