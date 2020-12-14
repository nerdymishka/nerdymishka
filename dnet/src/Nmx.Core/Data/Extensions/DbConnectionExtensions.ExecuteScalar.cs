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
    public static partial class DbConnectionExtensions
    {
        public static object ExecuteScalar(
            this DbConnection connection,
            string query,
            IEnumerable<IDbDataParameter> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteScalar(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                null,
                true);
        }

        public static object ExecuteScalar(
            this DbConnection connection,
            string query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteScalar(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                null,
                true);
        }

        public static object ExecuteScalar(
            this DbConnection connection,
            string query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteScalar(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                null,
                true);
        }

        public static object ExecuteScalar(
            this DbConnection connection,
            StringBuilder query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteScalar(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                null,
                true);
        }

        public static object ExecuteScalar(
            this DbConnection connection,
            StringBuilder query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteScalar(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                null,
                true);
        }

        public static object ExecuteScalar(
          this DbConnection connection,
          Action<DbCommand> configure,
          DbTransaction transaction = null,
          bool autoClose = true)
        {
            Check.ArgNotNull(connection, nameof(connection));
            Check.ArgNotNull(configure, nameof(configure));

            bool ownConnection = false;
            try
            {
                if (connection.State.HasFlag(ConnectionState.Closed))
                {
                    ownConnection = true;
                    connection.Open();
                }

                var cmd = connection.CreateCommand();
                configure(cmd);

                return cmd.ExecuteScalar();
            }
            catch
            {
                transaction?.Rollback();
                connection?.Close();
                throw;
            }
            finally
            {
                if (autoClose)
                {
                    if (ownConnection)
                        connection?.Close();
                }
            }
        }

        public static Task<object> ExecuteScalarAsync(
            this DbConnection connection,
            string query,
            IEnumerable<IDbDataParameter> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteScalarAsync(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                null,
                true,
                cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(
            this DbConnection connection,
            string query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteScalarAsync(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                null,
                true,
                cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(
            this DbConnection connection,
            string query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteScalarAsync(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                null,
                true,
                cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(
            this DbConnection connection,
            StringBuilder query,
            IEnumerable<IDbDataParameter> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteScalarAsync(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                null,
                true,
                cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(
            this DbConnection connection,
            StringBuilder query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteScalarAsync(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                null,
                true,
                cancellationToken);
        }

        public static Task<object> ExecuteScalarAsync(
            this DbConnection connection,
            StringBuilder query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteScalarAsync(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                null,
                true,
                cancellationToken);
        }

        public static async Task<object> ExecuteScalarAsync(
            this DbConnection connection,
            Action<DbCommand> configure,
            DbTransaction transaction = null,
            bool autoClose = true,
            CancellationToken cancellationToken = default)
        {
            Check.ArgNotNull(connection, nameof(connection));
            Check.ArgNotNull(configure, nameof(configure));

            bool ownConnection = false;
            try
            {
                if (connection.State.HasFlag(ConnectionState.Closed))
                {
                    ownConnection = true;
                    await connection.OpenAsync(cancellationToken)
                        .ConfigureAwait(false);
                }

                var cmd = connection.CreateCommand();
                configure(cmd);

                return await cmd.ExecuteScalarAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch
            {
                transaction?.Rollback();
                connection?.Close();
                throw;
            }
            finally
            {
                if (autoClose)
                {
                    if (ownConnection)
                        connection?.Close();
                }
            }
        }
    }
}