using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Util.Data
{
    /// <summary>
    /// Summary.
    /// </summary>
    public static partial class DbConnectionExtensions
    {
        public static DbDataReader ExecuteReader(
            this DbConnection connection,
            string query,
            IEnumerable<IDbDataParameter> parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteReader(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior,
                null);
        }

        public static DbDataReader ExecuteReader(
            this DbConnection connection,
            string query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteReader(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior,
                null);
        }

        public static DbDataReader ExecuteReader(
            this DbConnection connection,
            string query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteReader(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior,
                null);
        }

        public static DbDataReader ExecuteReader(
            this DbConnection connection,
            StringBuilder query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteReader(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior,
                null);
        }

        public static DbDataReader ExecuteReader(
            this DbConnection connection,
            StringBuilder query,
            object[] parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.CloseConnection,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix)
        {
            return ExecuteReader(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior,
                null);
        }

        public static DbDataReader ExecuteReader(
            this DbConnection connection,
            Action<DbCommand> configure,
            CommandBehavior behavior = CommandBehavior.CloseConnection,
            DbTransaction transaction = null)
        {
            Check.NotNull(nameof(connection), connection);
            Check.NotNull(nameof(configure), configure);

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

                return cmd.ExecuteReader(behavior);
            }
            catch
            {
                transaction?.Rollback();
                throw;
            }
            finally
            {
                if (ownConnection)
                    connection?.Close();
            }
        }

        public static Task<DbDataReader> ExecuteReaderAsync(
            this DbConnection connection,
            string query,
            IEnumerable<IDbDataParameter> parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteReaderAsync(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior,
                null,
                cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(
            this DbConnection connection,
            string query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteReaderAsync(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior,
                null,
                cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(
            this DbConnection connection,
            string query,
            IReadOnlyList<object> parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteReaderAsync(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior,
                null,
                cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(
            this DbConnection connection,
            StringBuilder query,
            IEnumerable<IDbDataParameter> parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteReaderAsync(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior,
                null,
                cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(
            this DbConnection connection,
            StringBuilder query,
            IEnumerable<KeyValuePair<string, object>> parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteReaderAsync(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior,
                null,
                cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(
            this DbConnection connection,
            StringBuilder query,
            IReadOnlyList<object> parameters,
            CommandType commandType = CommandType.Text,
            CommandBehavior behavior = CommandBehavior.Default,
            char parameterPrefix = DbCommandExtensions.DefaultPrefix,
            CancellationToken cancellationToken = default)
        {
            return ExecuteReaderAsync(
                connection,
                (cmd) =>
                {
                    cmd.AddParameters(query, parameters, parameterPrefix);
                    cmd.CommandType = commandType;
                },
                behavior,
                null,
                cancellationToken);
        }

        public static async Task<DbDataReader> ExecuteReaderAsync(
           this DbConnection connection,
           Action<DbCommand> configure,
           CommandBehavior behavior = CommandBehavior.Default,
           DbTransaction transaction = null,
           CancellationToken cancellationToken = default)
        {
            Check.NotNull(nameof(connection), connection);
            Check.NotNull(nameof(configure), configure);

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

                return await cmd.ExecuteReaderAsync(behavior, cancellationToken)
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
                if (ownConnection)
                    connection?.Close();
            }
        }
    }
}