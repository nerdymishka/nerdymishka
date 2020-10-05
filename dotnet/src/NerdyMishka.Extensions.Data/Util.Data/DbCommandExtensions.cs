using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NerdyMishka.Util.Data
{
    public static class DbCommandExtensions
    {
        public const char DefaultPrefix = '@';

        public static IDbCommand AddParameter(
            this IDbCommand command,
            IDbDataParameter parameter)
        {
            Check.NotNull(nameof(command), command);

            command.Parameters.Add(parameter);
            return command;
        }

        /// <summary>
        /// Adds the parameter to the command..
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="dbType">The data type.</param>
        /// <param name="size">The parameter size.</param>
        /// <param name="direction">The parameter direction.</param>
        /// <returns>The data command.</returns>
        /// <exception cref="ArgumentNullException">command.</exception>
        public static DbCommand AddParameter(this DbCommand command,
            string name,
            object value,
            DbType? dbType = null,
            int? size = null,
            ParameterDirection? direction = null)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            var p = command.CreateParameter();
            p.ParameterName = name;
            p.Value = value;

            if (dbType.HasValue)
                p.DbType = dbType.Value;

            if (size.HasValue)
                p.Size = size.Value;

            if (direction.HasValue)
                p.Direction = direction.Value;

            command.AddParameter(p);
            return command;
        }

        public static IDbCommand AddParameter(
            this IDbCommand command,
            string name,
            object value,
            int precision,
            int? scale = null,
            DbType? dbType = null,
            ParameterDirection? direction = null)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            var p = command.CreateParameter();
            p.ParameterName = name;
            p.Value = value;

            if (dbType.HasValue)
                p.DbType = dbType.Value;

            p.Precision = (byte)precision;

            if (scale.HasValue)
                p.Scale = (byte)scale.Value;

            if (direction.HasValue)
                p.Direction = direction.Value;

            command.AddParameter(p);
            return command;
        }

        public static DbCommand AddParameters(
            this DbCommand cmd,
            StringBuilder query,
            IReadOnlyList<object> parameters,
            char parameterPrefix = '@',
            string placeholder = "[?]")
        {
            Check.NotNull(nameof(query), query);

            return AddParameters(cmd,
                query.ToString(),
                parameters,
                parameterPrefix,
                placeholder);
        }

        [SuppressMessage(
            "Microsoft.Security",
            "CA2100:ReviewSqlQueriesForSecurityVulnerabilities",
            Justification = "These queries assume you are using parameters.")]
        public static DbCommand AddParameters(
            this DbCommand cmd,
            string query,
            IReadOnlyList<object> parameters,
            char parameterPrefix = '@',
            string placeholder = "[?]")
        {
            if (cmd is null)
                throw new ArgumentNullException(nameof(cmd));

            if (query is null)
                throw new ArgumentNullException(nameof(query));

            if (parameterPrefix == char.MinValue)
                parameterPrefix = '@';

            var sql = query;
            if (parameters != null && parameters.Count > 0)
            {
                int index = 0;

                // TODO: write a parser
                sql = Regex.Replace(sql, placeholder, (m) =>
                {
                    var name = parameterPrefix + index.ToString(CultureInfo.InvariantCulture);
                    cmd.AddParameter(name, parameters[index]);
                    index++;

                    return name;
                });
            }

            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        public static DbCommand AddParameters(
            this DbCommand cmd,
            string query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            char parameterPrefix = '@')
        {
            return AddParameters(
                cmd,
                new StringBuilder(query),
                parameters,
                parameterPrefix);
        }

        [SuppressMessage(
            "Microsoft.Security",
            "CA2100:ReviewSqlQueriesForSecurityVulnerabilities",
            Justification = "These queries assume you are using parameters.")]
        public static DbCommand AddParameters(
            this DbCommand cmd,
            StringBuilder query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            char parameterPrefix = '@')
        {
            if (cmd is null)
                throw new ArgumentNullException(nameof(cmd));

            if (query is null)
                throw new ArgumentNullException(nameof(query));

            if (parameters != null)
            {
                bool replace = parameterPrefix != DefaultPrefix;
                bool hasPrefix = parameters.First().Key[0] == DefaultPrefix;

                foreach (var set in parameters)
                {
                    var key = set.Key;
                    var parameterName = parameterPrefix + key;
                    var value = set.Value;

                    if (!replace)
                    {
                        cmd.AddParameter(key, value);
                        continue;
                    }

                    if (hasPrefix)
                    {
                        parameterName = parameterPrefix + key.Substring(1);
                        query.Replace(key, parameterName);
                        cmd.AddParameter(parameterName, value);
                        continue;
                    }

                    query.Replace(DefaultPrefix + key, parameterName);
                    cmd.AddParameter(parameterName, value);
                }
            }

            cmd.CommandText = query.ToString();
            return cmd;
        }

        public static DbCommand AddParameters(
           this DbCommand cmd,
           string query,
           IEnumerable<IDbDataParameter> parameters,
           char parameterPrefix = DefaultPrefix)
        {
            return AddParameters(
                cmd,
                new StringBuilder(query),
                parameters,
                parameterPrefix);
        }

        [SuppressMessage(
            "Microsoft.Security",
            "CA2100:ReviewSqlQueriesForSecurityVulnerabilities",
            Justification = "These queries assume you are using parameters.")]
        public static DbCommand AddParameters(
            this DbCommand cmd,
            StringBuilder query,
            IEnumerable<IDbDataParameter> parameters,
            char parameterPrefix = DefaultPrefix)
        {
            if (cmd is null)
                throw new ArgumentNullException(nameof(cmd));

            if (query is null)
                throw new ArgumentNullException(nameof(query));

            if (parameters != null)
            {
                bool replace = parameterPrefix != DefaultPrefix;

                foreach (var p in parameters)
                {
                    if (replace && p.ParameterName[0] == DefaultPrefix)
                    {
                        var name = parameterPrefix + p.ParameterName.Substring(1);
                        query.Replace(p.ParameterName, name);
                        p.ParameterName = name;
                    }

                    cmd.AddParameter(p);
                }
            }

            cmd.CommandText = query.ToString();

            return cmd;
        }

        public static DbCommand AddParameters(
            this DbCommand cmd,
            string query,
            IDictionary parameters,
            char parameterPrefix = DefaultPrefix)
        {
            return AddParameters(
                cmd,
                new StringBuilder(query),
                parameters,
                parameterPrefix);
        }

        [SuppressMessage(
            "Microsoft.Security",
            "CA2100:ReviewSqlQueriesForSecurityVulnerabilities",
            Justification = "These queries assume you are using parameters.")]
        public static DbCommand AddParameters(
            this DbCommand cmd,
            StringBuilder query,
            IDictionary parameters,
            char parameterPrefix = DefaultPrefix)
        {
            if (cmd is null)
                throw new ArgumentNullException(nameof(cmd));

            if (query is null)
                throw new ArgumentNullException(nameof(query));

            if (parameters != null && parameters.Count > 0)
            {
                bool replace = parameterPrefix != DefaultPrefix;
                var enumerator = parameters.GetEnumerator();
                enumerator.MoveNext();
                var hasPrefix = enumerator.Key.ToString()[0] == DefaultPrefix;

                foreach (string key in parameters.Keys)
                {
                    var parameterName = parameterPrefix + key;
                    var value = parameters[key];

                    if (!replace)
                    {
                        cmd.AddParameter(parameterName, value);
                        continue;
                    }

                    if (hasPrefix)
                    {
                        parameterName = parameterPrefix + key.Substring(1);
                        query.Replace(key, parameterName);
                        cmd.AddParameter(parameterName, value);
                        continue;
                    }

                    query.Replace(DefaultPrefix + key, parameterName);
                    cmd.AddParameter(parameterName, value);
                }
            }

            cmd.CommandText = query.ToString();
            return cmd;
        }
    }
}