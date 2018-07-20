using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

public static class MyExtensions
{
    #region String functions
    //
    public static string ToStandardElapsedFormat(this TimeSpan ts)
    {
        return String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
    }
    /// <summary>
    /// http://stackoverflow.com/questions/206717/how-do-i-replace-multiple-spaces-with-a-single-space-in-c
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ReplaceConsecutiveCopiesToOneInstance(this string input, string victim)
    {
        return string.Join(victim, input.Split(new string[] { victim }, StringSplitOptions.RemoveEmptyEntries));
    }
    /// <summary>
    /// Get string value between [first] a and [last] b.
    /// </summary>
    public static string Between(this string value, string a, string b)
    {
        int posA = value.IndexOf(a);
        int posB = value.LastIndexOf(b);
        if (posA == -1)
        {
            return "";
        }
        if (posB == -1)
        {
            return "";
        }
        int adjustedPosA = posA + a.Length;
        if (adjustedPosA >= posB)
        {
            return "";
        }
        return value.Substring(adjustedPosA, posB - adjustedPosA);
    }
    /// <summary>
    /// Get string value after [first] a.
    /// </summary>
    public static string Before(this string value, string a)
    {
        int posA = value.IndexOf(a);
        if (posA == -1)
        {
            return "";
        }
        return value.Substring(0, posA);
    }
    /// <summary>
    /// Get string value after [last] a.
    /// </summary>
    public static string After(this string value, string a)
    {
        int posA = value.LastIndexOf(a);
        if (posA == -1)
        {
            return "";
        }
        int adjustedPosA = posA + a.Length;
        if (adjustedPosA >= value.Length)
        {
            return "";
        }
        return value.Substring(adjustedPosA);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string LastWord(this string value)
    {
        if (!String.IsNullOrEmpty(value))
            return value.Trim().Split(' ').LastOrDefault();
        else
            return "";
    }
    /// <summary>
    /// http://stackoverflow.com/questions/228038/best-way-to-reverse-a-string
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string Reverse(this string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="startTag"></param>
    /// <param name="endTag"></param>
    /// <returns></returns>
    public static string ExtractStringBetweenTags(this string source, string startTag, string endTag)
    {
        if (String.IsNullOrWhiteSpace(source))
            return String.Empty;
        int startIndex = source.IndexOf(startTag) + startTag.Length;
        int endIndex = source.IndexOf(endTag, startIndex);
        return source.Substring(startIndex, endIndex - startIndex);
    }
    /// <summary>
    /// exact letters and symbols, ignoring letter case and whitespace
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool VerySimilarTo(this string x, string y)
    {
        if (String.IsNullOrEmpty(x))
            return String.IsNullOrEmpty(y) ? true : false;
        else if (String.IsNullOrEmpty(y))
            return false;

        string x1 = x.ToLowerInvariant().Replace(" ", "");
        string y1 = y.ToLowerInvariant().Replace(" ", "");

        return x1.Equals(y1);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool SimilarTo(this string x, string y)
    {
        if (String.IsNullOrEmpty(x))
            return String.IsNullOrEmpty(y);
        else
            if (String.IsNullOrEmpty(y))
                return false;

        return x.ToCanonicalForm().Equals(y.ToCanonicalForm(), StringComparison.InvariantCultureIgnoreCase);
    }
    /// <summary>
    /// set to lower case and ignore anything other than digits and letters.
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static string ToCanonicalForm(this string x)
    {
        if (String.IsNullOrWhiteSpace(x)) return x;

        char[] arr = x.ToLowerInvariant().ToCharArray();
        arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c))));   // || char.IsWhiteSpace(c) || c == '-'
        return new string(arr);

