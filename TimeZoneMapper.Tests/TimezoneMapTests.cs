using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using TimeZoneMapper.TZMappers;

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

        [TestMethod]
        public void CustomTZMapperStringConstructorPassingXML()
        {
            var mapper = new CustomValuesTZMapper(File.ReadAllText("testcldr.xml"));

            Assert.AreEqual(TimeZoneInfo.FindSystemTimeZoneById("UTC"), mapper.MapTZID("Test/A"));
            Assert.AreEqual(TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time"), mapper.MapTZID("Test/B"));
        }

        [TestMethod]
        public void CustomTZMapperStringConstructorPassingPath()
        {
            var mapper = new CustomValuesTZMapper("testcldr.xml", Encoding.UTF8);

            Assert.AreEqual(TimeZoneInfo.FindSystemTimeZoneById("UTC"), mapper.MapTZID("Test/A"));
            Assert.AreEqual(TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time"), mapper.MapTZID("Test/B"));
        }

        [TestMethod]
        public void CustomTZMapperStringConstructorPassingStream()
        {
            using (var stream = File.Open("testcldr.xml", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var mapper = new CustomValuesTZMapper(stream);

                Assert.AreEqual(TimeZoneInfo.FindSystemTimeZoneById("UTC"), mapper.MapTZID("Test/A"));
                Assert.AreEqual(TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time"), mapper.MapTZID("Test/B"));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException), "A non-existing TZID should throw")]
        public void CustomTZMapperThrowsOnNonExistingTZID()
        {
            var mapper = new CustomValuesTZMapper("testcldr.xml", Encoding.UTF8);
            mapper.MapTZID("XXX");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException), "A TZID in the CLDR that doesn't map to an existing TimeZone should throw since it shouldn't be in TZMapper")]
        public void CustomTZMapperDoesntContainNonExistingTimeZones()
        {
            var mapper = new CustomValuesTZMapper("testcldr.xml", Encoding.UTF8);
            mapper.MapTZID("Test/C");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException), "When a non-existing path is specified the CustomTZMapper should throw")]
        public void CustomTZMapperThrowsOnNonExistingFile()
        {
            var mapper = new CustomValuesTZMapper("XXX.xml", Encoding.UTF8);
        }

        [TestMethod]
        [ExpectedException(typeof(XmlException), "When invalid XML is specified the CustomTZMapper should throw")]
        public void CustomTZMapperThrowsOnInvalidXML()
        {
            var mapper = new CustomValuesTZMapper("<xxx>");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Valid XML, but not valid CLDR data should throw")]
        public void CustomTZMapperThrowsOnInvalidCLDR()
        {
            var mapper = new CustomValuesTZMapper("<foo/>");
        }

        [TestMethod]
        public void CustomTZMapperLoadsEmptyCLDRCorrectly()
        {
            var mapper = new CustomValuesTZMapper("<mapTimezones/>");
        }

        [TestMethod]
        public void VersionAttributesAreParsedCorrectly()
        {
            var mapper = new CustomValuesTZMapper("testcldr.xml", Encoding.UTF8);

            Assert.AreEqual("zyx", mapper.TZIDVersion);
            Assert.AreEqual("xyz", mapper.TZVersion);
            Assert.AreEqual("zyx.xyz", mapper.Version);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException), "A non-existing TZID should throw")]
        public void DefaultValuesTZMapperThrowsOnNonExistingTZID()
        {
            var mapper = TimeZoneMap.DefaultValuesTZMapper.MapTZID("XXX");
        }
    }
}
