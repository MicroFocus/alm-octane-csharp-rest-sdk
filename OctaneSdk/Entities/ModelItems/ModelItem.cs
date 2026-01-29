using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroFocus.Adm.Octane.Api.Core.Entities
{
    public class ModelItem : BaseEntity
    {
        public const string SUBTYPE_FIELD = "subtype";
        public const string CREATION_TIME_FIELD = "creation_time";
        public const string LAST_MODIFIED_FIELD = "last_modified";
        public const string AUTHOR_FIELD = "author";
        public const string OWNER_FIELD = "owner";
        public const string DESCRIPTION_FIELD = "description";
        public const string AUTOMATION_STATUS_FIELD = "automation_status";
        public const string PARENT_FIELD = "parent";
        public const string RISK_FIELD = "risk";

        public const string SUBTYPE_UNIT = "unit"; 
        public const string SUBTYPE_MODEL = "model";

        public ModelItem()
        {
            AggregateType = "model_item";
        }

        public ModelItem(EntityId id)
            : base(id)
        {
            AggregateType = "model_item";
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

        public BaseEntity Parent
        {
            get
            {
                return (BaseEntity)GetValue(PARENT_FIELD);
            }
            set
            {
                SetValue(PARENT_FIELD, value);
            }
        }

        public string Description
        {
            get { return GetStringValue(DESCRIPTION_FIELD); }
            set { SetValue(DESCRIPTION_FIELD, value); }
        }
    }
}