namespace TimeZoneMapper.TZMappers
{
    using System;
    using System.Net;

    /// <summary>
    ///     Provides TimeZoneID mapping based on a current (&quot;dynamic&quot;) resource.
    /// </summary>
    public sealed class OnlineValuesTZMapper : BaseTZMapper, TimeZoneMapper.TZMappers.ITZMapper
    {
        /// <summary>
        /// Default URL used for <see cref="OnlineValuesTZMapper"/>
        /// </summary>
        public const string DEFAULTRESOURCEURL = "http://unicode.org/repos/cldr/trunk/common/supplemental/windowsZones.xml";

        /// <summary>
        /// Initializes a new instance of an <see cref="OnlineValuesTZMapper"/> with default timeout of 5 seconds and 
        /// <see cref="DEFAULTRESOURCEURL"/> as resourceURL.
        /// </summary>
        public OnlineValuesTZMapper()
            : this(5000) { }


        /// <summary>
        /// Initializes a new instance of an <see cref="OnlineValuesTZMapper"/> with the specified timeout and 
        /// <see cref="DEFAULTRESOURCEURL"/> as resourceURL.
        /// </summary>
        /// <param name="timeout">The length of time, in milliseconds, before the request times out.</param>
        public OnlineValuesTZMapper(int timeout)
            : this(timeout, DEFAULTRESOURCEURL) { }

        /// <summary>
        /// Initializes a new instance of an <see cref="OnlineValuesTZMapper"/> with the specified timeout and 
        /// resourceURL.
        /// </summary>
        /// <param name="timeout">The length of time, in milliseconds, before the request times out.</param>
        /// <param name="resourceurl">The URL to use when retrieving CLDR data.</param>
        public OnlineValuesTZMapper(int timeout, string resourceurl)
            : this(timeout, new Uri(resourceurl, UriKind.Absolute)) { }

        /// <summary>
        /// Initializes a new instance of an <see cref="OnlineValuesTZMapper"/> with the specified timeout and 
        /// resourceURI.
        /// </summary>
        /// <param name="timeout">The length of time, in milliseconds, before the request times out.</param>
        /// <param name="resourceuri">The URI to use when retrieving CLDR data.</param>
        public OnlineValuesTZMapper(int timeout, Uri resourceuri)
            : base(new TimedWebClient(timeout).DownloadString(resourceuri)) { }


        /// <summary>
        /// Simple "wrapper class" providing timeouts.
        /// </summary>
        private class TimedWebClient : WebClient
        {
            public int Timeout { get; set; }

            public TimedWebClient(int timeout)
            {
                this.Timeout = timeout;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var wr = base.GetWebRequest(address);
                wr.Timeout = this.Timeout;
                return wr;
            }
        }
    }
}