        //return x.ToLowerInvariant().Replace(" ", "");
    }
    /// <summary>
    /// case-insensitive string.contains
    /// http://stackoverflow.com/questions/444798/case-insensitive-containsstring
    /// http://ppetrov.wordpress.com/2008/06/27/useful-method-6-of-n-ignore-case-on-stringcontains/
    /// </summary>
    /// <param name="source"></param>
    /// <param name="toCheck"></param>
    /// <param name="comp"></param>
    /// <returns></returns>
    public static bool Contains(this string source, string toCheck, StringComparison comp)
    {
        return source.IndexOf(toCheck, comp) >= 0;
    }
    /// <summary>
    /// http://social.msdn.microsoft.com/Forums/vstudio/en-US/354ed9c9-eea9-47c4-afaf-443182021d94/how-to-convert-current-date-into-string-yyyymmdd-like-20061113?forum=csharpgeneral
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    public static string ToYMD(this DateTime src)
    {
        return src.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
    }
    /// <summary>
    /// useful for Excel ranges
    /// http://stackoverflow.com/questions/181596/how-to-convert-a-column-number-eg-127-into-an-excel-column-eg-aa
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    public static string ToExcelColumnName(this int colNumber)
    {
        int dividend = colNumber;
        string columnName = String.Empty;
        int modulo;

        while (dividend > 0)
        {
            modulo = (dividend - 1) % 26;
            columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
            dividend = (int)((dividend - modulo) / 26);
        }

        return columnName;
    }
    /// <summary>
    /// this is 1-based
    /// http://stackoverflow.com/questions/181596/how-to-convert-a-column-number-eg-127-into-an-excel-column-eg-aa
    /// </summary>
    /// <param name="colAddress"></param>
    /// <returns></returns>
    public static int ToExcelColumnPosition(this string colAddress)
    {
        int[] digits = new int[colAddress.Length];
        for (int i = 0; i < colAddress.Length; ++i)
        {
            digits[i] = Convert.ToInt32(colAddress[i]) - 64;
        }
        int mul = 1; int res = 0;
        for (int pos = digits.Length - 1; pos >= 0; --pos)
        {
            res += digits[pos] * mul;
            mul *= 26;
        }
        return res;
    }
    /// <summary>
    /// http://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net
    /// </summary>
    /// <param name="byteCount"></param>
    /// <param name="num"></param>
    /// <param name="unit"></param>
    public static void BytesAsReadable(this long byteCount, out double num, out string unit)
    {
        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
        num = 0; unit = suf[0];
        if (byteCount == 0)
            return; //return "0" + suf[0];

        long bytes = Math.Abs(byteCount);
        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        num = Math.Round(bytes / Math.Pow(1024, place), 1);
        unit = suf[place];
        //return (Math.Sign(byteCount) * num).ToString() + unit;
    }
    //
    /// <summary>
    /// usage: MilestonesPerPackage = new Dictionary<string, int>(VerySimilarEqualityComparer.Comparer);
    /// </summary>
    class VerySimilarEqualityComparer : IEqualityComparer<String>
    {
        public static VerySimilarEqualityComparer Comparer { get { return new VerySimilarEqualityComparer(); } }

        public bool Equals(String x, String y)
        {
            return x.VerySimilarTo(y);
        }

        public int GetHashCode(String obj)
        {
            return this.GetHashCode();
        }
    }
    /// <summary>
    /// <see cref="https://stackoverflow.com/questions/19768519/c-sharp-extract-multiple-numbers-from-a-string"/>
    /// </summary>
    /// <param name="StringWithNumbers"></param>
    /// <returns></returns>
    public static int[] ExtractNumbers(this string StringWithNumbers)
    {
        var result = new Regex(@"\d+").Matches(StringWithNumbers)
                  .Cast<Match>()
                  .Select(m => Int32.Parse(m.Value))
                  .ToArray();

        return result;
    }
    //
    #endregion String functions

    #region reflection
    //
    /// <summary>
    /// used for downcasting
    /// http://stackoverflow.com/questions/988658/unable-to-cast-from-parent-class-to-child-class
    /// </summary>
    /// <param name="dst"></param>
    /// <param name="src"></param>
    public static void CopyProperties(this object src, object dst)
    {
        PropertyInfo[] srcProperties = src.GetType().GetProperties();
        dynamic dstType = dst.GetType();

        if (srcProperties == null | dstType.GetProperties() == null)
        {
            return;
        }

        foreach (PropertyInfo srcProperty in srcProperties)
        {
            PropertyInfo dstProperty = dstType.GetProperty(srcProperty.Name);

            if (dstProperty != null)
            {
                if (dstProperty.PropertyType.IsAssignableFrom(srcProperty.PropertyType) == true)
                {
                    dstProperty.SetValue(dst, srcProperty.GetValue(src, null), null);
                }
            }
        }
    }
    /// <summary>
    /// return the value of constants
    /// usage: typeof(Lookups.GoLiveDatesWorkbook.ColumnNames).GetConstantsValues();
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static List<String> GetConstantsValues(this Type type)
    {
        FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public |
             BindingFlags.Static | BindingFlags.FlattenHierarchy);

        return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).Select(fi => fi.GetValue(type) as String).ToList<String>();
    }
    /// <summary>
    /// Get property value based on its name
    /// http://www.java2s.com/Code/CSharp/Reflection/Getapropertyvaluegivenitsname.htm
    /// </summary>
    /// <param name="srcObj"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static Object GetPropertyValue(this object srcObj, string propertyName)
    {

        PropertyInfo propInfoObj = srcObj.GetType().GetProperty(propertyName);

        if (propInfoObj == null)
            return null;

        // Get the value from property.
        object srcValue = srcObj
                  .GetType()
                  .InvokeMember(propInfoObj.Name,
                          BindingFlags.GetProperty,
                          null,
                          srcObj,
                          null);

        return srcValue;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="srcObj"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static string GetPropertyStringValue(this object srcObj, string propertyName)
    {
        try
        {
            object value = GetPropertyValue(srcObj, propertyName);
            if (value == null)
                return String.Empty;
            else
                return value.ToString();

        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
    /// <summary>
    /// var propertyName = GetPropertyName(() => myObject.AProperty); // returns "AProperty"
    /// http://stackoverflow.com/questions/3661824/get-string-name-of-property-using-reflection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyExpression"></param>
    /// <returns></returns>
    public static string GetPropertyName<T>(this Expression<Func<T>> propertyExpression)
    {
        return (propertyExpression.Body as MemberExpression).Member.Name;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="srcObj"></param>
    /// <param name="properties"></param>
    /// <returns></returns>
    public static bool GetPropertyNames(this object srcObj, List<string> properties)
    {
        properties = new List<string>();

        try
        {
            PropertyInfo[] srcProperties = srcObj.GetType().GetProperties();

            if (srcProperties == null)
                return false;

            foreach (PropertyInfo srcProperty in srcProperties)
                properties.Add(srcProperty.Name);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    //
    #endregion reflection

    #region File operations
    //
    /// <summary>
    /// Gets the search results.
    /// http://stackoverflow.com/questions/571964/automatic-cookie-handling-c-net-httpwebrequesthttpwebresponse
    /// http://stackoverflow.com/questions/9603093/407-proxy-authentication-required-in-c-sharp
    /// </summary>
    /// <param name="url">The Url.</param>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <returns>xml results</returns>
    public static string GetWebConent(string url)
    {
        /*
            Make sure you add this to your config:
            <system.net>
                <defaultProxy useDefaultCredentials="true" />
            </system.net>
        */

        System.IO.Stream st = null;
        System.IO.StreamReader sr = null;

        try
        {
            // make a Web request
            System.Net.HttpWebRequest req = HttpWebRequest.Create(url) as HttpWebRequest;
            /*
            if (cookieJar != null)
                req.CookieContainer = cookieJar;
            */

            // get the response and read from the result stream
            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;

            st = resp.GetResponseStream();
            sr = new System.IO.StreamReader(st);

            // read all the text in it
            return sr.ReadToEnd();
        }
        catch
        {
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
    /// <summary>
    /// http://stackoverflow.com/questions/876473/is-there-a-way-to-check-if-a-file-is-in-use
    /// </summary>
    /// <returns></returns>
    public static bool IsFileLocked(this string filename)
    {
        if (!File.Exists(filename))
            return false;

        FileInfo file = new FileInfo(filename);
        FileStream stream = null;

        try
        {
            stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }
        catch (IOException)
        {
            //the file is unavailable because it is:
            //still being written to
            //or being processed by another thread
            //or does not exist (has already been processed)
            return true;
        }
        finally
        {
            if (stream != null)
                stream.Close();
        }

        //file is not locked
        return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static bool DeleteFile(this string filename)
    {
        try
        {
            if (!File.Exists(filename))
                return true;

            if (filename.IsFileLocked())
                return false;

            File.Delete(filename);
            string strCmdText;
            //strCmdText = String.Concat("DEL ", newReport);
            strCmdText = String.Format("/c sdelete -p 1 -s -z -q -a '{0}'", filename);
            Process p = new Process();
            ProcessStartInfo psi = new ProcessStartInfo("CMD.exe", strCmdText);
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            p.StartInfo = psi;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="compressed"></param>
    public static void UnzipHere(string compressed)
    {
        FileInfo fileToDecompress = new FileInfo(compressed);

        using (FileStream originalFileStream = fileToDecompress.OpenRead())
        {
            string currentFileName = fileToDecompress.FullName;
            string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

            using (FileStream decompressedFileStream = File.Create(newFileName))
            {
                using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                {
                    decompressionStream.CopyTo(decompressedFileStream);
                    //Console.WriteLine("Decompressed: {0}", fileToDecompress.Name);
                }
            }

        }
    }
    /// <summary>
    /// only used if you don't have access to 4.5
    /// http://stackoverflow.com/questions/3985795/gzipstream-copyto-alternate-and-easy-way-in-net-3-5
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public static long CopyTo(this Stream source, Stream destination)
    {
        byte[] buffer = new byte[2048];
        int bytesRead;
        long totalBytes = 0;
        while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
        {
            destination.Write(buffer, 0, bytesRead);
            totalBytes += bytesRead;
        }
        return totalBytes;
    }
    /// <summary>
    /// http://stackoverflow.com/questions/6101367/how-to-count-lines-fast
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="skipFirst"></param>
    /// <returns></returns>
    public static long CountLines(this string filename, bool skipFirst = false)
    {
        long count = 0;
        using (StreamReader r = new StreamReader(filename))
        {
            string line;
            while ((line = r.ReadLine()) != null)
                count++;
        }

        return skipFirst ? count-- : count;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static String ExecuteBatch(string command)
    {
        int exitCode;
        ProcessStartInfo processInfo;
        Process process;

        processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;
        /*
        // *** Redirect the output ***
        processInfo.RedirectStandardError = true;
        processInfo.RedirectStandardOutput = true;
        */

        process = Process.Start(processInfo);
        process.WaitForExit();

        // *** Read the streams ***
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        exitCode = process.ExitCode;

        /*
        Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
        Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
        Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
        */

        string res = process.StandardOutput.ReadToEnd();
        process.Close();
        return res;
    }
    /// <summary>
    ///  https://msdn.microsoft.com/en-us/library/jj155757.aspx
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="text"></param>
    /// <param name="appendNewLine"></param>
    /// <returns></returns>
    public static async Task WriteTextAsync(string filePath, string text, bool appendNewLine = true)
    {
        if (appendNewLine)
            text = String.Concat(text, Environment.NewLine);
        byte[] encodedText = Encoding.Unicode.GetBytes(text);

        using (FileStream sourceStream = new FileStream(filePath,
            FileMode.Append, FileAccess.Write, FileShare.None,
            bufferSize: 4096, useAsync: true))
        {
            await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        };
    }
    //
    #endregion File operations
    /*
    #region SheetObject
    /// <summary>
    /// Useful after sorting and re-arranging a list of SheetObjects to prepare them for writing
    /// </summary>
    /// <param name="originalList"></param>
    /// <param name="startIndex"></param>
    public static void ReIndex(this IEnumerable<SheetObject> originalList, int startIndex = 2)
    {
        int idx = startIndex;
        foreach (var item in originalList)
            item.Index = idx++;
    }
    #endregion SheetObject
    */
}