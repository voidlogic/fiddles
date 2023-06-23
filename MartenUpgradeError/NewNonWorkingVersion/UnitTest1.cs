using Common;
using Marten;
using Marten.Services;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NewNonWorkingVersion
{
    public class UnitTest1
    {
        [Fact]
        public async Task TestMethod1()
        {
            var connectionStringBase = "<REDACTED>";
            var dbName = "unittestnewversion";
            var adminDbName = "<REDACTED>";
            var docStore = GetDocumentStore(connectionStringBase, dbName);

            // kill db processes
            PostgresUtils.ExecuteNpgsqlCommandFromAdminDatabase($"SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = '{dbName}' AND pid <> pg_backend_pid();", connectionStringBase, adminDbName);
            // Recreate db
            PostgresUtils.ExecuteNpgsqlCommandFromAdminDatabase($"DROP DATABASE IF EXISTS {dbName};", connectionStringBase, adminDbName);
            PostgresUtils.ExecuteNpgsqlCommandFromAdminDatabase($"CREATE DATABASE {dbName};", connectionStringBase, adminDbName);

            using (var session = docStore.LightweightSession(new SessionOptions() { Timeout = 30 }))
            {
                // Only add testdata if there is none
                var query = session.Query<TestData>();
                var result = await query.ToListAsync();
                if (!result.Any())
                {
                    var testData1 = TestData.GetTestData(1);
                    session.Store(testData1);
                    await session.SaveChangesAsync();
                }
            }

            using (var session = docStore.OpenSession(new SessionOptions() { Timeout = 30 }))
            {
                // Query with linq for specific "innertestdataid"
                var query = session
                    .Query<TestData>()
                    .Where(td => td.InnerTestDatas.Any(itd => itd.InnerTestDataId.Equals("innertestdata_1")));
                var result = await query.ToListAsync();
                Assert.Equal(1, result.Count);
            }
        }

        private DocumentStore GetDocumentStore(string connectionStringBase, string dbName)
        {
            var serializer = new JsonNetSerializer();
            //var serializer = new JsonNetSerializer
            //{
            //    EnumStorage = EnumStorage.AsString,
            //    CollectionStorage = CollectionStorage.AsArray
            //};
            //var isoDateTimeConverter = new IsoDateTimeConverter();
            //isoDateTimeConverter.DateTimeFormat = "O";
            serializer.Customize(_ =>
            {
                //_.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //_.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
                //_.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
                //_.Converters.Add(isoDateTimeConverter);
            });
            var docStore = DocumentStore.For(_ =>
            {
                _.Policies.AllDocumentsAreMultiTenanted();
                _.Connection($"{connectionStringBase};Database={dbName}");
                //_.AutoCreateSchemaObjects = AutoCreate.All;
                //_.Serializer(serializer);
                //_.Schema.For<TestData>();
            });

            return docStore;
        }
    }
}