using InformationRadarCore.Models;
using InformationRadarCore.Models.Web;

namespace InformationRadarCore.Services
{
    public interface IDynDb
    {
        Task CreateLighthouseTable(string name, IDictionary<string, ILighthouseColumnType> cols);
        Task DropLighthouseTable(Lighthouse lighthouse);
    }
}
