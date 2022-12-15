using InformationRadarCore.Models;
using System.Collections.Immutable;
using System.Diagnostics;

namespace InformationRadarCore.Services
{
    public class ScrapyInterface : IScrapyInterface
    {
        private readonly ConfigService config;

        public ScrapyInterface(ConfigService config)
        {
            this.config = config;
        }

        private Process BatchProcess(string scriptName, string args)
        {
            var dir = Path.Join(config.ResourceRoot, "Scraper");
            var batchFile = Path.Join(dir, "batch_scripts", scriptName + ".bat");

            var info = new ProcessStartInfo()
            {
                FileName = batchFile,
                Arguments = args,
                WorkingDirectory = dir,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
            };

            return new Process()
            { 
                StartInfo = info,
            };
        }

        public async Task<int> Messenger(int lighthouseId)
        {
            using (var process = BatchProcess("invoke_messenger", $"{config.AnacondaPath} {lighthouseId}"))
            {
                process.Start();
                await process.WaitForExitAsync();
                return process.ExitCode;
            }
        }

        public async Task<int> Visitor(int lighthouseId, bool runSearch)
        {
            using (var process = BatchProcess("invoke_lighthouse", $"{config.AnacondaPath} {lighthouseId} {(runSearch ? 1 : 0)}"))
            {
                process.Start();
                await process.WaitForExitAsync();
                return process.ExitCode;
            }
        }

        public int CreateEnv()
        {
            using (var process = BatchProcess("setup_conda_env", config.AnacondaPath))
            {
                process.Start();
                process.WaitForExit();
                return process.ExitCode;
            }
        }
    }
}
