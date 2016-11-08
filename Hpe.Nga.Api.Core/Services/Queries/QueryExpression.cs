// (c) Copyright 2016 Hewlett Packard Enterprise Development LP

// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.

// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,

// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

// See the License for the specific language governing permissions and limitations under the License.

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
