using System.IO;
using System.Text;

namespace TimeZoneMapper.TZMappers
{
    /// <summary>
    ///     Provides TimeZoneID mapping based on custom values provided by a CLDR xml file.
    /// </summary>
    public sealed class CustomValuesTZMapper : BaseTZMapper, TimeZoneMapper.TZMappers.ITZMapper
    {
        /// <summary>
        ///     Initializes a CustomValuesTZMapper with custom CLDR data in XML format.
        /// </summary>
        /// <param name="cldrxml">A string containing an XML document with CLDR data.</param>
        public CustomValuesTZMapper(string cldrxml)
            : base(cldrxml, true) { }

        /// <summary>
        ///     Initializes a CustomValuesTZMapper with custom CLDR data in XML format.
        /// </summary>
        /// <param name="path">Path to an XML file containing CLDR data.</param>
        /// <param name="encoding">The encoding applied to the contents of the file.</param>
        public CustomValuesTZMapper(string path, Encoding encoding)
            : base(File.ReadAllText(path, encoding), true) { }

        /// <summary>
        ///     Initializes a CustomValuesTZMapper with custom CLDR data in XML format.
        /// </summary>
        /// <param name="cldrstream">The stream to be read.</param>
        public CustomValuesTZMapper(Stream cldrstream)
            : base(new StreamReader(cldrstream).ReadToEnd(), true) { }
    }
}
