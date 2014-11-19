using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtension
    {
        public static string ToTitleCase(this string value)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
        }

        public static string ToUpperFirstChar(this string value)
        {
            if (value.Length > 1)
            {
                return char.ToUpper(value[0]) + value.Substring(1).ToLower();
            }

            return value.ToUpper();
        }

        public static string Replace<TKey, TValue>(this string value, IDictionary<TKey, TValue> rules)
        {
            foreach (var rule in rules)
            {
                value = value.Replace(rule.Key.ToString(), rule.Value.ToString());
            }
            return value;
        }

        public static string Replace(this string value, IDictionary<string, string> rules)
        {
            return rules.Aggregate(value, (current, rule) => current.Replace(rule.Key, rule.Value));
        }

        public static string PregReplace(this string value, string pattern, string newValue)
        {
            return Regex.Replace(value, pattern, newValue);
        }

        public static string PregReplace(this string value, IDictionary<string, string> rules)
        {
            return rules.Aggregate(value, (current, rule) => current.PregReplace(rule.Key, rule.Value));
        }

        public static bool MatchIgnoreCase(this string value, IEnumerable<string> rules)
        {
            return rules.Any(rule => StringComparer.InvariantCultureIgnoreCase.Compare(value, rule) == 0);
        }

        public static int IndexOfAny(this string value, IEnumerable<string> rules, StringComparison comparison = StringComparison.CurrentCulture)
        {
            var index = -1;

            foreach (var rule in rules)
            {
                index = value.IndexOf(rule, comparison);
                if (index > -1)
                {
                    break;
                }
            }

            return index;
        }
    }
}
