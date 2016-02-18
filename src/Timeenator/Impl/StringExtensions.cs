using System;
using System.Text.RegularExpressions;

namespace FileDb.InterfaceImpl
{
    public static class StringExtensions
    {
        public const string TimeExpression = "[smhdwMy]";

        public static TimeSpan ToTimeSpan(this string expression)
        {
            var match = Regex.Match(expression, $"^(\\d+)({TimeExpression})$");
            if (match.Success == false)
            {
                throw new ArgumentException($"expression {expression} is invalid");
            }
            int number = int.Parse(match.Groups[1].Value);
            string type = match.Groups[2].Value;

            switch (type)
            {
                case "s":
                    return TimeSpan.FromSeconds(number);
                case "m":
                    return TimeSpan.FromMinutes(number);
                case "h":
                    return TimeSpan.FromHours(number);
                case "d":
                    return TimeSpan.FromDays(number);
                case "w":
                    return TimeSpan.FromDays(number * 7);
                case "M":
                    return TimeSpan.FromDays(number * 30);
                case "y":
                    return TimeSpan.FromDays(number*365);
            }
            throw new ArgumentException($"expression {expression} is invalid");
        }
    }
}