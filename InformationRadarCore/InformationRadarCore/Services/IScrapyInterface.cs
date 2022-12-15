using InformationRadarCore.Models;

namespace InformationRadarCore.Services
{
    public interface IScrapyInterface
    {
        Task<int> Visitor(int lighthouseId, bool search);
        Task<int> Messenger(int lighthouseId);
        int CreateEnv();
    }
}
