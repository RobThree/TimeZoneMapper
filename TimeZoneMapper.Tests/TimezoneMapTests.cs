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
        public void OnlineWithSpecificFallbackValuesMapper_ReturnsCorrectFallbackMapper()
        {
            var mapper = TimeZoneMap.CreateOnlineWithSpecificFallbackValuesTZMapper(new Uri("http://example.com/test.xml"), new CustomValuesTZMapper("testfiles/testcldr.xml", Encoding.UTF8));
            Assert.AreEqual("zyx.xyz", mapper.Version);
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
        public void TryMapTZIDReturnsExpectedValues()
        {
            var mapper = TimeZoneMap.DefaultValuesTZMapper;
            var expected = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

            Assert.IsTrue(mapper.TryMapTZID("Europe/Amsterdam", out TimeZoneInfo actual));
            Assert.AreEqual(expected, actual);

            Assert.IsFalse(mapper.TryMapTZID("Foo/Bar", out actual));
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void CustomTZMapperStringConstructorPassingXML()
        {
            var mapper = new CustomValuesTZMapper(File.ReadAllText("testfiles/testcldr.xml"));

            Assert.AreEqual(TimeZoneInfo.FindSystemTimeZoneById("UTC"), mapper.MapTZID("Test/A"));
            Assert.AreEqual(TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time"), mapper.MapTZID("Test/B"));
        }

        [TestMethod]
        public void SpaceSeparatedTimeZonesAreParsedCorrectly()
        {
            var mapper = new CustomValuesTZMapper(File.ReadAllText("testfiles/testcldr.xml"));

            Assert.AreEqual(TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time"), mapper.MapTZID("America/Anchorage"));
            Assert.AreEqual(TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time"), mapper.MapTZID("America/Juneau"));
            Assert.AreEqual(TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time"), mapper.MapTZID("America/Nome"));
            Assert.AreEqual(TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time"), mapper.MapTZID("America/Sitka"));
            Assert.AreEqual(TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time"), mapper.MapTZID("America/Yakutat"));
        }

        [TestMethod]
        public void CustomTZMapperStringConstructorPassingPath()
        {
            var mapper = new CustomValuesTZMapper("testfiles/testcldr.xml", Encoding.UTF8);

            Assert.AreEqual(TimeZoneInfo.FindSystemTimeZoneById("UTC"), mapper.MapTZID("Test/A"));
            Assert.AreEqual(TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time"), mapper.MapTZID("Test/B"));
        }

        [TestMethod]
        public void CustomTZMapperStringConstructorPassingStream()
        {
            using (var stream = File.Open("testfiles/testcldr.xml", FileMode.Open, FileAccess.Read, FileShare.Read))
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
            var mapper = new CustomValuesTZMapper("testfiles/testcldr.xml", Encoding.UTF8);
            mapper.MapTZID("XXX");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException), "A TZID in the CLDR that doesn't map to an existing TimeZone should throw since it shouldn't be in TZMapper")]
        public void CustomTZMapperDoesntContainNonExistingTimeZones()
        {
            var mapper = new CustomValuesTZMapper("testfiles/testcldr.xml", Encoding.UTF8);
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
            var mapper = new CustomValuesTZMapper("testfiles/testcldr.xml", Encoding.UTF8);

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

        [TestMethod]
        public void EnsureResourceFileIsValid()
        {
            // We take the ACTUAL resourcefile and load it in the StrictTestMapper which will throw on duplicate
            // keys and/or non-existing timezone ID's. This should NOT throw any exceptions.
            var mapper = new StrictTestMapper(
                xmldata: File.ReadAllText("../../../TimeZoneMapper/ResourceFiles/windowsZones.xml"),
                throwOnDuplicateKey: true,
                throwOnNonExisting: true
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TimeZoneNotFoundException))]
        public void ConstructorThrowsOnNonExistingTimeZoneIdWhenSpecified()
        {
            // We take a test resource file with a non-existing TimeZoneId (i.e. .Net won't recognize the TimeZoneId)
            // which should then throw a TimeZoneNotFoundException.
            var mapper = new StrictTestMapper(
                xmldata: File.ReadAllText("testfiles/nonexisting.xml"),
                throwOnDuplicateKey: true,
                throwOnNonExisting: true
            );
        }

        [TestMethod]
        public void ConstructorDoesNotThrowOnNonExistingTimeZoneIdWhenSpecified()
        {
            // We take a test resource file with a non-existing TimeZoneId (i.e. .Net won't recognize the TimeZoneId)
            // which should NOT throw a TimeZoneNotFoundException because we set throwOnNonExisting to false.
            var mapper = new StrictTestMapper(
                xmldata: File.ReadAllText("testfiles/nonexisting.xml"),
                throwOnDuplicateKey: true,
                throwOnNonExisting: false
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorThrowsOnDuplicateKeyWhenSpecified()
        {
            // We take a test resource file with a duplicate TimeZoneId which should then throw an ArgumentException.
            var mapper = new StrictTestMapper(
                xmldata: File.ReadAllText("testfiles/duplicatekey.xml"),
                throwOnDuplicateKey: true,
                throwOnNonExisting: true
            );
        }

        [TestMethod]
        public void ConstructorDoesNotThrowOnDuplicateKeyWhenSpecified()
        {
            // We take a test resource file with a duplicate TimeZoneId which should NOT throw an ArgumentException
            // because we set throwOnDuplicateKey to false.
            var mapper = new StrictTestMapper(
                xmldata: File.ReadAllText("testfiles/duplicatekey.xml"),
                throwOnDuplicateKey: false,
                throwOnNonExisting: true
            );
        }
    }

    public class StrictTestMapper : BaseTZMapper, ITZMapper
    {
        public StrictTestMapper(string xmldata, bool throwOnDuplicateKey, bool throwOnNonExisting)
            : base(xmldata, throwOnDuplicateKey, throwOnNonExisting) { }
    }
}
