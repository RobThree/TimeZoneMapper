namespace TimeZoneMapper.TZMappers
{
    using System.Net;

    /// <summary>
    ///     Provides TimeZoneID mapping based on a current (&quot;dynamic&quot;) resource.
    /// </summary>
    public sealed class OnlineValuesTZMapper : BaseTZMapper, TimeZoneMapper.TZMappers.ITZMapper
    {
        private const string resourceuri = "http://unicode.org/repos/cldr/trunk/common/supplemental/windowsZones.xml";
        internal OnlineValuesTZMapper()
            : base(new WebClient().DownloadString(resourceuri)) { }
    }
}