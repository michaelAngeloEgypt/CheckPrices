using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text.RegularExpressions;

public static class HtmlAgility
{

    public static string ReadHtmlBetween(string originalContent, string start, string end)
    {
        string section = originalContent.Between(start, end);
        HtmlDocument fragment;
        GetDocumentFromString(section, out fragment);
        string value = fragment.DocumentNode.InnerText.Cleanup();
        return value;
    }
    public static bool GetDocumentFromString(string fragment, out HtmlDocument doc)
    {
        doc = new HtmlDocument();

        try
        {
            doc.LoadHtml(fragment);

            if (doc.ParseErrors != null && doc.ParseErrors.Count() > 0)
            {
                // Handle any parse errors as required
                //return false;
            }

            return true;
        }
        catch (Exception e)
        {
            XLogger.Error(e);
            return false;
        }
    }
    public static bool GetDocumentFromFile(string filePath, out HtmlDocument doc)
    {
        doc = new HtmlDocument();

        try
        {
            String content = File.ReadAllText(filePath);

            doc.LoadHtml(content);

            if (doc.ParseErrors != null && doc.ParseErrors.Count() > 0)
            {
                // Handle any parse errors as required
                //return false;
            }

            return true;
        }
        catch (Exception e)
        {
            XLogger.Error(e);
            return false;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="theUrl"></param>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static bool GetDocumentFromUrl(string theUrl, out HtmlDocument doc)
    {
        doc = new HtmlDocument();

        try
        {
            String content = GetWebContent(theUrl);

            doc.LoadHtml(content);

            if (doc.ParseErrors != null && doc.ParseErrors.Count() > 0)
            {
                // Handle any parse errors as required
                //return false;
            }

            return true;
        }
        catch (Exception e)
        {
            e.Data.Add("theUrl", theUrl);
            XLogger.Error(e);
            return false;
        }
    }
    public static string GetWebContent(string url, int numRetries = 1)
    {
        string content = String.Empty;
        int consumedRetries = numRetries;
        while (String.IsNullOrWhiteSpace(content) && consumedRetries > 0)
        {
            //content = GetWebContentCore1(url);
            //content = GetWebContentCore2(url);
            //content = GetWebContentCore3(url);
            //content = GetWebContentCore4(url);
            content = GetWebContentCore5(url);
            //content = GetWebContentCore6(url);
            //content = GetWebContentCore7(url);
            //content = GetWebContentCore8(url);
            consumedRetries--;
        }
        return content;
    }
    public static string PostAndGetWebContent(string url, string user, string pass, int numRetries = 1)
    {
        string content = String.Empty;
        int consumedRetries = numRetries;
        while (String.IsNullOrWhiteSpace(content) && consumedRetries > 0)
        {
            content = PostAndGetWebContentCore1(url, user, pass);
            content = PostAndGetWebContentCore2(url, user, pass);
            content = PostAndGetWebContentCore3(url, user, pass);
            consumedRetries--;
        }
        return content;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private static string GetWebContentCore1(string url, bool useProxy = false)
    {

        System.IO.Stream st = null;
        System.IO.StreamReader sr = null;

        if (!url.StartsWith("http://"))
            url = "http://" + url;

        try
        {
            // make a Web request
            System.Net.HttpWebRequest req = HttpWebRequest.Create(url) as HttpWebRequest;

            if (useProxy)
            {
                // Create proxy authentication object
                NetworkCredential netCred = new NetworkCredential();
                netCred.UserName = "EHussuin";
                netCred.Password = "Newcoproject18";
                netCred.Domain = "vf-eg";
                req.Credentials = netCred;
                WebProxy wp = new WebProxy();
                wp.Credentials = netCred;
                wp.Address = new Uri("http://10.230.233.30:5110/proxy.pac");
                req.Proxy = wp;
            }
            // get the response and read from the result stream
            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;

            st = resp.GetResponseStream();
            sr = new System.IO.StreamReader(st, Encoding.UTF8);

            // read all the text in it
            return sr.ReadToEnd();
        }
        catch (Exception x)
        {
            x.Data.Add("url", url);
            XLogger.Error(x);
            return string.Empty;
        }
        finally
        {
            // always close readers and streams
            if (sr != null)
                sr.Close();

            if (st != null)
                st.Close();
        }
    }
    private static string GetWebContentCore2(string url, bool useProxy = false)
    {
        try
        {
            WebClient wc = new WebClient();

            if (!url.StartsWith("http://"))
                url = "http://" + url;

            wc.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.115 Safari/537.36";

            //wc.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            //wc.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
            //wc.Headers.Add("Accept-Language", "he-IL,he;q=0.8,en-US;q=0.6,en;q=0.4");
            //wc.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            //var data = wc.DownloadData(url);
            //return Encoding.UTF8.GetString(data);

            if (useProxy)
            {
                // Create proxy authentication object
                NetworkCredential netCred = new NetworkCredential();
                netCred.UserName = "EHussuin";
                netCred.Password = "Newcoproject18";
                netCred.Domain = "vf-eg";
                wc.Credentials = netCred;
                WebProxy wp = new WebProxy();
                wp.Credentials = netCred;
                wp.Address = new Uri("http://10.230.233.30:5110/proxy.pac");
                wc.Proxy = wp;
            }
            return wc.DownloadString(url);
        }
        catch (Exception x)
        {
            x.Data.Add("url", url);
            XLogger.Error(x);
            return string.Empty;
        }
    }
    private static string GetWebContentCore3(string url)
    {
        try
        {
            using (var client = new HttpClient())
            {
                //return client.GetStringAsync(url);
                return "";
            }
        }
        catch (Exception x)
        {
            x.Data.Add("url", url);
            XLogger.Error(x);
            return string.Empty;
        }
    }
    public static string GetWebContentCore4(string url, bool useProxy = false)
    {
        String res = "";
        System.IO.Stream st = null;
        System.IO.StreamReader sr = null;

        try
        {
            System.Net.HttpWebRequest req = HttpWebRequest.Create(url) as HttpWebRequest;
            req.Method = "GET";
            req.KeepAlive = false;
            req.ContentType = "text/html";
            if (useProxy)
            {
                // Create proxy authentication object
                NetworkCredential netCred = new NetworkCredential();
                netCred.UserName = "EHussuin";
                netCred.Password = "Newcoproject18";
                netCred.Domain = "vf-eg";
                req.Credentials = netCred;
                WebProxy wp = new WebProxy();
                wp.Credentials = netCred;
                wp.Address = new Uri("http://10.230.233.30:5110/proxy.pac");
                req.Proxy = wp;
            }

            using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
            {
                st = response.GetResponseStream();
                sr = new System.IO.StreamReader(st, Encoding.UTF8);
                res = sr.ReadToEnd();
                sr.Close();
                st.Close();
            }

            return res;
        }
        catch (Exception x)
        {
            x.Data.Add("url", url);
            XLogger.Error(x);
            return string.Empty;
        }
    }
    private static string GetWebContentCore5(string url)
    {
        string res = "";
        try
        {
            using (MyClient client = new MyClient()) // WebClient class inherits IDisposable
            {
                //client.DownloadFile(url, @"C:\localfile.html");

                // Or you can get the file content without saving it
                res = client.DownloadString(url);
            }

            return res;
        }
        catch (Exception x)
        {
            x.Data.Add("url", url);
            throw;
        }
    }
    private static string GetWebContentCore6(string url)
    {
        string res = "";
        try
        {
            var myClient = new MyClient();
            Uri myUri = new Uri(url, UriKind.Absolute);
            using (Stream data = myClient.OpenRead(myUri))
            {
                using (StreamReader reader = new StreamReader(data))
                {
                    string s = reader.ReadToEnd();
                    data.Close();
                    reader.Close();
                }
            }

            //MyWebRequest myRequest = new MyWebRequest(url);
            //res = myRequest.GetResponse();

            return res;
        }
        catch (Exception x)
        {
            x.Data.Add("url", url);
            throw;
        }
    }
    private static string GetWebContentCore7(string url)
    {
        string res = "";
        try
        {
            var homeRequest = WebRequest.CreateHttp(url);
            res = homeRequest.GetResponse().Body();

            return res;
        }
        catch (Exception x)
        {
            x.Data.Add("url", url);
            throw;
        }
    }
    private static string GetWebContentCore8(string url)
    {
        string res = "";
        try
        {
            WebRequest request = WebRequest.Create("http://www.example.com/test.xml");
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            return reader.ReadToEnd();
        }
        catch (Exception x)
        {
            x.Data.Add("url", url);
            throw;
        }
    }

    /// <summary>
    /// Access a page from behind a corporate proxy
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static string GetWebContentWithProxy(string url)
    {
        var client = new WebClient();

        NetworkCredential netCred = new NetworkCredential();
        netCred.UserName = "EHussuin";
        netCred.Password = "Newcoproject17";
        netCred.Domain = "vf-eg";
        client.Credentials = netCred;
        WebProxy wp = new WebProxy();
        wp.Credentials = netCred;
        wp.Address = new Uri(@"http://10.230.233.30:5110/proxy.pac");
        client.Proxy = wp;

        string accessToken = client.DownloadString(url);
        return accessToken;
    }
    //
    private static string PostAndGetWebContentCore1(string url, string user, string pass)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            var postData = String.Concat("thing1=", user);
            postData += String.Concat("&thing2=", pass);
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responseString;
        }
        catch (Exception x)
        {
            XLogger.Error(x);
            return "";
        }
    }
    private static string PostAndGetWebContentCore2(string url, string user, string pass)
    {
        try
        {
            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                values["thing1"] = user;
                values["thing2"] = pass;

                var response = client.UploadValues(url, values);

                var responseString = Encoding.Default.GetString(response);
                return responseString;
            }
        }
        catch (Exception x)
        {
            XLogger.Error(x);
            return "";
        }
    }
    private static string PostAndGetWebContentCore3(string url, string user, string pass)
    {
        try
        {
            throw new NotImplementedException();
            /*
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
            {
               { "thing1", user },
               { "thing2", pass }
            };

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("http://www.example.com/recepticle.aspx", content);

                var responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            */
        }
        catch (Exception x)
        {
            XLogger.Error(x);
            return "";
        }
    }
    //
    /// <summary>
    /// <see cref="http://stackoverflow.com/questions/10485903/regex-extract-value-from-the-string-between-delimiters"/>
    /// <seealso cref="http://derekslager.com/blog/posts/2007/09/a-better-dotnet-regular-expression-tester.ashx"/>
    /// <seealso cref="http://regexlib.com/"/>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string Cleanup(this string source)
    {
        string result = source;
        result = result.Replace("<![if !supportLists]>", "").Replace("<![endif]>", "");
        result = result.Replace("\r\n", " ").Replace("\t", " ");
        while (result.Contains("  "))
            result = result.Replace("  ", " ");

        StringWriter myWriter = new StringWriter();
        System.Net.WebUtility.HtmlDecode(result, myWriter);
        result = myWriter.ToString();

        return result.Trim();
    }
    //
    public static List<String> ScrapeElements(HtmlDocument doc, string itemsXpath)
    {
        List<String> results = new List<string>();
        List<HtmlNode> nodes = new List<HtmlNode>();
        return ScrapeElementsAndGetNodes(doc, itemsXpath, out nodes);
    }
    public static List<String> ScrapeElements(string pageUrl, string itemsXpath)
    {
        List<HtmlNode> nodes = new List<HtmlNode>();
        return ScrapeElementsAndGetNodes(pageUrl, itemsXpath, out nodes);
    }
    public static List<String> ScrapeElementsAndGetNodes(HtmlDocument doc, string itemsXpath, out List<HtmlNode> nodes)
    {
        List<String> results = new List<string>();
        nodes = new List<HtmlNode>();
        try
        {
            var nodesCore = doc.DocumentNode.SelectNodes(itemsXpath);
            if (nodesCore == null)
                throw new ApplicationException("Invalid path for items, no items could be read");
            else
                nodes.AddRange(nodesCore.ToList());

            //var theNodesUrls = theNodes.Select(n => n.ParentNode.Attributes["href"].Value);
            var theNodesTexts = nodes.Select(n => n.InnerText.Cleanup());
            results.AddRange(theNodesTexts);
        }
        catch (Exception x)
        {
            x.Data.Add("itemsXpath", itemsXpath);
            XLogger.Error(x);
        }
        return results;
    }
    public static List<String> ScrapeElementsAndGetNodes(string pageUrl, string itemsXpath, out List<HtmlNode> nodes)
    {
        List<String> results = new List<string>();
        nodes = new List<HtmlNode>();
        try
        {
            string content = GetWebContent(pageUrl);
            if (String.IsNullOrEmpty(content))
                throw new ApplicationException(String.Concat("Page Url cannot be read: ", pageUrl));

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);

            var nodesCore = doc.DocumentNode.SelectNodes(itemsXpath);
            if (nodesCore == null)
                throw new ApplicationException("Invalid path for items, no items could not be read");
            else
                nodes.AddRange(nodesCore.ToList());

            //var theNodesUrls = theNodes.Select(n => n.ParentNode.Attributes["href"].Value);
            var theNodesTexts = nodes.Select(n => n.InnerText.Cleanup());
            results.AddRange(theNodesTexts);
        }
        catch (Exception x)
        {
            x.Data.Add("itemsXpath", itemsXpath);
            XLogger.Error(x);
        }
        return results;
    }
    //
    public static string GetUrlFromAnchor(HtmlDocument doc, string elementXml, bool doCleanup = true)
    {
        HtmlNode theNode = null;
        ScrapElementAndGetNode(doc, elementXml, out theNode, doCleanup);
        if (theNode == null || theNode.Attributes["href"] == null || string.IsNullOrWhiteSpace(theNode.Attributes["href"].Value))
            return string.Empty;

        return theNode.Attributes["href"].Value;
    }
    public static string ScrapElement(HtmlDocument doc, string elementXml, bool doCleanup = true)
    {
        HtmlNode theNode = null;
        return ScrapElementAndGetNode(doc, elementXml, out theNode, doCleanup);
    }
    public static string ScrapElementAndGetNode(HtmlDocument doc, string elementXml, out HtmlNode theNode, bool doCleanup = true)
    {
        string output = String.Empty;
        theNode = null;
        try
        {
            if (String.IsNullOrWhiteSpace(elementXml))
                return output;
            theNode = doc.DocumentNode.SelectSingleNode(elementXml);
            if (theNode == null)
                return output;

            string queryElement = Path.GetFileName(elementXml);
            if (queryElement.StartsWith("@"))
                output = theNode.Attributes[queryElement.Replace("@", "")].Value;
            else
                output = theNode.InnerText;

            if (doCleanup)
                output = output.Cleanup();
        }
        catch (Exception x)
        {
            x.Data.Add("elementXml", elementXml);
            throw;
        }
        return output;
    }

    /// <summary>
    /// <see cref="https://stackoverflow.com/questions/924679/c-sharp-how-can-i-check-if-a-url-exists-is-valid"/>
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static bool UrlIsValid(string url)
    {
        try
        {
            using (var client = new MyClient())
            {
                client.HeadOnly = true;
                client.DownloadString(url);
                return true;
            }
        }
        catch
        {
            //either 404 or 503 
            return false;
        }
    }
    //
}

