using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroFocus.Adm.Octane.Api.Core.Entities
{
    /// <summary>
    /// Used to retrieve information about specific fields 
    /// </summary>
    public class EntityReference : BaseEntity
    {
        //Dictionary for every type, with value the ApiEntityName
        private static readonly Dictionary<string, string> ApiEntityNameTypePairs = new Dictionary<string, string>
        {
            {"work_item", "work_items" },
            {"story", "work_items" },
            {"quality_story", "work_items" },
            {"defect", "work_items" },
            {"work_item_root", "work_items" },
            {"epic", "work_items" },
            {"feature", "work_items" },

            {"test", "tests" },
            {"test_manual", "tests" },
            {"gherkin_test", "tests" },
            {"test_automated", "tests" },
            {"test_suite", "tests" },

            {"task", "tasks" },
            {"phase", "phases" },
            {"transition", "transitions" },
            {"run", "runs" },
            {"run_manual", "runs" },
            {"run_suite", "runs" },

            {"comment", "comments" },

            {"workspace_user", "workspace_users" },
            {"team", "teams" },

            {"requirement", "requirements" },
            {"requirement_document", "requirements" },

            {"user_item", "user_items" },
            {"user_tag", "user_tags" },

            {"list_node", "list_nodes" },

            {"release", "releases" },

            {"sprint", "sprints" },

            {"product_area", "product_areas" },

            {"taxonomy_node", "taxonomy_nodes" },
            {"taxonomy_item_node", "taxonomy_nodes" }
        };
        // This is the name of the entity passed, usually plural, used for rest calls
        public string ApiEntityName { get; set; }

        //This is a marker that the entity is a subtype of another entity (usually work_item)
        public EntityReference SubtypeOf { get; set; }

        //In case this is a subtype, you need to give the value of the subtype field to filter on
        public string SubtypeFieldValue { get; set; }

        public EntityReference(string apiEntityName, string typeName)
        {
            this.ApiEntityName = apiEntityName;
            this.TypeName = typeName;
        }

        public EntityReference(EntityReference subtypeOf, String subtypeName)
        {
            this.SubtypeOf = subtypeOf;
            this.ApiEntityName = subtypeOf.ApiEntityName;
            this.SubtypeFieldValue = subtypeName; 
        }

        public static EntityReference getEntityType(BaseEntity entity)
        {
            if (entity.GetValue("subtype") != null)
            {
                string subtype = entity.GetValue("subtype").ToString();

                //try finding the subtype
                if (subtype != null)
                {
                    return new EntityReference(ApiEntityNameTypePairs[subtype], entity.TypeName);
                }
            }

            if (entity.GetValue("type") != null)
            {
                string type = entity.GetValue("type").ToString();

                //try finding the subtype
                if (type != null)
                {
                    return new EntityReference(ApiEntityNameTypePairs[type], entity.TypeName);
                }
            }

            return null;
        }

        public static EntityReference createEntityReferenceWithType(string type)
        {
            string apiEntityName = ApiEntityNameTypePairs[type];
            EntityReference entityReference = new EntityReference(apiEntityName, type);
            return entityReference;
        }

        public bool IsSubType()
        {
            return SubtypeOf != null;
        }


    }
}
