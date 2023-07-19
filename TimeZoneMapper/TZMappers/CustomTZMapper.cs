using System.IO;
using System.Text;

namespace TimeZoneMapper.TZMappers;

/// <summary>
///     Provides TimeZoneID mapping based on custom values provided by a CLDR xml file.
/// </summary>
public sealed class CustomValuesTZMapper : BaseTZMapper, ITZMapper
{
    /// <summary>
    ///     Initializes a CustomValuesTZMapper with custom CLDR data in XML format.
    /// </summary>
    /// <param name="cldrxml">A string containing an XML document with CLDR data.</param>
    /// <param name="throwOnDuplicateKey">
    /// When true, an exception will be thrown when the XML data contains duplicate timezones. When false, 
    /// duplicates are ignored and only the first entry in the XML data will be used.
    /// </param>
    /// <param name="throwOnNonExisting">
    /// When true, an exception will be thrown when the XML data contains non-existing timezone ID's. When false,
    /// non-existing timezone ID's are ignored.
    /// </param>
    public CustomValuesTZMapper(string cldrxml, bool throwOnDuplicateKey = false, bool throwOnNonExisting = false)
        : base(cldrxml, throwOnDuplicateKey, throwOnNonExisting) { }

    /// <summary>
    ///     Initializes a CustomValuesTZMapper with custom CLDR data in XML format.
    /// </summary>
    /// <param name="path">Path to an XML file containing CLDR data.</param>
    /// <param name="encoding">The encoding applied to the contents of the file.</param>
    /// /// <param name="throwOnDuplicateKey">
    /// When true, an exception will be thrown when the XML data contains duplicate timezones. When false, 
    /// duplicates are ignored and only the first entry in the XML data will be used.
    /// </param>
    /// <param name="throwOnNonExisting">
    /// When true, an exception will be thrown when the XML data contains non-existing timezone ID's. When false,
    /// non-existing timezone ID's are ignored.
    /// </param>
    public CustomValuesTZMapper(string path, Encoding encoding, bool throwOnDuplicateKey = false, bool throwOnNonExisting = false)
        : base(File.ReadAllText(path, encoding), throwOnDuplicateKey, throwOnNonExisting) { }

    /// <summary>
    ///     Initializes a CustomValuesTZMapper with custom CLDR data in XML format.
    /// </summary>
    /// <param name="cldrstream">The stream to be read.</param>
    /// <param name="throwOnDuplicateKey">
    /// When true, an exception will be thrown when the XML data contains duplicate timezones. When false, 
    /// duplicates are ignored and only the first entry in the XML data will be used.
    /// </param>
    /// <param name="throwOnNonExisting">
    /// When true, an exception will be thrown when the XML data contains non-existing timezone ID's. When false,
    /// non-existing timezone ID's are ignored.
    /// </param>
    public CustomValuesTZMapper(Stream cldrstream, bool throwOnDuplicateKey = false, bool throwOnNonExisting = false)
        : base(new StreamReader(cldrstream).ReadToEnd(), throwOnDuplicateKey, throwOnNonExisting) { }
}
