using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimeZoneMapper.Tests
{
    [TestClass]
    public class TimeZoneMapTests
    {
        [TestMethod]
        public void DefaultValuesMapper_ReturnsCorrectTimeZoneInfo()
        {
            var mapper = TimeZoneMap.DefaultValuesTZMapper;
            var actual = mapper.MapTZID("Europe/Amsterdam");
            var expected = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OnlineValuesMapper_ReturnsCorrectTimeZoneInfo()
        {
            var mapper = TimeZoneMap.OnlineValuesTZMapper;
            var actual = mapper.MapTZID("Europe/Amsterdam");
            var expected = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OnlineWithFallbackValuesMapper_ReturnsCorrectTimeZoneInfo()
        {
            var mapper = TimeZoneMap.OnlineWithFallbackValuesTZMapper;
            var actual = mapper.MapTZID("Europe/Amsterdam");
            var expected = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DefaultValuesMapper_ReturnsUTCTimeZoneInfo()
        {
            var mapper = TimeZoneMap.DefaultValuesTZMapper;
            var actual = mapper.MapTZID("Etc/GMT");
            Assert.AreEqual(TimeZoneInfo.Utc, actual);
        }

        [TestMethod]
        public void OnlineValuesMapper_ReturnsUTCTimeZoneInfo()
        {
            var mapper = TimeZoneMap.OnlineValuesTZMapper;
            var actual = mapper.MapTZID("Etc/GMT");
            Assert.AreEqual(TimeZoneInfo.Utc, actual);
        }

        [TestMethod]
        public void GetAvailableTZIDsContainsAtLeastEtcGMT()
        {
            Assert.IsTrue(TimeZoneMap.DefaultValuesTZMapper.GetAvailableTZIDs().Contains("Etc/GMT", StringComparer.OrdinalIgnoreCase));
            Assert.IsTrue(TimeZoneMap.OnlineValuesTZMapper.GetAvailableTZIDs().Contains("Etc/GMT", StringComparer.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void GetAvailableTimeZonesContainsAtLeastCET()
        {
            Assert.IsTrue(TimeZoneMap.DefaultValuesTZMapper.GetAvailableTimeZones().Select(t => t.Id).Contains("W. Europe Standard Time", StringComparer.OrdinalIgnoreCase));
            Assert.IsTrue(TimeZoneMap.OnlineValuesTZMapper.GetAvailableTimeZones().Select(t => t.Id).Contains("W. Europe Standard Time", StringComparer.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void MapperIsNotCaseSensitive()
        {
            var mapper = TimeZoneMap.DefaultValuesTZMapper;
            var expected = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

            Assert.AreEqual(expected, mapper.MapTZID("europe/amsterdam"));
            Assert.AreEqual(expected, mapper.MapTZID("EUROPE/AMSTERDAM"));
            Assert.AreEqual(expected, mapper.MapTZID("Europe/Amsterdam"));
        }
    }
}
