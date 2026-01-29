using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroFocus.Adm.Octane.Api.Core.Entities.SuiteRuns
{
    public class SuiteRunSchedulerRun : BaseEntity
    {
        public const string CREATION_TIME_FIELD = "creation_time";
        public const string LAST_MODIFIED_FIELD = "last_modified";
        public const string AUTHOR_FIELD = "author";
        public const string OWNER_FIELD = "owner";
        public const string DESCRIPTION_FIELD = "description";
        public const string STATUS_FIELD = "status";
        public const string START_TIME_FIELD = "start_time";
        public const string DURATION_FIELD = "duration";
        public const string GENERAL_RUN_STATUS_FIELD = "general_run_status";

        public SuiteRunSchedulerRun()
        {
            AggregateType = "suite_run_scheduler_run";
        }

        public SuiteRunSchedulerRun(EntityId id) : base(id)
        {
            AggregateType = "suite_run_scheduler_run";
        }

        public string CreationTime
        {
            get { return GetStringValue(CREATION_TIME_FIELD); }
            set { SetValue(CREATION_TIME_FIELD, value); }
        }

        public string LastModified
        {
            get { return GetStringValue(LAST_MODIFIED_FIELD); }
            set { SetValue(LAST_MODIFIED_FIELD, value); }
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

        public BaseEntity Status
        {
            get { return (BaseEntity)GetValue(STATUS_FIELD); }
            set { SetValue(STATUS_FIELD, value); }
        }

        /// <summary>
        /// Start time as returned by server (string representation); parse client-side if needed.
        /// </summary>
        public string StartTime
        {
            get { return GetStringValue(START_TIME_FIELD); }
            set { SetValue(START_TIME_FIELD, value); }
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
        /// General run status value/reference (depends on server model).
        /// Keep as string if it is a primitive field, or change to BaseEntity if it is a reference.
        /// </summary>
        public string GeneralRunStatus
        {
            get { return GetStringValue(GENERAL_RUN_STATUS_FIELD); }
            set { SetValue(GENERAL_RUN_STATUS_FIELD, value); }
        }
    }
}
