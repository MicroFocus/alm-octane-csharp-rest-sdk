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

                if (logicalPhrase.NegativeCondition)
                {
                    output = "!" + output;
                }
            }
            else if (phrase is CrossQueryPhrase)
            {
                //release={id=5002}
                CrossQueryPhrase crossPhrase = (CrossQueryPhrase)phrase;
                String expStr = String.Format("{0}={{{1}}}", crossPhrase.FieldName, BuildPhraseString(crossPhrase.QueryPhrase));
                output = expStr;
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
                String str = value.ToString();
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

        private static string BuildGroupByString(string groupBy)
        {
            //group_by=severity
            if (!String.IsNullOrEmpty(groupBy))
            {
                return "group_by=" + groupBy;
            }
            return null;
        }

        private static string BuildQueryString(IList<QueryPhrase> phrases)
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
        public static string BuildQueryString(IList<QueryPhrase> queryPhrases, IList<String> fields, String orderBy, int? offset, int? limit, String groupBy)
        {
            String str = String.Empty;
            str = ConcateNewQueryString(str, QueryStringBuilder.BuildGroupByString(groupBy));
            str = ConcateNewQueryString(str, QueryStringBuilder.BuildQueryString(queryPhrases));
            str = ConcateNewQueryString(str, QueryStringBuilder.BuildOrderByString(orderBy));
            str = ConcateNewQueryString(str, QueryStringBuilder.BuildFieldsString(fields));
            str = ConcateNewQueryString(str, QueryStringBuilder.BuildOffsetString(offset));
            str = ConcateNewQueryString(str, QueryStringBuilder.BuildLimitString(limit));            
            return str;
        }

        private static String ConcateNewQueryString(String baseString, String newString)
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
