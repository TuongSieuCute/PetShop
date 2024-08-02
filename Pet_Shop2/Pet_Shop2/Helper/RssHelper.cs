using Pet_Shop2.Models;
using System.Xml.XPath;

namespace Pet_Shop2.Helper
{
    public class RssHelper
    {
        public static List<RssItem> Read(string url)
        {
            var rssItems = new List<RssItem>();
            try
            {
                var docs = new XPathDocument(url);
                XPathNavigator navigator = docs.CreateNavigator();
                XPathNodeIterator nodes = navigator.Select("//item");

                while (nodes.MoveNext())
                {
                    XPathNavigator currentNode = nodes.Current;

                    Console.WriteLine();

                    rssItems.Add(new RssItem
                    {
                        Title = currentNode.SelectSingleNode("title").Value,
                        Description = currentNode.SelectSingleNode("description").Value,
                        PubDate = currentNode.SelectSingleNode("pubDate").Value,
                        Link = currentNode.SelectSingleNode("link").Value,
                        Guid = currentNode.SelectSingleNode("guid").Value,
                        ImageUrl = currentNode.SelectSingleNode("enclosure")?.GetAttribute("url", "")
                    });
                }
            }
            catch
            {
                rssItems = null;
            }

            return rssItems;
        }
    }
}