static class Ext
{
    #region Extensions
    /// <summary>
    /// <see cref="http://stackoverflow.com/questions/10485903/regex-extract-value-from-the-string-between-delimiters"/>
    /// <see cref="http://stackoverflow.com/questions/378415/how-do-i-extract-text-that-lies-between-parentheses-round-brackets"/>
    /// <seealso cref="https://regex101.com"/>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="startDelimiter"></param>
    /// <param name="endDelimiter"></param>
    /// <param name="includeDelimiters"></param>
    /// <returns></returns>
    public static string ExtractStringBetweenDelimiters(this string source, string startDelimiter, string endDelimiter, bool includeDelimiters = true)
    {
        if (String.IsNullOrWhiteSpace(source))
            return String.Empty;

        string victim = Regex.Match(source, String.Format(@"{0}(.*?){1}", startDelimiter, endDelimiter)).Value;

        if (!includeDelimiters)
            victim = victim.Between(startDelimiter, endDelimiter);

        return victim;
    }
    /// <summary>
    /// <see cref="https://stackoverflow.com/a/13280617/193974"/>
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    internal static string Body(this WebResponse response)
    {
        var stream = response.GetResponseStream();
        using (var reader = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
        {
            return reader.ReadToEnd();
        }
    }
    #endregion Extensions

}
class MyClient : WebClient
{
    public bool HeadOnly { get; set; }
    /// <summary>
    /// <see cref="https://stackoverflow.com/questions/16829074/how-to-read-html-source-from-a-https-url"/>
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    protected override WebRequest GetWebRequest(Uri address)
    {
        var req = base.GetWebRequest(address) as HttpWebRequest;
        if (HeadOnly && req.Method == "GET")
        {
            req.Method = "HEAD";
        }
        req.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        return req;
    }
}

/// <summary>
/// <see cref="http://codesamplez.com/programming/http-request-c-sharp"/>
/// </summary>
public class MyWebRequest
{
    private WebRequest request;
    private Stream dataStream;

