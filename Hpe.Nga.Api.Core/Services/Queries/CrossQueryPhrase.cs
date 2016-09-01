using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hpe.Nga.Api.Core.Services.Query
    
{
    /// <summary>
    /// Used to execute filter by cross entities, for example : get defects by "owner" name
    /// </summary>
    public class CrossQueryPhrase : QueryPhrase
    {
        public String FieldName { get; set; }

        public QueryPhrase QueryPhrase { get; set; }

        public CrossQueryPhrase(String fieldName, QueryPhrase queryPhrase)
           
        {
            this.FieldName = fieldName;
            this.QueryPhrase = queryPhrase;
        }
    }
}
