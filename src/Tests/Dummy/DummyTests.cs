using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using FileDb.Impl;
using FileDb.Scripting;
using FluentAssertions;
using Nancy.Json;
using NUnit.Framework;
using Timeenator.Extensions;
using Timeenator.Extensions.Converting;
using Timeenator.Impl;

namespace Tests.Dummy
{
    [TestFixture, Ignore("Tests for manual execution")]
    public class DummyTests
    {

        [SetUp]
        public void Setup()
        {
           
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void ConvertPerformance()
        {
            int number = 1000000;
            List<float> list = new List<float>(number);
            var sw = Stopwatch.StartNew();
            for (float i = 0; i < number; i++)
            {
                list.Add(i);
            }
            sw.Stop();

            List<float> list2 = new List<float>(number);
            var sw2 = Stopwatch.StartNew();
            for (double i = 0; i < number; i++)
            {
                list2.Add(i.ToFloat());
            }
            sw2.Stop();


            List<double> list3 = new List<double>(number);
            var sw3 = Stopwatch.StartNew();
            for (long i = 0; i < number; i++)
            {
                list3.Add(i.ToDouble());
            }
            sw3.Stop();
        }

        [Test]
        public void ConverterTests()
        {
            int? test = 5;
            object t = test;
            float? result = t.ToType<float>();
            
        }

        [Test]
        public void ExpandoPerformance()
        {
            int number = 10000;
            var sw = Stopwatch.StartNew();
            dynamic e = new ExpandoObject();
            for (float i = 0; i < number; i++)
            {
                //e["A"] = i;
                //e["B"] = i;
                //e["C"] = i;
                e.A = i;
                e.B = i;
                e.C = i;

                double result = (float) e.A + (float) e.B;

            }
            sw.Stop();


        }

        [Test]
        public void ScriptTesting()
        {
            var sw = Stopwatch.StartNew();
            var result = ScriptingEngine.ExecuteTestAsync("1+2");
            sw.Stop();
            var sw2 = Stopwatch.StartNew();
            var result2 = ScriptingEngine.ExecuteTestAsync("4+2");
            sw2.Stop();
            var sw3 = Stopwatch.StartNew();
            var result3 =  ScriptingEngine.ExecuteTestAsync("4+5");
            sw3.Stop();

        }

        [Test]
        public void DynamicTesting()
        {
            DummyLambda(a => a);
            Dummy(3);
            dynamic otherEx = new ExpandoObject();
            otherEx.G = 3;
            int sdfa = otherEx.G;
            int number = 10000;
            var sw = Stopwatch.StartNew();
            for (double i = 0; i < number; i++)
            {
                var result = Add(i, i);
            }
            sw.Stop();
            var sw2 = Stopwatch.StartNew();
            for (double i = 0; i < number; i++)
            {
                var result = Add2(i, i);
            }
            sw2.Stop();
            for (double i = 0; i < number; i++)
            {
                var result = AddImplicit(i, i);
            }

            var sw3 = Stopwatch.StartNew();
            dynamic t = 4;
            var test = TestWithLambda(a => a + 2 + t, 4);
            var test2 = TestWithLambda2(a => (int)a + 2, 4);
            test = TestWithLambda(a => a + 2 + t, 4);
            test2 = TestWithLambda2(a => (int)a + 2, 4);
            sw3.Stop();
            dynamic tt = 3.4;
            var sw4 = Stopwatch.StartNew();
            dynamic ex = new ExpandoObject();
            ex.B = 2.2;
            var h = ex.B;
            sw4.Stop();
        }

        private int TestWithLambda(Func<dynamic, dynamic> testFunc, dynamic val)
        {
            return (int)testFunc(val);
        }
        private int TestWithLambda2(Func<float, int> testFunc, float val)
        {
            return testFunc(val);
        }

        private int DummyLambda(Func<dynamic, dynamic> testFunc)
        {
            return (int)testFunc(5);
        }

        private dynamic Add(dynamic a, dynamic b)
        {
            return a + b;
        }
        private dynamic Add2(dynamic a, dynamic b)
        {
            return a + b;
        }

        dynamic Dummy(dynamic a)
        {
            return a + 2;
        }
        private double AddImplicit(double a, double b)
        {
            return a + b;
        }

        [Test]
        public void DynamicSerieTests()
        {
            
        }

        [Test]
        public void BinaryDateTimePerformance()
        {
            DateTime now = DateTime.Now;
            long nowLong = 0;
            DateTime newDate = DateTime.MinValue;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                nowLong = now.ToBinary();
                newDate = DateTime.FromBinary(nowLong);
            }
            sw.Stop();
            newDate.Should().Be(now);
            //Zeit 729ms
        }

