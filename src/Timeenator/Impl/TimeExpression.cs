using System;
using System.Text.RegularExpressions;

namespace Timeenator.Impl
{
    /// <summary>
    /// Exrepssion from Grafana, examples:
    /// time > 1455354920s and time < 1455376521s
    /// time > now() - 6h
    /// </summary>
    public class TimeExpression
    {
        private string _expression;
        public DateTime? From { get; private set; }
        public DateTime? To { get; private set; }

        public TimeExpression(string expression)
        {
            _expression = expression;
            ParseTimeExpression();
        }

        private void ParseTimeExpression()
        {
            if (string.IsNullOrEmpty(_expression)) return;
            string[] times = _expression.Split(new [] {" and "}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var time in times)
            {
                ParseSingleTimeExpression(time);
            }
        }

        private void ParseSingleTimeExpression(string time)
        {
            time = time.Replace(" ", "");
            var match = Regex.Match(time, "time([<>])");
            if (match.Success)
            {
                DateTime? dateTime;
                if (time.Contains("now()"))
                {
                    dateTime = ParseRelativeTime(time);
                }
                else
                {
                    dateTime = ParseFixTime(time);
                }
                if (match.Groups[1].Value == ">")
                {
                    From = dateTime;
                }
                else
                {
                    To = dateTime;
                }
            }
        }

        private DateTime? ParseFixTime(string time)
        {
            var match = Regex.Match(time, "time[<>](\\d+)s");
            if (match.Success)
            {
                long number = long.Parse(match.Groups[1].Value);

                return number.FromSecondsAfter1970ToDateTime();
            }

            return null;
        }

        private DateTime? ParseRelativeTime(string time)
        {
            var match = Regex.Match(time, $"time[<>]now\\(\\)\\-(\\d+{StringExtensions.TimeExpression})");
            if (match.Success)
            {
                return DateTime.Now - match.Groups[1].Value.ToTimeSpan();
            }
            return null;
        }
    }
}
