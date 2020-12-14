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
        public static object ExecuteScalar(
            this DbTransaction transaction,
            string query,
            IEnumerable<IDbDataParameter> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteScalar(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                true);
        }

        public static object ExecuteScalar(
            this DbTransaction transaction,
            string query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteScalar(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                true);
        }

        public static object ExecuteScalar(
            this DbTransaction transaction,
            string query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteScalar(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                true);
        }

        public static object ExecuteScalar(
            this DbTransaction transaction,
            StringBuilder query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteScalar(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                true);
        }

        public static object ExecuteScalar(
            this DbTransaction transaction,
            StringBuilder query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteScalar(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                true);
        }

        public static object ExecuteScalar(
          this DbTransaction transaction,
          Action<DbCommand> configure,
          bool autoClose = true)
        {
            Check.ArgNotNull(transaction, nameof(transaction));
            Check.ArgNotNull(configure, nameof(configure));

            return transaction.Connection
                .ExecuteScalar(configure, transaction, autoClose);
        }

        public static Task<object> ExecuteScalarAsync(
            this DbTransaction transaction,
            string query,
            IEnumerable<IDbDataParameter> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteScalarAsync(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                true,
                cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(
            this DbTransaction transaction,
            string query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteScalarAsync(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                true,
                cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(
            this DbTransaction transaction,
            string query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteScalarAsync(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                true,
                cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(
            this DbTransaction transaction,
            StringBuilder query,
            IEnumerable<IDbDataParameter> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteScalarAsync(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                true,
                cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(
            this DbTransaction transaction,
            StringBuilder query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteScalarAsync(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                true,
                cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(
            this DbTransaction transaction,
            StringBuilder query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteScalarAsync(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                true,
                cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(
            this DbTransaction transaction,
            Action<DbCommand> configure,
            bool autoClose = true,
            CancellationToken cancellationToken = default)
        {
            Check.ArgNotNull(transaction, nameof(transaction));
            Check.ArgNotNull(configure, nameof(configure));

            return transaction.Connection
                .ExecuteScalarAsync(configure, transaction, autoClose, cancellationToken);
        }
    }
}