        [Test]
        public void Miliseconds1970DateTimePerformance()
        {
            DateTime now = DateTime.Now;
            long nowLong = 0;
            DateTime newDate = DateTime.MinValue;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                nowLong = now.ToMiliSecondsAfter1970();
                newDate = nowLong.FromMilisecondsAfter1970ToDateTime();
            }
            sw.Stop();
            //newDate.Should().Be(now);
            //Zeit 759ms
        }

        [Test]
        public void Miliseconds1970DateTimeUtcPerformance()
        {
            DateTime now = DateTime.UtcNow;
            long nowLong = 0;
            DateTime newDate = DateTime.MinValue;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                nowLong = now.ToMiliSecondsAfter1970Utc();
                newDate = nowLong.FromMilisecondsAfter1970ToDateTimeUtc();
            }
            sw.Stop();
            //newDate.Should().Be(now);
            //Zeit 759ms
        }

        [Test]
        public void FileTimeDateTimePerformance()
        {
            DateTime now = DateTime.Now;
            long nowLong = 0;
            DateTime newDate = DateTime.MinValue;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                nowLong = now.ToFileTimeUtc();
                newDate = DateTime.FromFileTimeUtc(nowLong);
            }
            sw.Stop();
            //newDate.Should().Be(now);
            //Zeit 793ms
        }

        [Test]
        public void FileTimeDateTimeUtcPerformance()
        {
            DateTime now = DateTime.UtcNow;
            long nowLong = 0;
            DateTime newDate = DateTime.MinValue;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                nowLong = now.ToFileTimeUtc();
                //var nowLong2 = now.ToFileTime();
                //var str = nowLong.ToString();
                //var lo = long.Parse(str);
                newDate = DateTime.FromFileTimeUtc(nowLong);
            }
            sw.Stop();
            newDate.Should().Be(now);
            //Zeit 793ms
        }

        [Test]
        public void StringDateTimeUtcPerformance()
        {
            DateTime now = DateTime.UtcNow;
            DateTime newDate = DateTime.MinValue;
            var s = new JavaScriptSerializer();
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                var str = s.Serialize(now);
                newDate = s.Deserialize<DateTime>(str);
            }
            sw.Stop();
            newDate.Should().Be(now);
            //Zeit 793ms
        }

        [Test]
        public void SecondsTimeUtcPerformance()
        {
            DateTime now = DateTime.UtcNow;
            DateTime newDate = DateTime.MinValue;
            var sw = Stopwatch.StartNew();
            UInt32 number = 0;
            for (int i = 0; i < 1000000; i++)
            {
                number = now.ToSecondsAfter1970Utc();
                //var nowLong2 = now.ToFileTime();
                //var str = nowLong.ToString();
                //var lo = long.Parse(str);
                newDate = number.FromSecondsAfter1970ToDateTimeUtc();
            }
            sw.Stop();
           // newDatl.Should().Be(now);            //Zeit 793ms
        }

        [Test]
        public void TicksDateTimePerformance()
        {
            DateTime now = DateTime.UtcNow;
            long nowLong = 0;
            DateTime newDate = DateTime.MinValue;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                nowLong = now.Ticks;
                newDate = DateTime.FromBinary(nowLong);
            }
            sw.Stop();
            //newDate.Should().Be(now);
            //Zeit 793ms
            var sommer = new DateTime(1850, 7, 1, 5, 0, 0);
            var winter = new DateTime(1850, 1, 1, 5, 0, 0);

            var somUtc = sommer.ToUniversalTime();
            var winUtc = winter.ToUniversalTime();

        }

        [Test]
        public void TestDateTimeAsUInt()
        {
            var now = DateTime.Now;
            var now2 = DateTime.UtcNow.ToFileTimeUtc();
            DateTime d = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime d2 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            var l1 = d.ToFileTimeUtc();
            var l2 = d2.ToFileTimeUtc();
            l1.Should().NotBe(l2);


        }

        [Test]
        public void CreateHausDb()
        {
            new DbManagement().CreateDb(@"c:\DBs\SharpTsdb", "Haus");
        }
    }
}