    private string status;

    public String Status
    {
        get
        {
            return status;
        }
        set
        {
            status = value;
        }
    }

    public MyWebRequest(string url)
    {
        // Create a request using a URL that can receive a post.

        request = WebRequest.Create(url);
    }

    public MyWebRequest(string url, string method)
        : this(url)
    {

        if (method.Equals("GET") || method.Equals("POST"))
        {
            // Set the Method property of the request to POST.
            request.Method = method;
        }
        else
        {
            throw new Exception("Invalid Method Type");
        }
    }

    public MyWebRequest(string url, string method, string data)
        : this(url, method)
    {

        // Create POST data and convert it to a byte array.
        string postData = data;
        byte[] byteArray = Encoding.UTF8.GetBytes(postData);

        // Set the ContentType property of the WebRequest.
        request.ContentType = "application/x-www-form-urlencoded";

        // Set the ContentLength property of the WebRequest.
        request.ContentLength = byteArray.Length;

        // Get the request stream.
        dataStream = request.GetRequestStream();

        // Write the data to the request stream.
        dataStream.Write(byteArray, 0, byteArray.Length);

        // Close the Stream object.
        dataStream.Close();

    }

    public string GetResponse()
    {
        // Get the original response.
        WebResponse response = request.GetResponse();

        this.Status = ((HttpWebResponse)response).StatusDescription;

        // Get the stream containing all content returned by the requested server.
        dataStream = response.GetResponseStream();

        // Open the stream using a StreamReader for easy access.
        StreamReader reader = new StreamReader(dataStream);

        // Read the content fully up to the end.
        string responseFromServer = reader.ReadToEnd();

        // Clean up the streams.
        reader.Close();
        dataStream.Close();
        response.Close();

        return responseFromServer;
    }

}