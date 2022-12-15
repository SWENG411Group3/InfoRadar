using InformationRadarCore.Models;

namespace InformationRadarCore.Services
{
    public interface IRunQueue
    {
        bool StartVisitor(int lighthouse, bool runSearch);
        bool StartMessenger(int lighthouse);
    }
}
