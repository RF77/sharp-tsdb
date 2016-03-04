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
                    return TimeSpan.FromDays(number*7);
                case "M":
                    return TimeSpan.FromDays(number*30);
                case "y":
                    return TimeSpan.FromDays(number*365);
            }
            throw new ArgumentException($"expression {expression} is invalid");
        }
    }
}