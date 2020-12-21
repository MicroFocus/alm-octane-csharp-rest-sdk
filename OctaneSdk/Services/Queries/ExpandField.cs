using System.Collections.Generic;

namespace MicroFocus.Adm.Octane.Api.Core.Services.Query
{
    public class ExpandField
    {

        public static ExpandField Create(string field, ICollection<string> expandFields)
        {
            ExpandField e = new ExpandField();
            e.Field = field;
            e.AddExpandFields(expandFields);
            return e;
        }

        public string Field { get; set; }
        private HashSet<string> expandFields = new HashSet<string>();

        public ICollection<string> ExpandFields
        {
            get
            {
                return expandFields;
            }
        }

        

        public void AddExpandField(string field)
        {
            expandFields.Add(field);
        }

        public void AddExpandFields(ICollection<string> fields)
        {
            expandFields.UnionWith(fields);
        }
    }
}
