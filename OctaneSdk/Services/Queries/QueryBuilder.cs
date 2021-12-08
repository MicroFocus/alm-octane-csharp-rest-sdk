/*!
* (c) Copyright 2021 Micro Focus or one of its affiliates.
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
using System.Text;

namespace MicroFocus.Adm.Octane.Api.Core.Services.Query
{
    /// <summary>
    /// Builder of query string for rest request , like this : query="id>1001"&order-by=end_date&fields=id,name,end_date&offset=1&limit=3
    /// </summary>
    public class QueryBuilder
    {
        private IList<QueryPhrase> queryPhrases;
        private IList<string> fields;
        private IList<ExpandField> expandFields;

        public string orderBy;
        private int? offset;
        private int? limit;
        private string groupBy;
        private Dictionary<string, string> serviceArguments;

        public QueryBuilder SetQueryPhrases(IList<QueryPhrase> queryPhrases)
        {
            this.queryPhrases = queryPhrases;
            return this;
        }

        public QueryBuilder SetFields(IList<string> fields)
        {
            this.fields = fields;
            return this;
        }

        public QueryBuilder SetExpandFields(IList<ExpandField> expandFields)
        {
            this.expandFields = expandFields;
            return this;
        }

        public QueryBuilder SetOrderBy(string orderBy)
        {
            this.orderBy = orderBy;
            return this;
        }

        public QueryBuilder SetOffset(int? offset)
        {
            this.offset = offset;
            return this;
        }

        public QueryBuilder SetLimit(int? limit)
        {
            this.limit = limit;
            return this;
        }

        public QueryBuilder SetGroupBy(string groupBy)
        {
            this.groupBy = groupBy;
            return this;
        }

        public QueryBuilder SetServiceArguments(Dictionary<string, string> serviceArguments)
        {
            this.serviceArguments = serviceArguments;
            return this;
        }

        public static QueryBuilder Create()
        {
            return new QueryBuilder();

        }

        /// <summary>
        /// 
        /// build full query string like this : query="id>1001"&order-by=end_date&fields=id,name,end_date&offset=1&limit=3
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            string str = string.Empty;
            str = ConcatNewQueryString(str, BuildGroupByString(groupBy));
            str = ConcatNewQueryString(str, BuildQueryString(queryPhrases));
            str = ConcatNewQueryString(str, BuildOrderByString(orderBy));
            str = ConcatNewQueryString(str, BuildFieldsString(fields, expandFields));
            str = ConcatNewQueryString(str, BuildOffsetString(offset));
            str = ConcatNewQueryString(str, BuildLimitString(limit));
            str = ConcatNewQueryString(str, BuildServiceArguments(serviceArguments));
            return str;
        }

        private static string BuildPhraseString(QueryPhrase phrase)
        {
            string output = null;
            if (phrase is LogicalQueryPhrase)
            {
                LogicalQueryPhrase logicalPhrase = (LogicalQueryPhrase)phrase;

                List<string> expStrings = new List<string>();
                foreach (QueryExpression exp in logicalPhrase.Expressions)
                {
                    string comparisonOperator = GetComparisonOperatorString(exp.Operator);
                    string valueStr = GetExpressionValueString(exp.Value);
                    string expStr = string.Format("{0}{1}{2}", logicalPhrase.FieldName, comparisonOperator, valueStr);
                    expStrings.Add(expStr);
                }

                output = string.Join("||", expStrings);
                if (expStrings.Count > 1)
                {
                    output = "(" + output + ")";
                }
            }
            else if (phrase is CrossQueryPhrase)
            {
                //release={id=5002}
                CrossQueryPhrase crossPhrase = (CrossQueryPhrase)phrase;
                List<String> crossPhraseStrings = new List<String>();
                foreach (QueryPhrase tempPhrase in crossPhrase.QueryPhrases)
                {
                    string phraseString = BuildPhraseString(tempPhrase);
                    crossPhraseStrings.Add(phraseString);

                }
                string crossJoin = string.Join(";", crossPhraseStrings);

                string expStr = string.Format("{0}={{{1}}}", crossPhrase.FieldName, crossJoin);
                output = expStr;
            }
            else if (phrase is NegativeQueryPhrase)
            {
                NegativeQueryPhrase negativePhrase = (NegativeQueryPhrase)phrase;
                string expStr = string.Format("!{0}", BuildPhraseString(negativePhrase.QueryPhrase));
                output = expStr;
            }
            else if (phrase is InQueryPhrase)
            {
                InQueryPhrase inQueryPhrase = (InQueryPhrase)phrase;
                StringBuilder sb = new StringBuilder("(");
                sb.Append(inQueryPhrase.FieldName);
                sb.Append("+IN+");

                for (var i = 0; i < inQueryPhrase.Values.Count; i++)
                {
                    sb.Append(GetExpressionValueString(inQueryPhrase.Values[i]));

                    if (i != inQueryPhrase.Values.Count - 1)
                    {
                        sb.Append(",");
                    }
                }

                sb.Append(")");
                output = sb.ToString();
            }
            else if (phrase is NullQueryPhrase)
            {
                output = "null";
            }
            else
            {
                throw new NotImplementedException();
            }
            return output;
        }

        private static string GetExpressionValueString(object value)
        {
            if (value is int || value is long)
            {
                return value.ToString();
            }
            else
            {
                string str = value == null ? "null" : value.ToString();
                str = str.Replace("\'", "*").Replace("\"", "\\\"");
                str = "'" + str + "'";
                return str;
            }
        }

        private static string GetComparisonOperatorString(ComparisonOperator op)
        {
            switch (op)
            {
                case ComparisonOperator.Greater:
                    return ">";
                case ComparisonOperator.GreaterOrEqual:
                    return ">=";
                case ComparisonOperator.Less:
                    return "<";
                case ComparisonOperator.LessOrEqual:
                    return "<=";
                default:
                    return "=";
            }
        }

        private static string BuildFieldsString(IList<string> fields, IList<ExpandField> expandFields)
        {
            if (fields != null && fields.Count > 0)
            {
                if (expandFields != null)
                {
                    //replace field by  expansion
                    foreach (ExpandField expand in expandFields)
                    {
                       int i =  fields.IndexOf(expand.Field);
                        if (i != -1)
                        {
                            fields[i] = expand.Field + "{" + string.Join("," , expand.ExpandFields) + "}";
                        }
                    }
                }
                return "fields=" + string.Join(",", fields);
            }

            return null;
        }

        private static string BuildOrderByString(string orderBy)
        {
            if (!string.IsNullOrEmpty(orderBy))
            {
                return "order_by=" + orderBy;
            }
            return null;

        }

        private static string BuildOffsetString(int? offset)
        {
            if (offset != null && offset.HasValue)
            {
                return "offset=" + offset;
            }
            return null;

        }

        private static string BuildLimitString(int? limit)
        {
            if (limit != null && limit.HasValue)
            {
                return "limit=" + limit;
            }
            return null;
        }

        private static string BuildServiceArguments(Dictionary<string, string> serviceArguments)
        {
            if (serviceArguments == null || serviceArguments.Count == 0)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var item in serviceArguments)
            {
                sb.Append(item.Key);
                sb.Append("=");
                sb.Append(item.Value);
            }
            return sb.ToString();
        }

        private static string BuildGroupByString(string groupBy)
        {
            //group_by=severity
            if (!string.IsNullOrEmpty(groupBy))
            {
                return "group_by=" + groupBy;
            }
            return null;
        }

        internal static string BuildQueryString(IList<QueryPhrase> phrases)
        {
            if (phrases == null || phrases.Count == 0)
            {
                return string.Empty;
            }


            //query="id>100;status='open';(rank>10||rank<20)"
            List<string> phraseStrings = new List<string>();
            foreach (QueryPhrase phrase in phrases)
            {
                string phraseString = BuildPhraseString(phrase);
                phraseStrings.Add(phraseString);

            }
            string join = string.Join(";", phraseStrings);
            string output = string.Format("query=\"{0}\"", join);
            return output;
        }

        private static string ConcatNewQueryString(string baseString, string newString)
        {
            if (!string.IsNullOrEmpty(newString))
            {
                if (!string.IsNullOrEmpty(newString))
                {
                    baseString += "&";
                }
                baseString += newString;
                return baseString;
            }
            return baseString;
        }
    }
}
