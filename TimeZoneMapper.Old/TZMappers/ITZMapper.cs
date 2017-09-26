namespace TimeZoneMapper.TZMappers
{
    using System;

    /// <summary>
    /// Provides the base interface for implementation of a TimeZoneMapper class.
    /// </summary>
    public interface ITZMapper
    {
        /// <summary>
        ///     Returns an array of available <see cref="TimeZoneInfo"/> objects that the mapper can return.
        /// </summary>
        /// <returns>Returns an array of available <see cref="TimeZoneInfo"/> objects that the mapper can return.</returns>
        TimeZoneInfo[] GetAvailableTimeZones();

        /// <summary>
        ///     Returns an array of available TimeZone ID's and returns these as an array.
        /// </summary>
        /// <returns>Returns an array of all available ('known') TimeZone ID's.</returns>
        string[] GetAvailableTZIDs();

        /// <summary>
        ///     Maps a TimeZone ID (e.g. "Europe/Amsterdam") to a corresponding TimeZoneInfo object.
        /// </summary>
        /// <param name="tzid">The TimeZone ID (e.g. "Europe/Amsterdam").</param>
        /// <returns>Returns a .Net BCL <see cref="TimeZoneInfo"/> object corresponding to the TimeZone ID.</returns>
        TimeZoneInfo MapTZID(string tzid);

        /// <summary>
        ///     Maps a TimeZone ID (e.g. "Europe/Amsterdam") to a corresponding TimeZoneInfo object.
        /// </summary>
        /// <param name="tzid">The TimeZone ID (e.g. "Europe/Amsterdam").</param>
        /// <param name="timeZoneInfo">
        /// When this method returns, contains the value associated with the specified TimeZone ID, if the timezone is
        /// found; otherwise, null.
        ///</param>
        /// <returns>true if the <see cref="ITZMapper"/> contains an element with the specified timezone; otherwise, false.</returns>
        bool TryMapTZID(string tzid, out TimeZoneInfo timeZoneInfo);

        /// <summary>
        /// Gets the TimeZoneID version part of the resource currently in use.
        /// </summary>
        /// <remarks>This value corresponds to the &quot;typeVersion&quot; attribute of the resource data.</remarks>
        string TZIDVersion { get; }

        /// <summary>
        /// Gets the TimeZoneInfo version part of the resource currently in use.
        /// </summary>
        /// <remarks>This value corresponds to the &quot;otherVersion&quot; attribute of the resource data.</remarks>
        string TZVersion { get; }

        /// <summary>
        /// Gets the version of the resource currently in use.
        /// </summary>
        /// <remarks>This value is a composite of &quot;<see cref="TZIDVersion"/>.<see cref="TZVersion"/>&quot;.</remarks>
        string Version { get; }
    }
}
