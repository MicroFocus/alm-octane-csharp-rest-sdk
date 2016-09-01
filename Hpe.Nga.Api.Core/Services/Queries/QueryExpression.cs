using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hpe.Nga.Api.Core.Services.Query
{
    /// <summary>
    /// 
    /// Represent expression value of query, for example "=5" or ">5"
    /// </summary>
    public class QueryExpression
    {
        private ComparisonOperator op = ComparisonOperator.Equal;

        public QueryExpression()
        {

        }

        public QueryExpression(Object value)
        {
            Value = value;
        }

        public QueryExpression(Object value, ComparisonOperator op)
        {
            Value = value;
            Operator = op;
        }

        public Object Value { get; set; }
        public ComparisonOperator Operator
        {
            get
            {
                return op;
            }
            set
            {
                this.op = value;
            }
        }
    }
}
