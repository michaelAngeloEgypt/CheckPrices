using HAP = HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Text.RegularExpressions;

namespace CheckPrices
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            XLogger.Application = "CheckPrices";
        }

        private string ReadPrice(SiteConfig siteConfig, string pageSource)
        {
            try
            {
                string res = "";
                if (siteConfig == null || String.IsNullOrEmpty(pageSource))
                    throw new ApplicationException("the inputs are invalid");

                Regex regex = new Regex(siteConfig.Match);
                Match match = regex.Match(pageSource);
                if (match.Success)
                {
                    Console.WriteLine(match.Value);
                    res = match.Groups[siteConfig.Group].Value.TrimStart('$');
                }

                return res;
            }
            catch (Exception x)
            {
                XLogger.Error(x);
                throw;
            }
        }

        private SiteConfig ReadSiteConfig()
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                string theNodeXPath = "//Sites/add[@Site='trupar']";

                XmlDocument doc = new XmlDocument();
                doc.Load(config.FilePath);
                var theNode = doc.DocumentElement.SelectSingleNode(theNodeXPath);//.Attributes["value"].Value;
                //priceHtmlRegex  string value = doc.DocumentElement.SelectSingleNode("/configuration/appSettings/add[@key='MyKeyName']").Attributes["value"].Value;

                var siteConfig = new SiteConfig()
                {
                    Site = theNode.Attributes["Site"].Value,
                    Match = theNode.Attributes["Match"].Value,
                    Group = int.Parse(theNode.Attributes["Group"].Value)
                };

                return siteConfig;
            }
            catch (Exception x)
            {
                XLogger.Error(x);
                throw;
            }
        }


        private string ReadPageSource()
        {
            try
            {
                return HtmlAgility.GetWebContent(txtProductUrl.Text);
            }
            catch (Exception x)
            {
                XLogger.Error(x);
                throw;
            }
        }
        private void btnGetPrice_Click(object sender, EventArgs e)
        {
            try
            {
                var siteConfig = ReadSiteConfig();
                var pageSource = ReadPageSource();

                string price = ReadPrice(siteConfig, pageSource);
                txtProductPrice.Text = price;
            }
            catch
            {
                MessageBox.Show("Something went wrong :(");
            }
        }
    }
}
