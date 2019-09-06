using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace produce.Models
{
    class Feed
    {
        public string id;
        public string Type;
        public string LastBuildDate;

        public async Task Persist()
        {
            await DocumentDBRepository<Feed>.UpdateItemAsync(id, this);
        }

        public static async Task<Feed> GetFeedAsync()
        {
            var feedRecords = await DocumentDBRepository<Feed>.GetItemsAsync(d => d.Type == "channel");
            var feed = feedRecords.First();
            return feed;
        }

        public static async Task<string> EmitRssAsync()
        {
            var feed = await Feed.GetFeedAsync();

            // Create an XmlWriterSettings instance.
            XmlWriterSettings oSettings = new XmlWriterSettings();
            //oSettings.Indent = false;
            oSettings.OmitXmlDeclaration = true;
            //oSettings.Encoding = Encoding.UTF7;

            StringBuilder sb = new StringBuilder();

            // Create the XmlWriter object and write some content.
            //var w = new XmlTextWriter(filename, null);
            using (var writer = XmlTextWriter.Create(sb, oSettings))
            {
                //This define if it is a stand alone xml file or not
                writer.WriteStartDocument(true);

                writer.WriteStartElement("rss");
                writer.WriteAttributeString("xmlns", "itunes", null, "http://www.itunes.com/dtds/podcast-1.0.dtd");
                writer.WriteAttributeString("version", "2.0");
                writer.WriteAttributeString("xmlns", "atom", null, "http://www.w3.org/2005/Atom");

                writer.WriteStartElement("channel");

                writer.WriteStartElement("title");
                writer.WriteValue("Azure Flash News");
                writer.WriteEndElement();

                writer.WriteStartElement("link");
                writer.WriteValue("http://www.azureflashnews.com");
                writer.WriteEndElement();

                writer.WriteStartElement("atom", "link", null);
                writer.WriteAttributeString("href", "http://itunes-podcast.azureflashnews.com/feed.xml");
                writer.WriteAttributeString("rel", "self");
                writer.WriteAttributeString("type", "application/rss+xml");
                writer.WriteEndElement();

                writer.WriteStartElement("language");
                writer.WriteValue("en-us");
                writer.WriteEndElement();

                writer.WriteStartElement("copyright");
                writer.WriteValue("Rick Weyenberg and Mark Garner");
                writer.WriteEndElement();

                writer.WriteStartElement("itunes", "subtitle", null);
                writer.WriteValue("A show about the weekly news around Microsoft's Cloud Platform, Azure");
                writer.WriteEndElement();

                writer.WriteStartElement("itunes", "author", null);
                writer.WriteValue("Microsoft");
                writer.WriteEndElement();

                writer.WriteStartElement("itunes", "summary", null);
                writer.WriteValue("A weekly show featuring news items and features about Microsoft's Cloud Platform, Azure");
                writer.WriteEndElement();

                writer.WriteStartElement("description");
                writer.WriteValue("A weekly show featuring news items and features about Microsoft's Cloud Platform, Azure");
                writer.WriteEndElement();

                writer.WriteStartElement("itunes", "owner", null);
                writer.WriteStartElement("itunes", "name", null);
                writer.WriteValue("Rick Weyenberg and Mark Garner");
                writer.WriteEndElement();
                writer.WriteStartElement("itunes", "email", null);
                writer.WriteValue("mgarner@outlook.com");
                writer.WriteEndElement();
                writer.WriteEndElement(); //itnes:owner

                writer.WriteStartElement("itunes", "image", null);
                writer.WriteAttributeString("href", "http://affinvitestorage.blob.core.windows.net/aff/AFN-itunes.jpg");
                writer.WriteEndElement();

                writer.WriteStartElement("itunes", "category", null);
                writer.WriteAttributeString("text", "Technology");
                writer.WriteEndElement();

                writer.WriteStartElement("itunes", "keywords", null);
                writer.WriteValue("Microsoft, Azure, News, Microsoft Azure, Azure Flash News");
                writer.WriteEndElement();

                writer.WriteStartElement("itunes", "explicit", null);
                writer.WriteValue("no");
                writer.WriteEndElement();

                writer.WriteStartElement("lastBuildDate");
                var lastBuildDate = DateTime.Parse(feed.LastBuildDate);
                string padLastBuildDay = string.Empty;
                string padLastBuildHour = string.Empty;
                string padLastBuildMinute = string.Empty;
                if (lastBuildDate.Day < 10)
                {
                    padLastBuildDay = "0";
                }
                if (lastBuildDate.Hour < 10)
                {
                    padLastBuildHour = "0";
                }
                if (lastBuildDate.Minute < 10)
                {
                    padLastBuildMinute = "0";
                }
                writer.WriteValue(lastBuildDate.DayOfWeek.ToString().Substring(0, 3) + ", " + padLastBuildDay + lastBuildDate.Day.ToString() + " " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(lastBuildDate.Month).Substring(0, 3) + " " + lastBuildDate.Year.ToString() + " " + padLastBuildHour + lastBuildDate.Hour.ToString() + ":" + padLastBuildMinute + lastBuildDate.Minute.ToString() + ":00 GMT");
                //Fri, 09 Feb 2018 10:00:00 CST
                writer.WriteEndElement();

                var items = await Item.GetAllItemsAsync();

                foreach (Item item in items)
                {
                    writer.WriteStartElement("item");

                    writer.WriteStartElement("title");
                    writer.WriteValue(item.Title);
                    writer.WriteEndElement();

                    writer.WriteStartElement("itunes", "episode", null);
                    writer.WriteValue(item.iTunesEpisodeNumber);
                    writer.WriteEndElement();
                    
                    writer.WriteStartElement("itunes", "author", null);
                    writer.WriteValue("Rick Weyenberg and Mark Garner");
                    writer.WriteEndElement();

                    writer.WriteStartElement("itunes", "subtitle", null);
                    writer.WriteValue(item.iTunesSubtitle);
                    writer.WriteEndElement();

                    writer.WriteStartElement("itunes", "summary", null);
                    writer.WriteValue(item.iTunesSummary.Replace("\n", " "));
                    writer.WriteEndElement();

                    writer.WriteStartElement("itunes", "image", null);
                    writer.WriteAttributeString("href", "http://affinvitestorage.blob.core.windows.net/aff/AFN-itunes.jpg");
                    writer.WriteEndElement();

                    writer.WriteStartElement("enclosure");
                    writer.WriteAttributeString("type", item.EnclosureType);
                    writer.WriteAttributeString("url", item.EnclosureURL);
                    writer.WriteAttributeString("length", item.EnclosureLength);
                    writer.WriteEndElement();

                    writer.WriteStartElement("guid");
                    writer.WriteValue(item.GUID);
                    writer.WriteEndElement();

                    writer.WriteStartElement("pubDate");
                    var pubDate = DateTime.Parse(item.PubDate);
                    string padPubDay = string.Empty;
                    string padPubHour = string.Empty;
                    string padPubMinute = string.Empty;

                    if (pubDate.Day < 10)
                    {
                        padPubDay = "0";
                    }
                    if (pubDate.Hour < 10)
                    {
                        padPubHour = "0";
                    }
                    if (pubDate.Minute < 10)
                    {
                        padPubMinute = "0";
                    }
                    writer.WriteValue(pubDate.DayOfWeek.ToString().Substring(0, 3) + ", " + padPubDay + pubDate.Day.ToString() + " " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(pubDate.Month).Substring(0, 3) + " " + pubDate.Year.ToString() + " "+ padPubHour + pubDate.Hour.ToString() + ":" + pubDate.Minute.ToString() + ":00 GMT");
                    writer.WriteEndElement();

                    writer.WriteStartElement("itunes", "duration", null);
                    writer.WriteValue(item.iTunesDuration);
                    writer.WriteEndElement();

                    writer.WriteStartElement("itunes", "explicit", null);
                    writer.WriteValue(item.iTunesExplicit);
                    writer.WriteEndElement();

                    //item
                    writer.WriteEndElement();

                }

                //channel
                writer.WriteEndElement();
                //rss element
                writer.WriteEndElement();

                writer.WriteEndDocument();
                writer.Flush();
            }


            return sb.ToString();
        }
    }
}
