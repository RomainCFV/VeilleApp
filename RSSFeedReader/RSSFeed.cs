using VeilleApp.Model;
using System.Xml;
using System.ServiceModel.Syndication;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;

namespace VeilleApp.RSSFeed
{
    
    public class RSSFeed
    {
        public async static Task<List<Veille>> FindAllArticles(Dictionary<string, string> RSS_Feeds, List<string> CurrentEntries)
        {
            List<Veille> NewEntries = new List<Veille>();

            foreach (var rss in RSS_Feeds)
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    try
                    {
                        HttpResponseMessage response = await httpClient.GetAsync(rss.Value);
                        response.EnsureSuccessStatusCode();

                        using (XmlReader reader = XmlReader.Create(await response.Content.ReadAsStreamAsync()))
                        {
                            SyndicationFeed feed = SyndicationFeed.Load(reader);

                            foreach (SyndicationItem article in feed.Items)
                            {
                                if (CurrentEntries.IsNullOrEmpty() || !CurrentEntries.Contains(article.Id))
                                {
                                    NewEntries.Add(new Veille {
                                        Guid = article.Id,
                                        Title = article.Title.Text,
                                        Link = article.Links.First().Uri.ToString(),
                                        Content = article.Summary.Text.Replace(" [&#8230;]", "").Replace(" [...]", "").Replace("&#8217;", "'").Replace("&#38;", "&").Replace("&#160;", " ").Replace("&#8220;", "\"").Replace("&#8221;", "\""),
                                        Publisher = rss.Key,
                                        PublishTime = article.PublishDate.DateTime
                                        });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error fetching or parsing RSS feed for {rss.Key} : {ex.Message}");
                    }
                }
            }

            return NewEntries;
        }
    }
}
