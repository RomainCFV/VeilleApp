using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VeilleApp.Context;
using VeilleApp.Model;

namespace VeilleApp.Pages;

public class IndexModel : PageModel
{
    private readonly VeilleContext _context;
    private readonly int MaxArticles = 600;
    public IList<Veille> Veilles = new List<Veille>();
    [BindProperty]
    public string SearchQuery { get; set; }  = string.Empty;

    public IndexModel(VeilleContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        Veilles = await _context.veilles
                        .OrderByDescending(v => v.PublishTime)
                        .Take(MaxArticles)
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
                                    .Take(MaxArticles)
                                    .ToListAsync();
        }
        else
        {
            Veilles = await _context.veilles
                            .OrderByDescending(v => v.PublishTime)
                            .Take(MaxArticles)
                            .ToListAsync();
        }
        return Page();
    }
}
