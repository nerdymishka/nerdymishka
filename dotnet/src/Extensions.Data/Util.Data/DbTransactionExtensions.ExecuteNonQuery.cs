using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Util.Data
{
    /// <summary>
    /// Extensions methods for <see cref="DbTransaction" />.
    /// </summary>
    [SuppressMessage("", "SA1601: Partial elements should be documented", Justification = "Documentation is supplied")]
    public static partial class DbTransactionExtensions
    {
        public static int ExecuteNonQueryAsync(
            this DbTransaction transaction,
            string query,
            IEnumerable<IDbDataParameter> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteNonQuery(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                true);
        }

        public static int ExecuteNonQuery(
            this DbTransaction transaction,
            string query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteNonQuery(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                true);
        }

        public static int ExecuteNonQuery(
            this DbTransaction transaction,
            string query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteNonQuery(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                true);
        }

        public static int ExecuteNonQuery(
            this DbTransaction transaction,
            StringBuilder query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteNonQuery(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                true);
        }

        public static int ExecuteNonQuery(
            this DbTransaction transaction,
            StringBuilder query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteNonQuery(
                transaction,
                (cmd) =>
                {
                    cmd.Transaction = transaction;
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                true);
        }

        public static int ExecuteNonQuery(
          this DbTransaction transaction,
          Action<DbCommand> configure,
          bool autoClose = true)
        {
            Check.NotNull(nameof(transaction), transaction);
            Check.NotNull(nameof(configure), configure);

            return transaction.Connection
                .ExecuteNonQuery(configure, transaction, autoClose);
        }

        public static Task<int> ExecuteNonQueryAsync(
            this DbTransaction transaction,
            string query,
            IEnumerable<IDbDataParameter> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteNonQueryAsync(
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

        public static Task<int> ExecuteNonQueryAsync(
            this DbTransaction transaction,
            string query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteNonQueryAsync(
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

        public static Task<int> ExecuteNonQueryAsync(
            this DbTransaction transaction,
            string query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteNonQueryAsync(
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

        public static Task<int> ExecuteNonQueryAsync(
            this DbTransaction transaction,
            StringBuilder query,
            IEnumerable<IDbDataParameter> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteNonQueryAsync(
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

        public static Task<int> ExecuteNonQueryAsync(
            this DbTransaction transaction,
            StringBuilder query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteNonQueryAsync(
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

        public static Task<int> ExecuteNonQueryAsync(
            this DbTransaction transaction,
            StringBuilder query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteNonQueryAsync(
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

        public static Task<int> ExecuteNonQueryAsync(
            this DbTransaction transaction,
            Action<DbCommand> configure,
            bool autoClose = true,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(nameof(transaction), transaction);
            Check.NotNull(nameof(configure), configure);

            return transaction.Connection.ExecuteNonQueryAsync(
                configure, transaction, autoClose, cancellationToken);
        }
    }
}