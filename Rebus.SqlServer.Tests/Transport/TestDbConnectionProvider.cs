﻿using System;
using System.Threading.Tasks;
#if NET45
using System.Transactions;
#endif
using NUnit.Framework;
using Rebus.Logging;

namespace Rebus.SqlServer.Tests.Transport
{

    [TestFixture, Category(Categories.SqlServer)]
    public class TestDbConnectionProvider
    {
        [Test, Ignore("assumes existence of a bimse table")]
        public async Task CanDoWorkInTransaction()
        {
            var provizzle = new DbConnectionProvider(SqlTestHelper.ConnectionString, new ConsoleLoggerFactory(true));

            using (var dbConnection = await provizzle.GetConnection())
            {
                using (var cmd = dbConnection.CreateCommand())
                {
                    cmd.CommandText = "insert into bimse (text) values ('hej med dig')";
                    
                    await cmd.ExecuteNonQueryAsync();
                }

                //await dbConnection.Complete();
            }
        }
#if NET45
        [Test, Ignore("assumes existence of a bimse table")]
        public async Task CanDoWorkInAmbientTransaction()
        {
            using (var tx = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromSeconds(60)
            }))
            {
                var provizzle = new DbConnectionProvider(SqlTestHelper.ConnectionString, new ConsoleLoggerFactory(true), 
                    enlistInAmbientTransaction: true);

                using (var dbConnection = await provizzle.GetConnection())
                {
                    using (var cmd = dbConnection.CreateCommand())
                    {
                        cmd.CommandText = "insert into bimse (text) values ('Nogen fjellaper liger 2PC')";
                    
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                // tx.Complete();
            }
        }
#endif
    }
}