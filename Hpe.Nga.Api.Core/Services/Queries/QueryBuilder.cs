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
    /// Builder of query string for rest request , like this : query="id>1001"&order-by=end_date&fields=id,name,end_date&offset=1&limit=3
    /// </summary>
    internal static class QueryStringBuilder
    {

        private static String BuildPhraseString(QueryPhrase phrase)
        {
            String output = null;
            if (phrase is LogicalQueryPhrase)
            {
                LogicalQueryPhrase logicalPhrase = (LogicalQueryPhrase)phrase;

                List<String> expStrings = new List<string>();
                foreach (QueryExpression exp in logicalPhrase.Expressions)
                {
                    String comparisonOperator = GetComparisonOperatorString(exp.Operator);
                    String valueStr = GetExpressionValueString(exp.Value);
                    String expStr = String.Format("{0}{1}{2}", logicalPhrase.FieldName, comparisonOperator, valueStr);
                    expStrings.Add(expStr);
                }

                output = String.Join("||", expStrings);
                if (expStrings.Count > 1)
                {
                    output = "(" + output + ")";
                }
            }
            else if (phrase is CrossQueryPhrase)
            {
                //release={id=5002}
                CrossQueryPhrase crossPhrase = (CrossQueryPhrase)phrase;
                String expStr = String.Format("{0}={{{1}}}", crossPhrase.FieldName, BuildPhraseString(crossPhrase.QueryPhrase));
                output = expStr;
            }
            else if (phrase is NegativeQueryPhrase)
            {
                NegativeQueryPhrase negativePhrase = (NegativeQueryPhrase)phrase;
                String expStr = String.Format("!{0}", BuildPhraseString(negativePhrase.QueryPhrase));
                output = expStr;
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

        private static String GetExpressionValueString(object value)
        {
            if (value is int || value is long)
            {
                return value.ToString();
            }
            else
            {
                String str = value == null ? "null" : value.ToString();
                str = str.Replace("\'", "*").Replace("\"", "\\\"");
                str = "'" + str + "'";
                return str;
            }
        }

        private static String GetComparisonOperatorString(ComparisonOperator op)
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

        private static string BuildFieldsString(IList<string> fields)
        {
            if (fields != null && fields.Count > 0)
            {
                return "fields=" + String.Join(",", fields);
            }

            return null;
        }

        private static string BuildOrderByString(string orderBy)
        {
            if (!String.IsNullOrEmpty(orderBy))
            {
                return "order-by=" + orderBy;
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
            if (!String.IsNullOrEmpty(groupBy))
            {
                return "group_by=" + groupBy;
            }
            return null;
        }

        internal static string BuildQueryString(IList<QueryPhrase> phrases)
        {
            if (phrases == null || phrases.Count == 0)
            {
                return String.Empty;
            }


            //query="id>100;status='open';(rank>10||rank<20)"
            List<String> phraseStrings = new List<string>();
            foreach (QueryPhrase phrase in phrases)
            {
                String phraseString = BuildPhraseString(phrase);
                phraseStrings.Add(phraseString);

            }
            String join = String.Join(";", phraseStrings);
            String output = String.Format("query=\"{0}\"", join);
            return output;
        }

        /// <summary>
        /// build full query string like this : query="id>1001"&order-by=end_date&fields=id,name,end_date&offset=1&limit=3
        /// </summary>
        /// <param name="queryPhrases"></param>
        /// <param name="orderBy"></param>
        /// <param name="fields"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static string BuildQueryString(IList<QueryPhrase> queryPhrases, IList<String> fields, String orderBy, int? offset, int? limit,
            String groupBy, Dictionary<String,String> serviceArguments)
        {
            String str = String.Empty;
            str = ConcatNewQueryString(str, QueryStringBuilder.BuildGroupByString(groupBy));
            str = ConcatNewQueryString(str, QueryStringBuilder.BuildQueryString(queryPhrases));
            str = ConcatNewQueryString(str, QueryStringBuilder.BuildOrderByString(orderBy));
            str = ConcatNewQueryString(str, QueryStringBuilder.BuildFieldsString(fields));
            str = ConcatNewQueryString(str, QueryStringBuilder.BuildOffsetString(offset));
            str = ConcatNewQueryString(str, QueryStringBuilder.BuildLimitString(limit));
            str = ConcatNewQueryString(str, QueryStringBuilder.BuildServiceArguments(serviceArguments));
            return str;
        }

        private static String ConcatNewQueryString(String baseString, String newString)
        {
            if (!String.IsNullOrEmpty(newString))
            {
                if (!String.IsNullOrEmpty(newString))
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
