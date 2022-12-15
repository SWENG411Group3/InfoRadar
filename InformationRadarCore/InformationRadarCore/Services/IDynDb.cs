using InformationRadarCore.Models;
using InformationRadarCore.Models.Web;
using System.Data;

namespace InformationRadarCore.Services
{
    public interface IDynDb
    {
        Task CreateLighthouseTable(string name, IDictionary<string, ILighthouseColumnType> cols);
        Task DropLighthouseTable(Lighthouse lighthouse);
        Task<GenericRecordTable> LighthouseRecords(Lighthouse lighthouse, int pageSize, int? before = null);
        Task<int> CountRecords(Lighthouse lighthouse, int? cursor = null);
    }
}
