namespace TimeZoneMapper.TZMappers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Cache;

    /// <summary>
    /// Provides TimeZoneID mapping based on a current (&quot;dynamic&quot;) resource.
    /// </summary>
    public sealed class OnlineValuesTZMapper : BaseTZMapper, ITZMapper
    {
        /// <summary>
        /// Default URL used for <see cref="OnlineValuesTZMapper"/>
        /// </summary>
        public const string DEFAULTRESOURCEURL = "http://unicode.org/repos/cldr/trunk/common/supplemental/windowsZones.xml";

        /// <summary>
        /// Default timeout for HTTP requests.
        /// </summary>
        public const int DEFAULTTIMEOUTMS = 5000;

        /// <summary>
        /// Default cache TTL time.
        /// </summary>
        public static readonly TimeSpan DEFAULTCACHETTL = TimeSpan.FromHours(24);

        /// <summary>
        /// Initializes a new instance of an <see cref="OnlineValuesTZMapper"/> with default timeout of 5 seconds and 
        /// <see cref="DEFAULTRESOURCEURL"/> as resourceURL.
        /// </summary>
        /// <remarks>
        /// By default, the data retrieved is cached for <see cref="DEFAULTCACHETTL">the default cache TTL time</see>
        /// in the user's temporary folder retrieved from <see cref="Path.GetTempPath"/>.
        /// </remarks>
        public OnlineValuesTZMapper()
            : this(TimeSpan.FromMilliseconds(DEFAULTTIMEOUTMS)) { }

        /// <summary>
        /// Initializes a new instance of an <see cref="OnlineValuesTZMapper"/> with the specified timeout and 
        /// <see cref="DEFAULTRESOURCEURL"/> as resourceURL.
        /// </summary>
        /// <param name="timeout">The length of time, in milliseconds, before the request times out.</param>
        /// <remarks>
        /// By default, the data retrieved is cached for <see cref="DEFAULTCACHETTL">the default cache TTL time</see>
        /// in the user's temporary folder retrieved from <see cref="Path.GetTempPath"/>.
        /// </remarks>
        public OnlineValuesTZMapper(int timeout)
            : this(TimeSpan.FromMilliseconds(timeout)) { }

        /// <summary>
        /// Initializes a new instance of an <see cref="OnlineValuesTZMapper"/> with the specified timeout and 
        /// <see cref="DEFAULTRESOURCEURL"/> as resourceURL.
        /// </summary>
        /// <param name="timeout">The length of time before the request times out.</param>
        /// <remarks>
        /// By default, the data retrieved is cached for <see cref="DEFAULTCACHETTL">the default cache TTL time</see>
        /// in the user's temporary folder retrieved from <see cref="Path.GetTempPath"/>.
        /// </remarks>
        public OnlineValuesTZMapper(TimeSpan timeout)
            : this(timeout, DEFAULTRESOURCEURL) { }

        /// <summary>
        /// Initializes a new instance of an <see cref="OnlineValuesTZMapper"/> with the specified timeout and 
        /// resourceURL.
        /// </summary>
        /// <param name="timeout">The length of time, in milliseconds, before the request times out.</param>
        /// <param name="resourceurl">The URL to use when retrieving CLDR data.</param>
        /// <remarks>
        /// By default, the data retrieved is cached for <see cref="DEFAULTCACHETTL">the default cache TTL time</see>
        /// in the user's temporary folder retrieved from <see cref="Path.GetTempPath"/>.
        /// </remarks>
        public OnlineValuesTZMapper(int timeout, string resourceurl)
            : this(TimeSpan.FromMilliseconds(timeout), resourceurl) { }

        /// <summary>
        /// Initializes a new instance of an <see cref="OnlineValuesTZMapper"/> with the specified timeout and 
        /// resourceURL.
        /// </summary>
        /// <param name="timeout">The length of time before the request times out.</param>
        /// <param name="resourceurl">The URL to use when retrieving CLDR data.</param>
        /// <remarks>
        /// By default, the data retrieved is cached for <see cref="DEFAULTCACHETTL">the default cache TTL time</see>
        /// in the user's temporary folder retrieved from <see cref="Path.GetTempPath"/>.
        /// </remarks>
        public OnlineValuesTZMapper(TimeSpan timeout, string resourceurl)
            : this(timeout, new Uri(resourceurl, UriKind.Absolute)) { }

        /// <summary>
        /// Initializes a new instance of an <see cref="OnlineValuesTZMapper"/> with the specified timeout and 
        /// resourceURI.
        /// </summary>
        /// <param name="timeout">The length of time, in milliseconds, before the request times out.</param>
        /// <param name="resourceuri">The URI to use when retrieving CLDR data.</param>
        /// <remarks>
        /// By default, the data retrieved is cached for <see cref="DEFAULTCACHETTL">the default cache TTL time</see>
        /// in the user's temporary folder retrieved from <see cref="Path.GetTempPath"/>.
        /// </remarks>
        public OnlineValuesTZMapper(int timeout, Uri resourceuri)
            : this(TimeSpan.FromMilliseconds(timeout), resourceuri) { }

        /// <summary>
        /// Initializes a new instance of an <see cref="OnlineValuesTZMapper"/> with the specified timeout and 
        /// resourceURI.
        /// </summary>
        /// <param name="timeout">The length of time before the request times out.</param>
        /// <param name="resourceuri">The URI to use when retrieving CLDR data.</param>
        /// <remarks>
        /// By default, the data retrieved is cached for <see cref="DEFAULTCACHETTL">the default cache TTL time</see>
        /// in the user's temporary folder retrieved from <see cref="Path.GetTempPath"/>.
        /// </remarks>
        public OnlineValuesTZMapper(TimeSpan timeout, Uri resourceuri)
            : this(timeout, resourceuri, DEFAULTCACHETTL) { }

        /// <summary>
        /// Initializes a new instance of an <see cref="OnlineValuesTZMapper"/> with the specified timeout and 
        /// resourceURI.
        /// </summary>
        /// <param name="timeout">The length of time, in milliseconds, before the request times out.</param>
        /// <param name="resourceuri">The URI to use when retrieving CLDR data.</param>
        /// <param name="cachettl">
        /// Expiry time for downloaded data; unless this TTL has expired a cached version will be used.
        /// </param>
        /// <remarks>
        /// The default cache directory used is retrieved from <see cref="Path.GetTempPath"/>.
        /// </remarks>
        public OnlineValuesTZMapper(int timeout, Uri resourceuri, TimeSpan cachettl)
            : this(TimeSpan.FromMilliseconds(timeout), resourceuri, cachettl) { }

        /// <summary>
        /// Initializes a new instance of an <see cref="OnlineValuesTZMapper"/> with the specified timeout and 
        /// resourceURI.
        /// </summary>
        /// <param name="timeout">The length of time before the request times out.</param>
        /// <param name="resourceuri">The URI to use when retrieving CLDR data.</param>
        /// <param name="cachettl">
        /// Expiry time for downloaded data; unless this TTL has expired a cached version will be used.
        /// </param>
        /// <remarks>
        /// The default cache directory used is retrieved from <see cref="Path.GetTempPath"/>.
        /// </remarks>
        public OnlineValuesTZMapper(TimeSpan timeout, Uri resourceuri, TimeSpan cachettl)
            : this(timeout, resourceuri, cachettl, Path.GetTempPath()) { }

        /// <summary>
        /// Initializes a new instance of an <see cref="OnlineValuesTZMapper"/> with the specified timeout and 
        /// resourceURI.
        /// </summary>
        /// <param name="timeout">The length of time before the request times out.</param>
        /// <param name="resourceuri">The URI to use when retrieving CLDR data.</param>
        /// <param name="cachettl">
        /// Expiry time for downloaded data; unless this TTL has expired a cached version will be used.
        /// </param>
        /// <param name="cachedirectory">The directory to use to store a cached version of the data.</param>
        public OnlineValuesTZMapper(int timeout, Uri resourceuri, TimeSpan cachettl, string cachedirectory)
            : this(TimeSpan.FromMilliseconds(timeout), resourceuri, cachettl, cachedirectory) { }

        /// <summary>
        /// Initializes a new instance of an <see cref="OnlineValuesTZMapper"/> with the specified timeout and 
        /// resourceURI.
        /// </summary>
        /// <param name="timeout">The length of time, in milliseconds, before the request times out.</param>
        /// <param name="resourceuri">The URI to use when retrieving CLDR data.</param>
        /// <param name="cachettl">
        /// Expiry time for downloaded data; unless this TTL has expired a cached version will be used.
        /// </param>
        /// <param name="cachedirectory">The directory to use to store a cached version of the data.</param>
        /// <param name="throwOnDuplicateKey">
        /// When true, an exception will be thrown when the XML data contains duplicate timezones. When false, 
        /// duplicates are ignored and only the first entry in the XML data will be used.
        /// </param>
        /// <param name="throwOnNonExisting">
        /// When true, an exception will be thrown when the XML data contains non-existing timezone ID's. When false,
        /// non-existing timezone ID's are ignored.
        /// </param>
        public OnlineValuesTZMapper(TimeSpan timeout, Uri resourceuri, TimeSpan cachettl, string cachedirectory, bool throwOnDuplicateKey = false, bool throwOnNonExisting = false)
            : base(new TimedWebClient(timeout, cachettl, cachedirectory).RetrieveCachedString(resourceuri), throwOnDuplicateKey, throwOnNonExisting) { }

        /// <summary>
        /// Simple "wrapper class" providing timeouts.
        /// </summary>
        private class TimedWebClient : WebClient
        {
            public int Timeout { get; set; }

            public TimeSpan DefaultTTL { get; set; }

            public string CacheDirectory { get; set; }

            public TimedWebClient(TimeSpan timeout, TimeSpan ttl, string cachedirectory)
            {
                Timeout = (int)Math.Max(0, timeout.TotalMilliseconds);
                DefaultTTL = ttl;
                CacheDirectory = cachedirectory;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var wr = base.GetWebRequest(address);
                wr.Timeout = Timeout;
                return wr;
            }

            public string RetrieveCachedString(Uri uri)
            {
                var filename = Path.GetFileName(uri.AbsolutePath);
                if (string.IsNullOrEmpty(filename))
                    filename = "windowsZones.xml";
                var dest = Path.Combine(CacheDirectory, filename);
                if (IsFileExpired(dest, DefaultTTL))
                {
                    CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                    DownloadFile(uri, dest);
                }

                using (var f = File.OpenRead(dest))
                using (var fr = new StreamReader(f))
                {
                    return fr.ReadToEnd();
                }
            }

            private static bool IsFileExpired(string path, TimeSpan ttl)
            {
                var x = (DateTime.UtcNow - new FileInfo(path).LastWriteTimeUtc);
                return (!File.Exists(path) || x > ttl);
            }
        }
    }
}