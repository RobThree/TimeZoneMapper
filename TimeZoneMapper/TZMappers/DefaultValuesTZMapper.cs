namespace TimeZoneMapper.TZMappers
{
    /// <summary>
    ///     Provides TimeZoneID mapping based on a built-in (&quot;static&quot;) resource.
    /// </summary>
    public sealed class DefaultValuesTZMapper : BaseTZMapper, TimeZoneMapper.TZMappers.ITZMapper
    {
        internal DefaultValuesTZMapper()
            : base(Properties.Resources.windowsZones) { }
    }
}