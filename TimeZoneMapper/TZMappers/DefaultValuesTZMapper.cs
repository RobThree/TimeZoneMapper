namespace TimeZoneMapper.TZMappers
{
    /// <summary>
    ///     Provides TimeZoneID mapping based on a built-in (&quot;static&quot;) resource.
    /// </summary>
    public sealed class DefaultValuesTZMapper : BaseTZMapper, ITZMapper
    {
        /// <summary>
        /// Provides TimeZoneID mapping based on a built-in (&quot;static&quot;) resource.
        /// </summary>
        /// <param name="throwOnDuplicateKey">
        /// When true, an exception will be thrown when the XML data contains duplicate timezones. When false, 
        /// duplicates are ignored and only the first entry in the XML data will be used.
        /// </param>
        /// <param name="throwOnNonExisting">
        /// When true, an exception will be thrown when the XML data contains non-existing timezone ID's. When false,
        /// non-existing timezone ID's are ignored.
        /// </param>
        public DefaultValuesTZMapper(bool throwOnDuplicateKey = true, bool throwOnNonExisting = true)
            : base(Properties.Resources.windowsZones, throwOnDuplicateKey, throwOnNonExisting) { }
    }
}