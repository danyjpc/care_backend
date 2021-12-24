using System;
using System.Text.RegularExpressions;
using Serilog;

namespace care_core.util
{
    public class StringMatcher
    {
        public static Boolean isDate(String s)
        {
            //Log.Error(s);
            if (s == null)
                return false;

            //format YYYY-MM-DD
            Regex regex = new Regex(@"^[0-9]{4}-[0-9]{2}-[0-9]{2}$");

            Match match = regex.Match(s);
            //Log.Error(match.Success.ToString());

            return match.Success;
        }
    }
}