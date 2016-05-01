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

namespace Timeenator.Impl.Scientific
{
    public class TemperatureMath
    {
        private const double a1 = 7.5;
        private const double b1 = 237.3;
        //Für T < 0 über Eis
        private const double a2 = 9.5;
        private const double b2 = 265.5;
        private const double R = 8314.3; // J/kmol*K
        private const double mw = 18.016; // kg

        public TemperatureMath(double temperature, double relativeHumidity)
        {
            Temperature = temperature;
            RelativeHumidity = relativeHumidity;

            //Berechnen

            double a = Temperature > 0 ? a1 : a2;
            double b = Temperature > 0 ? b1 : b2;
            double TK = Temperature + 273.15;


            SättigungsDampfDruck = 6.1078*Math.Pow(10, (a*Temperature)/(b + Temperature));
            DampfDruck = RelativeHumidity/100*SättigungsDampfDruck;

            double v = Math.Log10(DampfDruck/6.1078);

            Taupunkt = b*v/(a - v);
            AbsoluteHumitity = 100000*mw/R*DampfDruck/TK;
            SimpleAbsoluteHumitity = (6.112*Math.Pow(Math.E, (17.67*Temperature)/(Temperature + 243.5))*RelativeHumidity*
                                      2.1674)
                                     /(273.15 + Temperature);

            Humidex = Temperature + (3.39556)*Math.Exp(19.8336 - 5417.75/(Taupunkt + 273.15)) - 5.5556;
            HeatIndex = ComputeHeatIndex(temperature, relativeHumidity);
        }

       public double ConvertCtoF(double c)
        {
            return c * 1.8 + 32;
        }

        public double ConvertFtoC(double f)
        {
            return (f - 32) / 1.8;
        }

        double ComputeHeatIndex(double temperature, double percentHumidity)
        {
            // Using both Rothfusz and Steadman's equations
            // http://www.wpc.ncep.noaa.gov/html/heatindex_equation.shtml
            double hi;

            temperature = ConvertCtoF(temperature);

            hi = 0.5 * (temperature + 61.0 + ((temperature - 68.0) * 1.2) + (percentHumidity * 0.094));

            if (hi > 79)
            {
                hi = -42.379 +
                         2.04901523 * temperature +
                        10.14333127 * percentHumidity +
                        -0.22475541 * temperature * percentHumidity +
                        -0.00683783 * Math.Pow(temperature, 2) +
                        -0.05481717 * Math.Pow(percentHumidity, 2) +
                         0.00122874 * Math.Pow(temperature, 2) * percentHumidity +
                         0.00085282 * temperature * Math.Pow(percentHumidity, 2) +
                        -0.00000199 * Math.Pow(temperature, 2) * Math.Pow(percentHumidity, 2);

                if ((percentHumidity < 13) && (temperature >= 80.0) && (temperature <= 112.0))
                    hi -= ((13.0 - percentHumidity) * 0.25) * Math.Sqrt((17.0 - Math.Abs(temperature - 95.0)) * 0.05882);

                else if ((percentHumidity > 85.0) && (temperature >= 80.0) && (temperature <= 87.0))
                    hi += ((percentHumidity - 85.0) * 0.1) * ((87.0 - temperature) * 0.2);
            }

            return ConvertFtoC(hi);
        }

        public double Humidex { get; private set; }

        /// <summary>
        /// http://www.wpc.ncep.noaa.gov/html/heatindex_equation.shtml
        /// </summary>
        public double HeatIndex { get; private set; }
        public double Temperature { get; }
        public double RelativeHumidity { get; }

        /// <summary>
        ///     in C°
        /// </summary>
        public double Taupunkt { get; }

        /// <summary>
        ///     Absolute Feuchtigkeit in g/m3
        /// </summary>
        public double AbsoluteHumitity { get; private set; }

        /// <summary>
        ///     https://carnotcycle.wordpress.com/2012/08/04/how-to-convert-relative-humidity-to-absolute-humidity/
        ///     0.1% genau
        /// </summary>
        public double SimpleAbsoluteHumitity { get; private set; }

        /// <summary>
        ///     in hPa
        /// </summary>
        public double SättigungsDampfDruck { get; }

        /// <summary>
        ///     in hPa
        /// </summary>
        public double DampfDruck { get; }
    }
}