namespace TimeZoneMapper
{
    using System;
    using TimeZoneMapper.TZMappers;

    /// <summary>
    ///     Provides access to different types of <see cref="ITZMapper"/>s.
    /// </summary>
    public static class TimeZoneMap
    {
        /// <summary>
        ///     Gets a <see cref="ITZMapper"/> that uses a built-in (and thus, possibly outdated) resource.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The mappings are based on the built-in data. The specific version of the resource can be retrieved
        ///         by examining the <see cref="ITZMapper.TZVersion"/> and <see cref="ITZMapper.TZIDVersion"/> (or
        ///         <see cref="ITZMapper.Version"/>)
        ///         properties.
        ///     </para>
        /// </remarks>
        public static ITZMapper DefaultValuesTZMapper { get { return _defaultvaluesmapper.Value; } }

        /// <summary>
        ///     Gets a <see cref="ITZMapper"/> that uses a online (and thus, possibly 'unreachable') resource.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The mappings are retrieved from the online resource a single time (upon first usage) and used from
        ///         there on. Consider that, between different runs of the application, different values may be
        ///         returned when the online resource changes. The specific version of the resource can be retrieved by
        ///         examining the <see cref="ITZMapper.TZVersion"/> and <see cref="ITZMapper.TZIDVersion"/> (or 
        ///         <see cref="ITZMapper.Version"/>)
        ///         properties.
        ///     </para>
        /// </remarks>
        public static ITZMapper OnlineValuesTZMapper { get { return _onlinevaluesmapper.Value; } }

        /// <summary>
        ///     Gets a <see cref="ITZMapper"/> that tries to use the online resource and, when unreachable or otherwise
        ///     problematic, uses the built-in resource as fallback.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Consider that, between different runs of the application, different values may be returned when the
        ///         online resource changes or the online resource is out of sync with the current built-in resource
        ///         and unreachable.
        ///     </para>
        /// </remarks>
        /// <seealso cref="OnlineValuesTZMapper"/>
        public static ITZMapper OnlineWithFallbackValuesTZMapper { get { return _onlinewithfallbackvaluesmapper.Value; } }

        private readonly static Lazy<DefaultValuesTZMapper> _defaultvaluesmapper;
        private readonly static Lazy<OnlineValuesTZMapper> _onlinevaluesmapper;
        private readonly static Lazy<ITZMapper> _onlinewithfallbackvaluesmapper;

        static TimeZoneMap()
        {
            _defaultvaluesmapper = new Lazy<DefaultValuesTZMapper>(() => new DefaultValuesTZMapper());
            _onlinevaluesmapper = new Lazy<OnlineValuesTZMapper>(() => new OnlineValuesTZMapper());
            _onlinewithfallbackvaluesmapper = new Lazy<ITZMapper>(() => { try { return (ITZMapper)TimeZoneMap.OnlineValuesTZMapper; } catch { return (ITZMapper)TimeZoneMap.DefaultValuesTZMapper; } });
        }
    }
}