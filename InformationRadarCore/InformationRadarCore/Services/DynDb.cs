using InformationRadarCore.Data;
using InformationRadarCore.Models.Web;
using InformationRadarCore.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Query;
using System.Data.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace InformationRadarCore.Services
{
    // Database helper that deals with any dynamically generated SQL tables
    // Used for creating, deleting, and reading records from lighthouses
    // Any "raw" SQL should be in this class
    public class DynDb : IDynDb
    {
        private readonly ApplicationDbContext db;
        public DynDb(ApplicationDbContext db)
        {
            this.db = db;
        }

        // It is absolutely CRUCIAL that the parameters are sanitized before this method is called
        // There isn't a way to bind table names and columns in perpared statements, so their values
        // have to be "manually" vetted
        async public Task CreateLighthouseTable(string name, IDictionary<string, ILighthouseColumnType> cols)
        {
            // Internal name is validated via regex annotation
            var query = $"CREATE TABLE {name}_Lighthouse (" +
                "Id INT PRIMARY KEY IDENTITY, " +
                "Created DATETIME2 NOT NULL,";

            foreach (var (key, value) in cols)
            {
                query += $"Field_{key} {value.SqlColumn()},";
            }

            query += ");";

            await db.Database.ExecuteSqlRawAsync(query);
        }

        public Task DropLighthouseTable(Lighthouse lighthouse)
        {
            return db.Database.ExecuteSqlRawAsync($"DROP TABLE IF EXISTS {lighthouse.InternalName}_Lighthouse;");
        }

        public async Task<GenericRecordTable> LighthouseRecords(Lighthouse lighthouse, int pageSize, int? before = null)
        {
            var table = new GenericRecordTable();

            var connection = db.Database.GetDbConnection();
            var wasClosed = connection.State != ConnectionState.Open;

            // We need raw connection access ADO.NET features
            if (wasClosed)
            {
                await connection.OpenAsync();
            }

            try
            {
                using (var command = connection.CreateCommand())
                {
                    var queryText = $"SELECT TOP (@PAGESIZE) * FROM {lighthouse.InternalName}_Lighthouse";
                    if (before.HasValue)
                    {
                        queryText += " WHERE Id < @BEFORE";
                    }
                    queryText += " ORDER BY Id DESC;";

                    command.CommandText = queryText;
                    command.CommandType = CommandType.Text;

                    command.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@PAGESIZE",
                        Value = pageSize,
                        SqlDbType = SqlDbType.Int,
                    });

                    if (before.HasValue)
                    {
                        command.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@BEFORE",
                            Value = before.Value,
                            SqlDbType = SqlDbType.Int,
                        });
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        table.Columns = reader.GetColumnSchema();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var id = await reader.GetFieldValueAsync<int>(0);
                                var created = await reader.GetFieldValueAsync<DateTime>(1);
                                var values = new List<object>();

                                for (int i = 2; i < reader.FieldCount; i++)
                                {
                                    values.Add(await reader.GetFieldValueAsync<object>(i));
                                }

                                table.Rows.Add(new GenericRecordTable.RecordRow()
                                {
                                    Id = id,
                                    Created = created,
                                    Values = values,
                                });
                            }
                        }
                    }
                }
            }
            finally 
            {
                // If we open the connection manually, we're also responsible for closing it unless the entire db context is disposed
                // https://learn.microsoft.com/en-us/ef/ef6/fundamentals/connection-management#behavior-in-ef6-and-future-versions-1
                if (wasClosed && connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }

            return table;
        }

        public async Task<int> CountRecords(Lighthouse lighthouse, int? cursor = null)
        {
            int count = 0;

            var connection = db.Database.GetDbConnection();
            var wasClosed = connection.State != ConnectionState.Open;

            if (wasClosed)
            {
                await connection.OpenAsync();
            }

            try
            {
                var query = $"SELECT COUNT(*) FROM {lighthouse.InternalName}_Lighthouse";
                if (cursor.HasValue)
                {
                    query += " WHERE Id < @CURSOR;";
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;

                    if (cursor.HasValue)
                    {
                        command.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@CURSOR",
                            Value = cursor.Value,
                            SqlDbType = SqlDbType.Int,
                        });
                    }

                    var result = (int?)(await command.ExecuteScalarAsync());
                    if (result.HasValue)
                    {
                        count = result.Value;
                    }

                }
            }
            finally
            {
                if (wasClosed && connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }

            return count;
        }
    }
}
