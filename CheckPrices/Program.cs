using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

//Here is the once-per-application setup information
[assembly: log4net.Config.XmlConfigurator(Watch = true)]



namespace CheckPrices
{
    static class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static readonly int NThreads = int.Parse(SettingsConfigurationSection.AppSetting("Threads"));
        static Thread[] threads = new Thread[NThreads];
        static AutoResetEvent stopWaitHandle;

        static List<Sites> sites;
        static Stack<Tuple<string, string, Articles>> PricesToCalculate = new Stack<Tuple<string, string, Articles>>();
        static List<Articles> Articles = new List<Articles>();

        private class args
        {
            public int index { get; private set; }
            public string site { get; private set; }
            public string code { get; private set; }
            public Articles article { get; private set; }

            public args(int index, string site, string code, Articles article)
            {
                this.index = index;
                this.site = site;
                this.code = code;
                this.article = article;
            }
        }

        [STAThread]
        static void Main()
        {
            log.Info("Start");
            Stopwatch timer = Stopwatch.StartNew();
            XLogger.Application = String.Format("CheckPrices||{0}||{1}", ConfigurationManager.AppSettings["ExeVersion"], DateTime.Now.ToString("yyyyMMddHHmmss"));
            XLogger.Info("BEGIN:\t Scheduled Task Execution");

            try
            {
                var prices = ReadPrices();
                SendReport(prices);

                /*
                //OLD
                ComputePrices_OLD();
                SendReport_OLD();
                */
            }
            catch (Exception ex)
            {
                log.Error("Exception occured: ", ex);
            }

            log.Info("End");
            TimeSpan ts = timer.Elapsed;
            var elapsed = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            XLogger.Info($"END:{elapsed}\t Scheduled Task Execution");
        }

