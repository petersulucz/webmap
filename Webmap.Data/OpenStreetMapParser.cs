using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Webmap.Data
{
    public static class OpenStreetMapParser
    {
        /// <summary>
        /// Parse the Map definition.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static MapDefinition Parse(Stream stream)
        {
            var document = XmlReader.Create(stream);
            while(document.Read() && !string.Equals(document.Name, "osm", StringComparison.OrdinalIgnoreCase))
            {
            }

            return new MapDefinition(document);
        }
    }
}
