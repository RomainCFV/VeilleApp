using VeilleApp.Model;
using System.Xml;
using System.ServiceModel.Syndication;
using Microsoft.IdentityModel.Tokens;

namespace VeilleApp.RSSFeed
{
    
    public class RSSFeed
    {
        public static List<Veille> FindAllArticles(Dictionary<string, string> RSS_Feeds, List<string> CurrentEntries)
        {
            List<Veille> NewEntries = new List<Veille>();
            foreach (var rss in RSS_Feeds)
            {
                XmlReader reader = XmlReader.Create(rss.Value);
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                reader.Close();
                
                foreach (SyndicationItem article in feed.Items)
                {
                    if (CurrentEntries.IsNullOrEmpty() || !CurrentEntries.Contains(article.Id))
                    {
                        NewEntries.Add(new Veille{
                            Guid = article.Id,
                            Title = article.Title.Text,
                            Link = article.Links.First().Uri.ToString(),
                            Content = article.Summary.Text.Replace(" [&#8230;]", "").Replace(" [...]", ""),
                            Publisher = rss.Key,
                            PublishTime = article.PublishDate.DateTime
                            });
                    }
                }
            }

            return NewEntries;
        }
    }
}