        private static void SendReport(List<ReportEntry> prices)
        {
            //var html = Extensions.GetMyTable<ReportEntry>(prices, re=> re.idx, re=>re.PartCode);
            var html = Utils.GetMyTable<ReportEntry>(prices,
                r => r.idx, r => r.PartCode, r => r.HGMPriceStr, r => r.TruparPriceStr, r => r.DifferenceStr, r => r.DifferencePCTStr);

            var contentsFile = "LastMail.html";
            //log.DebugFormat("Sending mail with contents: {0}", html);
            log.DebugFormat("Sending mail");
            contentsFile.DeleteFile();
            File.WriteAllText(contentsFile, html);
            //
            int Port;
            int.TryParse(SettingsConfigurationSection.AppSetting("SendMailPort"), out Port);
            using (SmtpClient client = new SmtpClient(SettingsConfigurationSection.AppSetting("SendMailHost"), Port))
            {
                client.EnableSsl = SettingsConfigurationSection.AppSetting("SendMailEnableSsl").Equals("true", StringComparison.OrdinalIgnoreCase);
                MailAddress from = new MailAddress(SettingsConfigurationSection.AppSetting("SendMailFrom"), SettingsConfigurationSection.AppSetting("SendMailFrom"));
                string SendMailTo = SettingsConfigurationSection.AppSetting("SendMailTo");
#if false
                    MailAddress to = new MailAddress(SendMailTo, SendMailTo);
                    using (MailMessage message = new MailMessage(from, to))
                    {
#else
                using (MailMessage message = new MailMessage())
                {
                    message.From = from;
                    foreach (var to in (SendMailTo ?? "").Replace(";", ",").Split(',').Select(x => x.Trim()).Where(x => (x.Length > 0) && (x.Contains('@'))))
                        message.To.Add(to);

#endif
                    message.IsBodyHtml = true;
                    message.Subject = SettingsConfigurationSection.AppSetting("SendMailSubject");
                    message.Body = html;
                    message.ReplyToList.Add(SettingsConfigurationSection.AppSetting("SendMailFrom"));
                    NetworkCredential myCreds = new NetworkCredential(SettingsConfigurationSection.AppSetting("SendMailUser"), SettingsConfigurationSection.AppSetting("SendMailPwd"), SettingsConfigurationSection.AppSetting("SendMailDomain"));
                    client.Credentials = myCreds;
                    client.Send(message);
                }
            }

        }

        private static List<ReportEntry> ReadPrices()
        {
            var res = new List<ReportEntry>();
            try
            {
                PartUrl.GetRefs(Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["InputFile"]));

                //var currentRef = PartUrl.refs.FirstOrDefault(r => r.idx == 1);
                ParallelOptions options = new ParallelOptions() { MaxDegreeOfParallelism = NThreads };
                //foreach (var currentRef in PartUrl.refs)
                Parallel.ForEach(PartUrl.refs, options, (currentRef) =>
                {
                    string hgmPartCode, trupartPartCode;
                    decimal hgmPrice, truparPrice;
                    Engine.ParseHGM(currentRef.hgmURL, out hgmPartCode, out hgmPrice);
                    Engine.ParseTrupar(currentRef.TruparURL, out trupartPartCode, out truparPrice);
                    var rep = new ReportEntry()
                    {
                        idx = currentRef.idx,
                        HGMPartCode = hgmPartCode,
                        TruparPartCode = trupartPartCode,
                        HGMPrice = hgmPrice,
                        TruparPrice = truparPrice,
                        Difference = Math.Round(truparPrice - hgmPrice, 2),
                        DifferencePCT = Math.Round((truparPrice - hgmPrice) / hgmPrice * 100, 2)
                    };

                    res.Add(rep);
                }
                );

                return res.OrderBy(r => r.idx).ToList();
            }
            catch (Exception x)
            {
                XLogger.Error(x);
            }
            return res;
        }
        private static void SendReport_OLD()
        {
            StringBuilder sb = new StringBuilder("<table border='1'><tr><th>HGMLiftParts Code</th><th>HGMLiftParts Price</th><th>Site</th><th>Code</th><th>Price</th><th>Difference</th><th>Difference (%)</th></tr>");
            foreach (var article in Articles)
            {
                sb.AppendFormat("<tr><td>{0}</td><td align='right'>{1:N2}</td><td>{2}</td><td>{3}</td><td align='right'>{4}{5:N2}{6}</td><td align='right'>{7:+0.00;-0.00;0.00}</td><td align='right'>{8:+0.00;-0.00;0.00}</td></tr>", article.HGMLiftParts_Code, article.HGMLiftParts_Price == -1 ? null : article.HGMLiftParts_Price, article.Site, article.Code, article.Price != -1 && article.Price < article.HGMLiftParts_Price ? "<font color=red>" : "", article.Price == -1 ? null : article.Price, article.Price != -1 && article.Price < article.HGMLiftParts_Price ? "</font>" : "", article.Price == -1 || article.HGMLiftParts_Price == -1 ? null : article.Price - article.HGMLiftParts_Price, article.Price == -1 || article.HGMLiftParts_Price == -1 || article.HGMLiftParts_Price == 0 ? null : (article.Price - article.HGMLiftParts_Price) / article.HGMLiftParts_Price * 100.0);
            }
            sb.Append("</table>");
            //File.AppendAllText("output.html", sb.ToString());

            var contents = sb.ToString();
            var contentsFile = "LastMail.html";
            log.DebugFormat("Sending mail with contents: {0}", sb.ToString());
            contentsFile.DeleteFile();
            File.WriteAllText(contentsFile, contents);

            int Port;
            int.TryParse(SettingsConfigurationSection.AppSetting("SendMailPort"), out Port);
            using (SmtpClient client = new SmtpClient(SettingsConfigurationSection.AppSetting("SendMailHost"), Port))
            {
                client.EnableSsl = SettingsConfigurationSection.AppSetting("SendMailEnableSsl").Equals("true", StringComparison.OrdinalIgnoreCase);
                MailAddress from = new MailAddress(SettingsConfigurationSection.AppSetting("SendMailFrom"), SettingsConfigurationSection.AppSetting("SendMailFrom"));
                string SendMailTo = SettingsConfigurationSection.AppSetting("SendMailTo");
#if false
                    MailAddress to = new MailAddress(SendMailTo, SendMailTo);
                    using (MailMessage message = new MailMessage(from, to))
                    {
#else
                using (MailMessage message = new MailMessage())
                {
                    message.From = from;
                    foreach (var to in (SendMailTo ?? "").Replace(";", ",").Split(',').Select(x => x.Trim()).Where(x => (x.Length > 0) && (x.Contains('@'))))
                        message.To.Add(to);

#endif
                    message.IsBodyHtml = true;
                    message.Subject = SettingsConfigurationSection.AppSetting("SendMailSubject");
                    message.Body = sb.ToString();
                    message.ReplyToList.Add(SettingsConfigurationSection.AppSetting("SendMailFrom"));
                    NetworkCredential myCreds = new NetworkCredential(SettingsConfigurationSection.AppSetting("SendMailUser"), SettingsConfigurationSection.AppSetting("SendMailPwd"), SettingsConfigurationSection.AppSetting("SendMailDomain"));
                    client.Credentials = myCreds;
                    client.Send(message);
                }
            }
        }

        static void ComputePrices_OLD()
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = NThreads;
            var settings = SettingsConfigurationSection.Settings("Settings");
            sites = settings.Sites;

            var articles = settings.Articles;
            string HGMLiftParts_Code = "";

            foreach (var article in articles.OrderBy(x => x.HGMLiftParts_Code).ThenBy(x => x.Site))
            {
                Articles.Add(article);
                if (article.HGMLiftParts_Code != HGMLiftParts_Code)
                    PricesToCalculate.Push(new Tuple<string, string, Articles>("HGMLiftParts", HGMLiftParts_Code = article.HGMLiftParts_Code, article));
                PricesToCalculate.Push(new Tuple<string, string, Articles>(article.Site, article.Code, article));
            }

            while (PricesToCalculate.Count > 0)
            {
                stopWaitHandle = new AutoResetEvent(false);
                for (int i = 0; (i < NThreads) && (PricesToCalculate.Count > 0); i++)
                    if (threads[i] == null)
                    {
                        threads[i] = new Thread(HandleThread);
                        var PriceToCalculate = PricesToCalculate.Pop();
                        threads[i].Name = string.Format("site: {0}, code: {1}", PriceToCalculate.Item1, PriceToCalculate.Item2);
                        threads[i].Start(new args(i, PriceToCalculate.Item1, PriceToCalculate.Item2, PriceToCalculate.Item3));
                    }
                stopWaitHandle.WaitOne();
            }

            for (int i = 0; i < NThreads; i++)
                if (threads[i] != null)
                    threads[i].Join();

            HGMLiftParts_Code = "";
            double? HGMLiftParts_Price = null;
            foreach (var article in Articles)
            {
                if (article.HGMLiftParts_Code == HGMLiftParts_Code)
                {
                    article.HGMLiftParts_Price = HGMLiftParts_Price;
                }
                else
                {
                    HGMLiftParts_Price = article.HGMLiftParts_Price;
                    HGMLiftParts_Code = article.HGMLiftParts_Code;
                }
            }

        }
        static string LoadPage(string url)
        {
            string buf = "";
            int i;
            const int maxCount = 1;

            for (i = 0; (i < maxCount) && (buf.Length == 0); i++)
            {
                try
                {
                    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                    myRequest.Method = "GET";
                    //myRequest.Timeout = 100000;

                    WebHeaderCollection myWebHeaderCollection = myRequest.Headers;

                    myWebHeaderCollection.Add("Accept-Language: en-US,en;q=0.8");

                    using (HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse())
                    {
                        using (StreamReader sr = new StreamReader(myResponse.GetResponseStream() /* , System.Text.Encoding.UTF8 */))
                        {
                            buf = sr.ReadToEnd();
                            sr.Close();
                        }
                        myResponse.Close();
                    }
                }
                catch (Exception ex)
                {
                    if (i == maxCount)
                        throw ex;
                    buf = ex.Message;
                    buf = "";
                }
            }

            return buf;
        }
        private static void HandleThread(object args)
        {
            args typedargs = args as args;

            log.DebugFormat("Thread started for {0}, code: {1}", typedargs.site, typedargs.code);
            var price = GetPrice(typedargs.site, typedargs.code, typedargs.article);
            log.DebugFormat("Thread done for {0}, code: {1}, price: {2}", typedargs.site, typedargs.code, price);
            threads[typedargs.index] = null;
            stopWaitHandle.Set();
        }
        private static double GetPrice(string Site, string Code, Articles article)
        {
            string s;
            var site = sites.Single(x => x.Site == Site);

            var url = string.Format(site.URL, Code);
            s = HtmlAgility.GetWebContent(url);
            s = s.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');
            var match = Regex.Match(s, site.Match).Groups[int.Parse(site.Group)].Value.Replace(",", "").TrimStart('$');

            double price;
            if (!double.TryParse(match, out price))
                price = -1;
            if (Site == "HGMLiftParts")
                article.HGMLiftParts_Price = price;
            else
                article.Price = price;

            return price;
        }
    }
}
