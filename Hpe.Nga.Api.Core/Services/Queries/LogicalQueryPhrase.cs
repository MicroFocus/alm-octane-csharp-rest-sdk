using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hpe.Nga.Api.Core.Services.Query
{
    public class LogicalQueryPhrase : QueryPhrase
    {
        public String FieldName { get; set; }
        public bool NegativeCondition { get; set; }
        public List<QueryExpression> Expressions { get; set; }

        public LogicalQueryPhrase(String fieldName)
        {
            FieldName = fieldName;
            Expressions = new List<QueryExpression>();
        }

        public LogicalQueryPhrase(String fieldName, Object queryValue)
            : this(fieldName)
        {
            Expressions.Add(new QueryExpression(queryValue));
        }

        public LogicalQueryPhrase(String fieldName, Object queryValue, ComparisonOperator op)
            : this(fieldName)
        {
            AddExpression(queryValue, op);
        }

        public void AddExpression(Object queryValue, ComparisonOperator op)
        {
            AddExpression(new QueryExpression(queryValue, op));

        }

        public void AddExpression(QueryExpression expression)
        {
            Expressions.Add(expression);

        }


    }
}
