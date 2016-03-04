// /*******************************************************************************
//  * Copyright (c) 2016 by RF77 (https://github.com/RF77)
//  * All rights reserved. This program and the accompanying materials
//  * are made available under the terms of the Eclipse Public License v1.0
//  * which accompanies this distribution, and is available at
//  * http://www.eclipse.org/legal/epl-v10.html
//  *
//  * Contributors:
//  *    RF77 - initial API and implementation and/or initial documentation
//  *******************************************************************************/ 

using System;
using System.Text.RegularExpressions;

namespace Timeenator.Extensions
{
    /// <summary>
    ///     Exrepssion from Grafana, examples:
    ///     time > 1455354920s and time
    ///     < 1455376521s
    ///         time>
    ///         now() - 6h
    /// </summary>
    public class TimeExpression
    {
        private readonly string _expression;

        public TimeExpression(string expression)
        {
            _expression = expression;
            ParseTimeExpression();
        }

        public DateTime? From { get; private set; }
        public DateTime? To { get; private set; }

        private void ParseTimeExpression()
        {
            if (string.IsNullOrEmpty(_expression)) return;
            string[] times = _expression.Split(new[] {" and "}, StringSplitOptions.RemoveEmptyEntries);
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

                return number.FromSecondsAfter1970ToDateTimeUtc();
            }

            return null;
        }

        private DateTime? ParseRelativeTime(string time)
        {
            var match = Regex.Match(time, $"time[<>]now\\(\\)\\-(\\d+{StringExtensions.TimeExpression})");
            if (match.Success)
            {
                return DateTime.UtcNow - match.Groups[1].Value.ToTimeSpan();
            }
            return null;
        }
    }
}