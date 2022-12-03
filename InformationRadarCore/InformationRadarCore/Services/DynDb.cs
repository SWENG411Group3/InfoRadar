using InformationRadarCore.Data;
using InformationRadarCore.Models.Web;
using InformationRadarCore.Models;
using Microsoft.EntityFrameworkCore;

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
                "Created DATETIME NOT NULL,";

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
    }
}
