using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HAP = HtmlAgilityPack;

namespace CheckPrices
{
    static class Engine
    {
        public static void ParseHGM(string hgmUrl, out string hgmPartCode, out decimal hgmPrice)
        {
            hgmPrice = -1;
            hgmPartCode = string.Empty;
            try
            {
                if (!HtmlAgility.UrlIsValid(hgmUrl))
                    throw new ArgumentException("invalid url", "hgmUrl");

                HAP.HtmlDocument docSearch;
                HtmlAgility.GetDocumentFromUrl(hgmUrl, out docSearch);
                var strPrice = HtmlAgility.ScrapElement(docSearch, ConfigurationManager.AppSettings["HGM.Price"])?.Trim('$');
                decimal.TryParse(strPrice, out hgmPrice);
                hgmPartCode = HtmlAgility.ScrapElement(docSearch, ConfigurationManager.AppSettings["HGM.PartCode"]);
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
                if (!HtmlAgility.UrlIsValid(truparURL))
                    throw new ArgumentException("invalid url", "truparURL");

                HAP.HtmlDocument docSearch;
                HtmlAgility.GetDocumentFromUrl(truparURL, out docSearch);
                var strPrice = HtmlAgility.ScrapElement(docSearch, ConfigurationManager.AppSettings["Trupar.Price"])?.Trim('$');
                partCode = HtmlAgility.ScrapElement(docSearch, ConfigurationManager.AppSettings["Trupar.PartCode"]);

                /* - details
                var detailsUrl = HtmlAgility.GetUrlFromAnchor(docSearch, ConfigurationManager.AppSettings["Trupar.Details"]);
                HAP.HtmlDocument docDetails;
                HtmlAgility.GetDocumentFromUrl(detailsUrl, out docDetails);
                var strPrice = HtmlAgility.ScrapElement(docSearch, ConfigurationManager.AppSettings["Trupar.Price"])?.Trim('$');
                partCode = HtmlAgility.ScrapElement(docSearch, ConfigurationManager.AppSettings["Trupar.PartCode"]);
                // IList<HAP.HtmlNode> nodes = doc.QuerySelectorAll("span .itemprop > ul li");
                */

                decimal.TryParse(strPrice, out truparPrice);
            }
            catch (Exception x)
            {
                x.Data.Add("truparURL", truparURL);
                XLogger.Error(x);
            }
        }

        public static void ParseLiftParts(string liftPartsUrl, out string liftPartsPartCode, out decimal liftPartsPrice)
        {
            liftPartsPrice = -1;
            liftPartsPartCode = string.Empty;
            try
            {
                if (!HtmlAgility.UrlIsValid(liftPartsUrl))
                    throw new ArgumentException("invalid url", "liftPartsUrl");

                HAP.HtmlDocument docSearch;
                HtmlAgility.GetDocumentFromUrl(liftPartsUrl, out docSearch);
                var strPrice = HtmlAgility.ScrapElement(docSearch, ConfigurationManager.AppSettings["LiftParts.Price"])?.Trim('$').Trim();
                decimal.TryParse(strPrice, out liftPartsPrice);
                liftPartsPartCode = HtmlAgility.ScrapElement(docSearch, ConfigurationManager.AppSettings["LiftParts.PartCode"]);
            }
            catch (Exception x)
            {
                x.Data.Add("liftPartsUrl", liftPartsUrl);
                XLogger.Error(x);
            }
        }

    }
}
