using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroFocus.Adm.Octane.Api.Core.Entities
{
    public class SuiteRunScheduler : BaseEntity
    {
        public const string TYPE_SUITE_RUN_SCHEDULER = "suite_run_scheduler";
        public const string CREATION_TIME_FIELD = "creation_time";
        public const string LAST_MODIFIED_FIELD = "last_modified";
        public const string AUTHOR_FIELD = "author";
        public const string OWNER_FIELD = "owner";
        public const string DESCRIPTION_FIELD = "description";
        public const string RELEASE_FIELD = "release";
        public const string MILESTONE_FIELD = "milestone";
        public const string STATUS_FIELD = "status";

        public SuiteRunScheduler()
        {
            AggregateType = "suite_run_scheduler";
        }

        public SuiteRunScheduler(EntityId id) : base(id)
        {
            AggregateType = "suite_run_scheduler";
        }

        /// <summary>
        /// Creation time as returned by server (string representation).
        /// </summary>
        public string CreationTime
        {
            get { return GetStringValue(CREATION_TIME_FIELD); }
            set { SetValue(CREATION_TIME_FIELD, value); }
        }

        /// <summary>
        /// Last modified as returned by server (string representation).
        /// </summary>
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

        /// <summary>
        /// Release reference (use a stronger type if you have one in SDK).
        /// </summary>
        public BaseEntity Release
        {
            get { return (BaseEntity)GetValue(RELEASE_FIELD); }
            set { SetValue(RELEASE_FIELD, value); }
        }

        /// <summary>
        /// Milestone reference (use a stronger type if you have one in SDK).
        /// </summary>
        public BaseEntity Milestone
        {
            get { return (BaseEntity)GetValue(MILESTONE_FIELD); }
            set { SetValue(MILESTONE_FIELD, value); }
        }

        /// <summary>
        /// Status reference/value (use a stronger type if you have one in SDK).
        /// </summary>
        public BaseEntity Status
        {
            get { return (BaseEntity)GetValue(STATUS_FIELD); }
            set { SetValue(STATUS_FIELD, value); }
        }
    }
}
