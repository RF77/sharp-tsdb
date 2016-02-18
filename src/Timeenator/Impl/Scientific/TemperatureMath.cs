using System;

namespace QueryLanguage.Scientific
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


            SättigungsDampfDruck = 6.1078 * Math.Pow(10, (a * Temperature) / (b + Temperature));
            DampfDruck = RelativeHumidity / 100 * SättigungsDampfDruck;

            double v = Math.Log10(DampfDruck / 6.1078);

            Taupunkt = b * v / (a - v);
            AbsoluteHumitity = 100000 * mw / R * DampfDruck / TK;
            SimpleAbsoluteHumitity = (6.112 * Math.Pow(Math.E, (17.67 * Temperature) / (Temperature + 243.5)) * RelativeHumidity * 2.1674)
                                     / (273.15 + Temperature);

            Humidex = Temperature + (3.39556) * Math.Exp(19.8336 - 5417.75 / (Taupunkt + 273.15)) - 5.5556;
        }

        public double Humidex { get; private set; }

        public double Temperature { get; }
        public double RelativeHumidity { get; }

        /// <summary>
        /// in C°
        /// </summary>
        public double Taupunkt { get; }

        /// <summary>
        /// Absolute Feuchtigkeit in g/m3
        /// </summary>
        public double AbsoluteHumitity { get; private set; }

        /// <summary>
        /// https://carnotcycle.wordpress.com/2012/08/04/how-to-convert-relative-humidity-to-absolute-humidity/
        /// 0.1% genau
        /// </summary>
        public double SimpleAbsoluteHumitity { get; private set; }

        /// <summary>
        /// in hPa
        /// </summary>
        public double SättigungsDampfDruck { get; }

        /// <summary>
        /// in hPa
        /// </summary>
        public double DampfDruck { get; }


        // Für T > 0
    }
}