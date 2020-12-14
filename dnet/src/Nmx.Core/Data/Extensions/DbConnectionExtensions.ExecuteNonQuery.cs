using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Data.Extensions
{
    public static partial class DbConnectionExtensions
    {
        public static int ExecuteNonQuery(
            this DbConnection connection,
            string query,
            IEnumerable<IDbDataParameter> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteNonQuery(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                null,
                true);
        }

        public static int ExecuteNonQuery(
            this DbConnection connection,
            string query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteNonQuery(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                null,
                true);
        }

        public static int ExecuteNonQuery(
            this DbConnection connection,
            string query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteNonQuery(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                null,
                true);
        }

        public static int ExecuteNonQuery(
            this DbConnection connection,
            StringBuilder query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteNonQuery(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                null,
                true);
        }

        public static int ExecuteNonQuery(
            this DbConnection connection,
            StringBuilder query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteNonQuery(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                null,
                true);
        }

        public static int ExecuteNonQuery(
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

                return cmd.ExecuteNonQuery();
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

        public static Task<int> ExecuteNonQueryAsync(
            this DbConnection connection,
            string query,
            IEnumerable<IDbDataParameter> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteNonQueryAsync(
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

        public static Task<int> ExecuteNonQueryAsync(
            this DbConnection connection,
            string query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteNonQueryAsync(
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

        public static Task<int> ExecuteNonQueryAsync(
            this DbConnection connection,
            string query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteNonQueryAsync(
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

        public static Task<int> ExecuteNonQueryAsync(
            this DbConnection connection,
            StringBuilder query,
            IEnumerable<IDbDataParameter> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteNonQueryAsync(
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

        public static Task<int> ExecuteNonQueryAsync(
            this DbConnection connection,
            StringBuilder query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteNonQueryAsync(
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

        public static Task<int> ExecuteNonQueryAsync(
            this DbConnection connection,
            StringBuilder query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteNonQueryAsync(
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

        public static async Task<int> ExecuteNonQueryAsync(
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

                return await cmd.ExecuteNonQueryAsync(cancellationToken)
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