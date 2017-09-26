﻿namespace TimeZoneMapper.TZMappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    ///     Provides a base class for TimeZoneMapper objects.
    /// </summary>
    public abstract class BaseTZMapper : ITZMapper
    {
        private readonly Dictionary<string, TimeZoneInfo> _mappings;

        /// <summary>
        /// Gets the TimeZoneID version part of the resource currently in use.
        /// </summary>
        /// <remarks>This value corresponds to the &quot;typeVersion&quot; attribute of the resource data.</remarks>
        public string TZIDVersion { get; private set; }

        /// <summary>
        /// Gets the TimeZoneInfo version part of the resource currently in use.
        /// </summary>
        /// <remarks>This value corresponds to the &quot;otherVersion&quot; attribute of the resource data.</remarks>
        public string TZVersion { get; private set; }

        /// <summary>
        /// Gets the version of the resource currently in use.
        /// </summary>
        /// <remarks>This value is a composite of &quot;<see cref="TZIDVersion"/>.<see cref="TZVersion"/>&quot;.</remarks>
        public string Version { get; private set; }

        internal BaseTZMapper(string xmldata, bool throwOnDuplicateKey)
        {
            var root = XDocument.Parse(xmldata).Descendants("mapTimezones").First();
            _mappings = root.Descendants("mapZone")
                .Where(n => !n.Attribute("territory").Value.Equals("001"))
                .SelectMany(n => n.Attribute("type").Value.Split(new[] { ' ' }), (n, t) => new { TZID = t, TZ = TryGetTimeZone(n.Attribute("other").Value) })
                .Where(n => n.TZ != null)   //Filter out "not found" TimeZones
                .OrderBy(n => n.TZID)
                .ToDictionarySafe(n => n.TZID, v => v.TZ, StringComparer.OrdinalIgnoreCase, throwOnDuplicateKey);

            this.TZIDVersion = root.Attribute("typeVersion").GetSafeValue();
            this.TZVersion = root.Attribute("otherVersion").GetSafeValue();
            this.Version = string.Format("{0}.{1}", this.TZIDVersion, this.TZVersion);
        }

        /// <summary>
        /// Retrieves a TimeZone by it's Id, handling exceptions and returning null instead for invalid / not found Id's.
        /// </summary>
        /// <param name="id">The time zone identifier, which corresponds to the Id property.</param>
        /// <returns>Returns the TimeZone when found, null otherwise.</returns>
        private static TimeZoneInfo TryGetTimeZone(string id)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
                return null;
            }
            catch (InvalidTimeZoneException)
            {
                return null;
            }
        }

        /// <summary>
        ///     Maps a TimeZone ID (e.g. "Europe/Amsterdam") to a corresponding TimeZoneInfo object.
        /// </summary>
        /// <param name="tzid">The TimeZone ID (e.g. "Europe/Amsterdam").</param>
        /// <returns>Returns a .Net BCL <see cref="TimeZoneInfo"/> object corresponding to the TimeZone ID.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the specified TimeZone ID is not found.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the specified TimeZone ID is null.</exception>
        public TimeZoneInfo MapTZID(string tzid)
        {
            return _mappings[tzid];
        }

        /// <summary>
        ///     Maps a TimeZone ID (e.g. "Europe/Amsterdam") to a corresponding TimeZoneInfo object.
        /// </summary>
        /// <param name="tzid">The TimeZone ID (e.g. "Europe/Amsterdam").</param>
        /// <param name="timeZoneInfo">
        /// When this method returns, contains the value associated with the specified TimeZone ID, if the timezone is
        /// found; otherwise, null.
        ///</param>
        /// <returns>true if the <see cref="ITZMapper"/> contains an element with the specified timezone; otherwise, false.</returns>
        public bool TryMapTZID(string tzid, out TimeZoneInfo timeZoneInfo)
        {
            return _mappings.TryGetValue(tzid, out timeZoneInfo);
        }

        /// <summary>
        ///     Builds an array of available TimeZone ID's and returns these as an array.
        /// </summary>
        /// <returns>Returns an array of all available ('known') TimeZone ID's.</returns>
        public string[] GetAvailableTZIDs()
        {
            return _mappings.Keys.ToArray();
        }

        /// <summary>
        ///     Builds an array of available <see cref="TimeZoneInfo"/> objects that the mapper can return.
        /// </summary>
        /// <returns>Returns an array of available <see cref="TimeZoneInfo"/> objects that the mapper can return.</returns>
        public TimeZoneInfo[] GetAvailableTimeZones()
        {
            return _mappings.Values.Distinct().ToArray();
        }
    }

    internal static class LinqExtensions
    {
        public static Dictionary<TKey, TElement> ToDictionarySafe<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, bool throwOnDuplicateKey)
        {
            Dictionary<TKey, TElement> ret = new Dictionary<TKey, TElement>(comparer ?? EqualityComparer<TKey>.Default);
            foreach (TSource item in source)
            {
                var key = keySelector(item);
                if (!ret.ContainsKey(key))
                    ret.Add(key, elementSelector(item));
                else
                {
                    // if throwOnDuplicateKey then we throw, if not we disregard any duplicate keys and only store the first we encounter
                    if (throwOnDuplicateKey)
                        throw new ArgumentException(string.Format("Key '{0}' already exists", key));
                }
            }
            return ret;
        }
    }

    internal static class XAttributeExtensions
    {
        public static string GetSafeValue(this XAttribute att)
        {
            if (att == null)
                return null;
            return att.Value;
        }
    }
}