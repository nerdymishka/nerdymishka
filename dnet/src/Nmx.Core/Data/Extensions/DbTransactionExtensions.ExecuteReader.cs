using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Data.Extensions
{
    /// <summary>
    /// Summary.
    /// </summary>
    public static partial class DbTransactionExtensions
    {
        public static DbDataReader ExecuteReader(
            this DbTransaction transaction,
            string query,
            IReadOnlyList<IDbDataParameter> parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteReader(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior);
        }

        public static DbDataReader ExecuteReader(
            this DbTransaction transaction,
            string query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteReader(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior);
        }

        public static DbDataReader ExecuteReader(
            this DbTransaction transaction,
            string query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteReader(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior);
        }

        public static DbDataReader ExecuteReader(
            this DbTransaction transaction,
            StringBuilder query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteReader(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior);
        }

        public static DbDataReader ExecuteReader(
            this DbTransaction transaction,
            StringBuilder query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteReader(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior);
        }

        public static DbDataReader ExecuteReader(
            this DbTransaction transaction,
            Action<DbCommand> configure,
            CommandBehavior behavior = CommandBehavior.Default)
        {
            Check.ArgNotNull(transaction, nameof(transaction));
            Check.ArgNotNull(configure, nameof(configure));

            return transaction.Connection
                .ExecuteReader(configure, behavior, transaction);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(
            this DbTransaction transaction,
            string query,
            IEnumerable<IDbDataParameter> parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteReaderAsync(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior,
                cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(
            this DbTransaction transaction,
            string query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteReaderAsync(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior,
                cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(
            this DbTransaction transaction,
            string query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteReaderAsync(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior,
                cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(
            this DbTransaction transaction,
            StringBuilder query,
            IEnumerable<IDbDataParameter> parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteReaderAsync(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior,
                cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(
            this DbTransaction transaction,
            StringBuilder query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteReaderAsync(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior,
                cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(
            this DbTransaction transaction,
            StringBuilder query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteReaderAsync(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior,
                cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(
           this DbTransaction transaction,
           Action<DbCommand> configure,
           CommandBehavior behavior = CommandBehavior.Default,
           CancellationToken cancellationToken = default)
        {
            Check.ArgNotNull(transaction, nameof(transaction));
            Check.ArgNotNull(configure, nameof(configure));

            return transaction.Connection.ExecuteReaderAsync(
                configure,
                behavior,
                transaction,
                cancellationToken);
        }
    }
}