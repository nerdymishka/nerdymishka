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
    /// Extensions methods for <see cref="DbConnection" />.
    /// </summary>
    [SuppressMessage("", "SA1601: Partial elements should be documented", Justification = "Documentation is supplied")]
    public static partial class DbConnectionExtensions
    {
        public static void Use(
           this DbConnection connection,
           Action<DbConnection> execute)
        {
            Check.NotNull(nameof(connection), connection);
            Check.NotNull(nameof(execute), execute);

            bool ownConnection = false;
            try
            {
                if (connection.State.HasFlag(ConnectionState.Closed))
                {
                    ownConnection = true;
                    connection.Open();
                }

                execute(connection);
            }
            catch
            {
                connection?.Close();
                throw;
            }
            finally
            {
                if (ownConnection)
                    connection.Close();
            }
        }

        public static async Task UseAsync(
           this DbConnection connection,
           Action<DbConnection> execute,
           CancellationToken cancellationToken = default)
        {
            Check.NotNull(nameof(connection), connection);
            bool ownConnection = false;
            try
            {
                if (connection.State.HasFlag(ConnectionState.Closed))
                {
                    ownConnection = true;
                    await connection.OpenAsync(cancellationToken)
                        .ConfigureAwait(false);
                }

                var task = new Task(() => execute(connection));
                task.Start();
                await task.ConfigureAwait(false);
            }
            catch
            {
                connection?.Close();
                throw;
            }
            finally
            {
                if (ownConnection)
                    connection.Close();
            }
        }
    }
}