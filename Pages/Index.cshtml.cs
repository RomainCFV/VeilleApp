using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using VeilleApp.Context;
using VeilleApp.Model;

namespace VeilleApp.Pages;

public class IndexModel : PageModel
{
    private readonly VeilleContext _context;
    public IList<Veille> Veilles = new List<Veille>();
    [BindProperty]
    public string SearchQuery { get; set; }  = string.Empty;

    public IndexModel(VeilleContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
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

        List<Veille> NewEntries = RSSFeed.RSSFeed.FindAllArticles(RSS_Feeds);
        var sql = @"
            INSERT INTO veilles (""Title"", ""Content"", ""Link"", ""Publisher"", ""PublishTime"", ""Guid"")
            VALUES (@Title, @Content, @Link, @Publisher, @PublishTime, @Guid)
            ON CONFLICT (""Guid"") DO NOTHING;
        ";

        foreach (Veille entry in NewEntries)
        {
            await _context.Database.ExecuteSqlRawAsync(sql,
                    new NpgsqlParameter("@Title", entry.Title),
                    new NpgsqlParameter("@Content", entry.Content),
                    new NpgsqlParameter("@Link", entry.Link),
                    new NpgsqlParameter("@Publisher", entry.Publisher),
                    new NpgsqlParameter("@PublishTime", entry.PublishTime),
                    new NpgsqlParameter("@Guid", entry.Guid));
        }

        await _context.SaveChangesAsync();

        Veilles = await _context.veilles
                        .OrderByDescending(v => v.PublishTime)
                        .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!string.IsNullOrEmpty(SearchQuery))
        {
            SearchQuery = SearchQuery.ToLower();
            Veilles = await _context.veilles.Where(v => v.Title.ToLower().Contains(SearchQuery) ||
                                    v.Content.ToLower().Contains(SearchQuery) ||
                                    v.Publisher.ToLower().Contains(SearchQuery))
                                    .OrderByDescending(v => v.PublishTime)
                                    .ToListAsync();
        }
        else
        {
            Veilles = await _context.veilles
                            .OrderByDescending(v => v.PublishTime)
                            .ToListAsync();
        }
        return Page();
    }
}
