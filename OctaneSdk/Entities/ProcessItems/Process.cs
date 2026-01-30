using MicroFocus.Adm.Octane.Api.Core.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroFocus.Adm.Octane.Api.Core.Entities
{
    [CustomCollectionPath("processes")]
    public class Process : BaseEntity
    {
        public const string SUBTYPE_FIELD = "subtype";
        public const string CREATION_TIME_FIELD = "creation_time";
        public const string LAST_MODIFIED_FIELD = "last_modified";
        public const string AUTHOR_FIELD = "author";
        public const string OWNER_FIELD = "owner";
        public const string DESCRIPTION_FIELD = "description";
        public const string DURATION_FIELD = "duration";
        public const string START_TIME_FIELD = "start_time";
        public const string END_TIME_FIELD = "end_time";

        // known subtypes
        public const string SUBTYPE_AUTO_ACTION = "process_auto_action";
        public const string SUBTYPE_MANUAL_ACTION = "process_action";
        public const string SUBTYPE_QUALITY_GATE = "process_quality_gate";

        public Process(IDictionary<string, object> properties)
        : base(properties)
    {
        AggregateType = "processes";
    }

        public Process()
        {
            AggregateType = "processes";
        }

        public Process(EntityId id)
            : base(id)
        {
            AggregateType = "process";
        }

        public string SubType
        {
            get { return GetStringValue(SUBTYPE_FIELD); }
            set { SetValue(SUBTYPE_FIELD, value); }
        }

        public WorkspaceUser Author
        {
            get { return (WorkspaceUser)GetValue(AUTHOR_FIELD); }
            set { SetValue(AUTHOR_FIELD, value); }
        }

        public WorkspaceUser Owner
        {
            get { return (WorkspaceUser)GetValue(OWNER_FIELD); }
            set { SetValue(OWNER_FIELD, value); }
        }

        public string Description
        {
            get { return GetStringValue(DESCRIPTION_FIELD); }
            set { SetValue(DESCRIPTION_FIELD, value); }
        }

        /// <summary>
        /// Duration in seconds (or numeric unit returned by server). Use GetLongValue.
        /// </summary>
        public long? Duration
        {
            get { return GetLongValue(DURATION_FIELD); }
            set
            {
                if (value == null)
                    SetValue(DURATION_FIELD, null);
                else
                    SetLongValue(DURATION_FIELD, (long)value);
            }
        }

        /// <summary>
        /// Start time as returned by server (string representation); parse clientside if needed.
        /// </summary>
        public string StartTime
        {
            get { return GetStringValue(START_TIME_FIELD); }
            set { SetValue(START_TIME_FIELD, value); }
        }

        /// <summary>
        /// End time as returned by server(string representation)
        /// </summary>
        public string EndTime
        {
            get { return GetStringValue(END_TIME_FIELD); }
            set { SetValue(END_TIME_FIELD, value); }
        }
    }
}
