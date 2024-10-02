using System.Xml;
using System.Collections.Generic;
namespace EMAds.Ads
{
    public class VASTAd
    {
        public string MediaFileUrl { get; set; }
        public string ClickThroughUrl { get; set; }
        // Other ad properties like impressions, trackers, etc.
        public string VastAdTagUrl { get; set; }
    }


    internal class VASTParser
    {
        internal List<VASTAd> ParseVASTResponse(string vastXml)
        {
            List<VASTAd> ads = new List<VASTAd>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(vastXml);

            XmlNodeList adNodes = doc.GetElementsByTagName("Ad");
            foreach (XmlNode adNode in adNodes)
            {
                VASTAd vastAd = new VASTAd();

                // Extract MediaFile URL
                XmlNode mediaFileNode = adNode.SelectSingleNode(".//MediaFile");
                if (mediaFileNode != null)
                {
                    vastAd.MediaFileUrl = mediaFileNode.InnerText;
                }

                // Extract ClickThrough URL
                XmlNode clickThroughNode = adNode.SelectSingleNode(".//ClickThrough");
                if (clickThroughNode != null)
                {
                    vastAd.ClickThroughUrl = clickThroughNode.InnerText;
                }

                ads.Add(vastAd);
            }

            return ads;
        }
    }

}