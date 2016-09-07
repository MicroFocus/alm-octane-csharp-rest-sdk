using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hpe.Nga.Api.Core.Services.Query
    
{
    /// <summary>
    /// Used to execute negative filter, for example : get all defects except defects in closed metaphase  
    /// </summary>
    public class NegativeQueryPhrase : QueryPhrase
    {

        public QueryPhrase QueryPhrase { get; set; }

        public NegativeQueryPhrase(QueryPhrase phrase)
           
        {
            this.QueryPhrase = phrase;
        }
    }
}
