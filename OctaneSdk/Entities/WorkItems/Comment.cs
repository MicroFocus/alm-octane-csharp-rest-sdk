namespace MicroFocus.Adm.Octane.Api.Core.Entities.WorkItems
{
    /// <summary>
    /// Wrapper for Comment entity.
    /// </summary>
    public class Comment : WorkItem
    {
        public const string AUTHOR_FIELD = "author";
        public const string OWNER_WORK_FIELD = "owner_work_item";
        public const string OWNER_TEST_FIELD = "owner_test";
        public const string OWNER_RUN_FIELD = "owner_run";
        public const string TEXT_FIELD = "text";

        public Comment()
        {
        }

        public Comment(EntityId id)
            : base(id)
        {
        }

        public BaseEntity OwnerWorkItem => (BaseEntity)GetValue(OWNER_WORK_FIELD);

        public BaseEntity OwnerTest => (BaseEntity)GetValue(OWNER_TEST_FIELD);

        public BaseEntity OwnerRun => (BaseEntity)GetValue(OWNER_RUN_FIELD);
    }
}
