using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services.Core;

namespace Hpe.Nga.Api.Core.Services.GroupBy
{
    public class Group 
    {
        public static string COUNT_FIELD = "count";
        public static string VALUE_FIELD = "value";


        public int count {   get; set; }

        public GroupByValue value {   get; set; }
    }
}
