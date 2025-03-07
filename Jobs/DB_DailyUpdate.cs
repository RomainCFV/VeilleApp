using Microsoft.EntityFrameworkCore;
using VeilleApp.Context;
using VeilleApp.Model;
using VeilleApp.RSSFeed;
using Npgsql;
using Microsoft.IdentityModel.Tokens;

public class DailyJobService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public DailyJobService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var delayTime = TimeSpan.FromMinutes(1);
            await Task.Delay(delayTime, stoppingToken);

            await UpdateDatabaseJob();
        }
    }

    private async Task UpdateDatabaseJob()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<VeilleContext>();

            Dictionary<string, string> RSS_Feeds = new Dictionary<string, string>
            {
                { "Kaspersky", "https://www.kaspersky.com/blog/feed/" },
                { "ZDNet", "https://www.zdnet.com/topic/security/rss.xml" },
                { "SecurityAffairs", "https://securityaffairs.com/feed" },
                { "Bleeping Computer", "https://www.bleepingcomputer.com/feed/" },
                { "CERT-FR", "https://www.cert.ssi.gouv.fr/feed/" },
                { "Krebs on Security", "https://krebsonsecurity.com/feed/" },            
                { "ZATAZ", "https://www.zataz.com/feed/" },
                { "L'Usine Digitale", "https://www.usine-digitale.fr/rss" }
            };
            
            List<string> CurrentEntries = await dbContext.veilles.Select(v => v.Guid).ToListAsync();
            List<Veille> NewEntries = await RSSFeed.FindAllArticles(RSS_Feeds, CurrentEntries);

            if (!NewEntries.IsNullOrEmpty())
            {
                await dbContext.veilles.AddRangeAsync(NewEntries);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}