using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services;

namespace Hpe.Nga.Api.Core.Entities
{
    /// <summary>
    /// Wrapper for Release entity
    /// More fields might be supported by entity that still are not exposed in the class
    /// </summary>
    public class Release : BaseEntity
    {
        public static string START_DATE_FIELD = "start_date";
        public static string END_DATE_FIELD = "end_date";
        public static string SPRINT_DURATION_FIELD = "sprint_duration";
        public static string NUM_OF_SPRINTS_FIELD = "num_of_sprints";

        public Release()
        {

        }

        public Release(long id) : base(id)
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


        public int SprintDuration
        {
            get
            {
                return GetIntValue(SPRINT_DURATION_FIELD, 0);
            }
            set
            {
                SetIntValue(SPRINT_DURATION_FIELD, value);
            }

        }

        public int NumOfSprints
        {
            get
            {
                return GetIntValue(NUM_OF_SPRINTS_FIELD, 0);
            }
            set
            {
                SetIntValue(NUM_OF_SPRINTS_FIELD, value);
            }

        }

    }
}
