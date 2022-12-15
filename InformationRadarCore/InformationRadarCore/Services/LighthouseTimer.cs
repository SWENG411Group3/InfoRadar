using InformationRadarCore.Data;
using Microsoft.EntityFrameworkCore;

namespace InformationRadarCore.Services
{
    public class LighthouseTimer : BackgroundService
    {
        private readonly IServiceProvider services;
        private readonly IRunQueue exec;
        private readonly ILogger<LighthouseTimer> logger;
        private readonly int DELAY;
        public LighthouseTimer(IServiceProvider services, ConfigService config, IRunQueue exec, ILogger<LighthouseTimer> logger)
        {
            this.services = services;
            this.exec = exec;
            this.logger = logger;
            DELAY = config.TimerMsDelay ?? 60_000;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var tick = DateTime.Now;
                logger.LogInformation("Starting lighthouse exeuction cycle at " + tick);

                using (var scope = services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    await GatherVisitors(db, tick, stoppingToken);
                    await GatherMessengers(db, tick, stoppingToken);
                }

                await Task.Delay(DELAY, stoppingToken);
            }
        }

        private async Task GatherVisitors(ApplicationDbContext db, DateTime now, CancellationToken stoppingToken)
        {
            var overdue = await db.Lighthouses
                .Where(l => 
                    l.Enabled && !l.HasError && 
                    (l.LastVisitorRun == null || EF.Functions.DateDiffSecond(l.LastVisitorRun.Value, now) > (int)l.Frequency)
                )
                .Select(lighthouse => new
                {
                    lighthouse.Id,
                    RunSearch = lighthouse.GoogleQueries.Any(),
                })
                .ToListAsync(stoppingToken);

            foreach (var lighthouse in overdue)
            {
                exec.StartVisitor(lighthouse.Id, lighthouse.RunSearch);
            }
        }

        private async Task GatherMessengers(ApplicationDbContext db, DateTime now, CancellationToken stoppingToken)
        {
            var overdue = await db.Lighthouses
                .Where(l =>
                    l.Enabled && !l.HasError &&
                    l.LastSentMessage == null || EF.Functions.DateDiffSecond(l.LastSentMessage.Value, now) > (int)(l.MessengerFrequency ?? l.Frequency)
                )
                .Select(l => l.Id)
                .ToListAsync(stoppingToken);

            foreach (var lighthouse in overdue)
            {
                exec.StartMessenger(lighthouse);
            }
        }
    }
}
