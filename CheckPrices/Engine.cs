using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HAP = HtmlAgilityPack;

namespace CheckPrices
{
    static class Engine
    {
        public static void ParseHGM(string hgmUrl, out decimal hgmPrice)
        {
            hgmPrice = -1;
            try
            {
                if (!HtmlAgility.UrlIsValid(hgmUrl))
                    throw new ArgumentException("invalid url", "url");

                HAP.HtmlDocument docSearch;
                HtmlAgility.GetDocumentFromUrl(hgmUrl, out docSearch);
                var strPrice = HtmlAgility.ScrapElement(docSearch, "//span[@itemprop = 'price']")?.Trim('$');
                decimal.TryParse(strPrice, out hgmPrice);
            }
            catch (Exception x)
            {
                x.Data.Add("hgmUrl", hgmUrl);
                XLogger.Error(x);
            }
        }

        public static void ParseTrupar(string truparURL, out string partCode, out decimal truparPrice)
        {
            partCode = string.Empty;
            truparPrice = -1;

            try
            {
                HAP.HtmlDocument docSearch;
                HtmlAgility.GetDocumentFromUrl(truparURL, out docSearch);
                var detailsUrl = HtmlAgility.GetUrlFromAnchor(docSearch, "//a[@class='link view - product'");

                HAP.HtmlDocument docDetails;
                HtmlAgility.GetDocumentFromUrl(detailsUrl, out docDetails);
                var strPrice = HtmlAgility.ScrapElement(docSearch, "//span[@itemprop = 'price']")?.Trim('$');
                decimal.TryParse(strPrice, out truparPrice);
                partCode = HtmlAgility.ScrapElement(docSearch, "//span[@itemprop = 'sku']");
                // IList<HAP.HtmlNode> nodes = doc.QuerySelectorAll("span .itemprop > ul li");
            }
            catch (Exception x)
            {
                x.Data.Add("truparURL", truparURL);
                XLogger.Error(x);
            }
        }
    }
}
