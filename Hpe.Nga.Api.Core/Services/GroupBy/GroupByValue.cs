using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services.Core;

namespace Hpe.Nga.Api.Core.Services.GroupBy
{
    public class GroupByValue
    {
        public static string ID_FIELD = "id";
        public static string NAME_FIELD = "name";
        public static string TYPE_FIELD = "type";
        public static string LOGICAL_NAME_FIELD = "logical_name";

        public int id { get; set; }

        public String name { get; set; }

        public String type { get; set; }

        public String logical_name { get; set; }

    }
}
