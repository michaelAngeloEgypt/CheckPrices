using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

    public static class Extensions
    {
        #region String functions
        //
        #region unformat
        /// <summary>
        /// 
        /// </summary>
        /// <see cref="http://www.mikeobrien.net/blog/parseexact-for-strings/"/>
        /// <param name="data"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string[] ParseExact(this string data, string format)
        {
            return ParseExact(data, format, false);
        }
        public static string[] ParseExact(this string data, string format, bool ignoreCase)
        {
            string[] values;

            if (TryParseExact(data, format, out values, ignoreCase))
                return values;
            else
                throw new ArgumentException("Format not compatible with value.");
        }
        public static bool TryExtract(this string data, string format, out string[] values)
        {
            return TryParseExact(data, format, out values, false);
        }
        public static bool TryParseExact(this string data, string format, out string[] values, bool ignoreCase)
        {
            int tokenCount = 0;
            format = Regex.Escape(format).Replace("\\{", "{");

            for (tokenCount = 0; ; tokenCount++)
            {
                string token = string.Format("{{{0}}}", tokenCount);
                if (!format.Contains(token)) break;
                format = format.Replace(token,
                    string.Format("(?'group{0}'.*)", tokenCount));
            }

            RegexOptions options =
                ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;

            Match match = new Regex(format, options).Match(data);

            if (tokenCount != (match.Groups.Count - 1))
            {
                values = new string[] { };
                return false;
            }
            else
            {
                values = new string[tokenCount];
                for (int index = 0; index < tokenCount; index++)
                    values[index] =
                        match.Groups[string.Format("group{0}", index)].Value;
                return true;
            }
        }
        #endregion unformat

        #region Excel
        /// <summary>
        /// used for CSV numbers to be displayed as-is in Excel
        /// </summary>
        /// <returns></returns>
        public static string FormatAsExcelLiteral(this string cellValue)
        {
            string textFieldTemplate = "\"=\"\"{0}\"\"\"";
            return String.Format(textFieldTemplate, cellValue);
        }
        /// <summary>
        /// used for CSV numbers to be displayed as-is in Excel
        /// </summary>
        /// <returns></returns>
        public static string RemoveExcelLiteralFormat(this string cellValue)
        {
            string textFieldTemplate = "\"=\"\"{0}\"\"\"";
            string prefix = textFieldTemplate.Before("{0}");
            string suffix = textFieldTemplate.After("{0}");

            return cellValue.Replace(prefix, "").Replace(suffix, "");
        }
        /// <summary>
        /// useful for Excel ranges
        /// <see cref="http://stackoverflow.com/questions/181596/how-to-convert-a-column-number-eg-127-into-an-excel-column-eg-aa"/>
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
        /// </summary>
        /// <see cref="http://stackoverflow.com/questions/181596/how-to-convert-a-column-number-eg-127-into-an-excel-column-eg-aa"/>
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
        #endregion Excel

        #region conversions
        //
        /// <summary>
        /// <see cref="http://stackoverflow.com/questions/372865/path-combine-for-urls"/>
        /// </summary>
        /// <param name="url1"></param>
        /// <param name="url2"></param>
        /// <returns></returns>
        public static string UrlCombine(string url1, string url2)
        {
            if (url1.Length == 0)
            {
                return url2;
            }

            if (url2.Length == 0)
            {
                return url1;
            }

            url1 = url1.TrimEnd('/', '\\');
            url2 = url2.TrimStart('/', '\\');

            return string.Format("{0}/{1}", url1, url2);
        }
        private static string EnumToFriendlyName(string defaultName)
        {
            var sb = new StringBuilder(defaultName);

            for (int i = 1; i < sb.Length; ++i)
                if (char.IsUpper(sb[i]))
                {
                    sb.Insert(i, ' ');
                    ++i;
                }

            return sb.ToString();
        }
        /// <summary>
        /// </summary>
        /// <see cref="http://stackoverflow.com/questions/206717/how-do-i-replace-multiple-spaces-with-a-single-space-in-c"/>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ReplaceConsecutiveCopiesToOneInstance(this string input, string victim)
        {
            return string.Join(victim, input.Split(new string[] { victim }, StringSplitOptions.RemoveEmptyEntries));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <see cref="http://social.msdn.microsoft.com/Forums/vstudio/en-US/354ed9c9-eea9-47c4-afaf-443182021d94/how-to-convert-current-date-into-string-yyyymmdd-like-20061113?forum=csharpgeneral"/>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string ToYMD(this DateTime src)
        {
            return src.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <see cref="http://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net"/>
        /// <param name="byteCount"></param>
        /// <param name="num"></param>
        /// <param name="unit"></param>
        public static void BytesAsReadableDetails(this long byteCount, out double num, out string unit)
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
        /// <summary>
        /// <see cref="http://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net"/>
        /// </summary>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        static String BytesAsReadable(this long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <see cref="http://stackoverflow.com/questions/228038/best-way-to-reverse-a-string"/>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Reverse(this string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        /// <summary>
        /// removes illegal chars from a propsed filename, but doesn't validate that it has an extension
        /// </summary>
        /// <param name="badFilename"></param>
        /// <returns></returns>
        public static string ConvertToStandardFilename(this string badFilename)
        {
            string res = badFilename;

            List<char> invalidChars = new List<char>() { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
            invalidChars.AddRange(Path.GetInvalidFileNameChars());
            invalidChars.ForEach(c => res = res.Replace(c, '_'));

            res = Encoding.ASCII.GetString(
            Encoding.Convert(
                Encoding.UTF8,
                Encoding.GetEncoding(
                    Encoding.ASCII.EncodingName,
                    new EncoderReplacementFallback(string.Empty),
                    new DecoderExceptionFallback()
                    ),
                Encoding.UTF8.GetBytes(res)
            ));

            return res.Trim();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string ConvertToHex(decimal d)
        {
            int[] bits = decimal.GetBits(d);
            if (bits[3] != 0)
            {
                throw new ArgumentException();
            }
            return string.Format("{ 0:x8}{1:x8}{2:x8}",
                (uint)bits[2], (uint)bits[1], (uint)bits[0]);
        }
        /// <summary>
        /// Get the string slice between the two indexes.
        /// Inclusive for start index, exclusive for end index.
        /// The second parameter can be any integer. When it is negative, it means you start from the end and then count backwards.
        /// </summary>
        /// <see cref="http://www.dotnetperls.com/string-slice"/>
        public static string Slice(this string source, int start, int end)
        {
            if (end < 0) // Keep this for negative end support
            {
                end = source.Length + end;
            }
            int len = end - start;               // Calculate length
            return source.Substring(start, len); // Return Substring of length
        }
        /// <summary>
        /// <see cref="http://stackoverflow.com/questions/7256142/way-to-quickly-check-if-string-is-xml-or-json-in-c-sharp"/>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsJson(this string input)
        {
            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}")
                   || input.StartsWith("[") && input.EndsWith("]");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static string ToStandardElapsedFormat(this TimeSpan ts)
        {
            return String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
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
        #endregion conversions

        #region lookup
        /// <summary>
        /// Get string value between [first] a and [last] b.
        /// </summary>
        public static string Between(this string value, string a, string b)
        {
            /*
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
            */

            if (String.IsNullOrEmpty(value))
                return "";

            int start = value.IndexOf(a) + a.Length;
            if (value.StartsWith(a)) start++;
            int end = value.IndexOf(b, start);
            if (end == -1) return "";
            string result = value.Substring(start, end - start);
            return result;
        }
        /// <summary>
        /// Get string value before [first] a.
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
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startTag"></param>
        /// <param name="endTag"></param>
        /// <returns></returns>
        [Obsolete("use ExtractStringBetweenDelimiters() instead")]
        public static string ExtractStringBetweenTags(this string source, string startTag, string endTag, bool includeTags = false)
        {
            if (String.IsNullOrWhiteSpace(source))
                return String.Empty;

            int startIndex = source.IndexOf(startTag) + (!includeTags ? startTag.Length : 0);
            if (startIndex == -1)
                return String.Empty;

            int endIndex = source.IndexOf(endTag, startIndex);
            if (endIndex == -1)
                return String.Empty;

            endIndex += (includeTags ? endTag.Length : 0);
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
        public static int CommonChars(string left, string right)
        {
            return left.GroupBy(c => c)
                .Join(
                    right.GroupBy(c => c),
                    g => g.Key,
                    g => g.Key,
                    (lg, rg) => lg.Zip(rg, (l, r) => l).Count())
                .Sum();
        }
        public static string[] CommonString(string left, string right)
        {
            List<string> result = new List<string>();
            string[] rightArray = right.ToUpperInvariant().Split();
            string[] leftArray = left.ToUpperInvariant().Split();

            result.AddRange(rightArray.Where(r => leftArray.Any(l => l.StartsWith(r))));

            // must check other way in case left array contains smaller words than right array
            result.AddRange(leftArray.Where(l => rightArray.Any(r => r.StartsWith(l))));

            return result.Distinct().ToArray();
        }
        #endregion lookup
        //
        #endregion String functions
    }
