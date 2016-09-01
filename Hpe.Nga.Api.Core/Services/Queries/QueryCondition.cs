using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hpe.Nga.Api.Core.Services.Query
{
    public class QueryCondition
    {
        private string op = "=";

        public String FieldName { get; set; }
        public Object Expression { get; set; }
        public String Operator
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

        public QueryCondition()
        {
        }

        public QueryCondition(String fieldName, Object expression)
        {
            FieldName = fieldName;
            Expression = expression;
        }

        public QueryCondition(String fieldName, Object expression, String op)
        {
            FieldName = fieldName;
            Expression = expression;
            Operator = op;
        }

 
    }
}
