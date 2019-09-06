using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace FixSystemTime
{
    internal class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SystemTime st);

        [StructLayout(LayoutKind.Sequential)]
        public struct SystemTime
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }

        public static DateTime GetTime()
        {
            DateTime dateTime = DateTime.MinValue;
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://www.google.com");
            request.Method = "GET";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
            request.ContentType = "application/x-www-form-urlencoded";
            request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string todaysDates = response.Headers["date"];

                dateTime = DateTime.ParseExact(todaysDates, "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                    System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat, System.Globalization.DateTimeStyles.AssumeUniversal);
            }

            return dateTime;
        }

        private static int ChangeHour()
        {
            var text = File.ReadAllText("changeHour");
            return int.Parse(text);
        }

        private static void Main()
        {
            var date = GetTime();
            var st = new SystemTime
            {
                wYear = (short)date.Year,
                wMonth = (short)date.Month,
                wDay = (short)date.Day,
                wHour = (short)date.Hour,
                wMinute = (short)date.Minute,
                wSecond = (short)date.Second,
                wDayOfWeek = (short)date.DayOfWeek,
                wMilliseconds = (short)date.Millisecond
            };
            st.wHour = (short) (st.wHour + ChangeHour());

            if (st.wHour == 0)
                st.wHour = 23;
            else
                st.wHour = (short) (st.wHour - 1);

            SetSystemTime(ref st); // invoke this method.
        }
    }
}
