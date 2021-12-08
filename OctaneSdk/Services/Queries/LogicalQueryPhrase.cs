﻿/*!
* (c) Copyright 2016-2021 Micro Focus or one of its affiliates.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/


using System;
using System.Collections.Generic;

namespace MicroFocus.Adm.Octane.Api.Core.Services.Query
{
	public class LogicalQueryPhrase : QueryPhrase
    {
        public string FieldName { get; set; }
        public bool NegativeCondition { get; set; }
        public List<QueryExpression> Expressions { get; set; }

        public LogicalQueryPhrase(string fieldName)
        {
            FieldName = fieldName;
            Expressions = new List<QueryExpression>();
        }

        public LogicalQueryPhrase(string fieldName, object queryValue)
            : this(fieldName)
        {
            Expressions.Add(new QueryExpression(queryValue));
        }

        public LogicalQueryPhrase(string fieldName, object queryValue, ComparisonOperator op)
            : this(fieldName)
        {
            AddExpression(queryValue, op);
        }

        public void AddExpression(object queryValue, ComparisonOperator op)
        {
            AddExpression(new QueryExpression(queryValue, op));

        }

        public void AddExpression(QueryExpression expression)
        {
            Expressions.Add(expression);

        }


    }
}
