using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Data.Extensions
{
    public static partial class DbTransactionExtensions
    {
        public static void Use(
           this DbTransaction transaction,
           Action<DbTransaction> execute)
        {
            Check.ArgNotNull(transaction, nameof(transaction));
            Check.ArgNotNull(execute, nameof(execute));

            bool ownConnection = false;
            var connection = transaction.Connection;
            try
            {
                if (connection.State.HasFlag(ConnectionState.Closed))
                {
                    ownConnection = true;
                    connection.Open();
                }

                execute(transaction);
                transaction.Commit();
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
                    connection.Close();
            }
        }

        public static async Task UseAsync(
           this DbTransaction transaction,
           Action<DbTransaction> execute,
           CancellationToken cancellationToken = default)
        {
            Check.ArgNotNull(transaction, nameof(transaction));

            var connection = transaction.Connection;
            bool ownConnection = false;
            try
            {
                if (connection.State.HasFlag(ConnectionState.Closed))
                {
                    ownConnection = true;
                    await connection.OpenAsync(cancellationToken)
                        .ConfigureAwait(false);
                }

                var task = new Task(() => execute(transaction));
                task.Start();
                await task.ConfigureAwait(false);
                transaction.Commit();
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
                    connection.Close();
            }
        }
    }
}