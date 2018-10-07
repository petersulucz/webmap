namespace Webmap.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using Webmap.Common;

    public class MapDefinition
    {
        private readonly Dictionary<long, MapNode> nodes;
        private readonly Dictionary<long, MapWay> ways;
        private readonly Dictionary<long, MapPolyWay> relations;
        private readonly XmlReader reader;

        internal MapDefinition(XmlReader reader)
        {
            this.reader = reader;
            this.nodes = new Dictionary<long, MapNode>();
            this.ways = new Dictionary<long, MapWay>();
            this.relations = new Dictionary<long, MapPolyWay>();

            this.ParseOsmNode(ref this.MinBound, ref this.MaxBound);
            this.ParseFirstSetOfNodes();
        }

        public readonly Vector2 MinBound;
        public readonly Vector2 MaxBound;

        /// <summary>
        /// Read the bounds from the OSM node.
        /// </summary>
        /// <param name="minBound">The min bound.</param>
        /// <param name="maxBound">The max bound.</param>
        private void ParseOsmNode(ref Vector2 minBound, ref Vector2 maxBound)
        {
            if(false == string.Equals(this.reader.Name, "osm"))
            {
                throw new InvalidDataException($"The first node should be an OSM node. Instead is {this.reader.Name}");
            }

            // Move to the bounds..
            while (false == reader.Name.Equals("bounds"))
            {
                reader.Read();
            }

            
            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name)
                {
                    case "minlon":
                        minBound.X = double.Parse(reader.Value);
                        break;
                    case "minlat":
                        minBound.Y = double.Parse(reader.Value);
                        break;
                    case "maxlon":
                        maxBound.X = double.Parse(reader.Value);
                        break;
                    case "maxlat":
                        maxBound.Y = double.Parse(reader.Value);
                        break;
                }
            }
        }

        /// <summary>
        /// Parse the first set of nodes.
        /// </summary>
        /// <param name="reader">The xml reader.</param>
        private void ParseFirstSetOfNodes()
        {
            while(this.reader.Read() && string.Equals(this.reader.Name, "node", System.StringComparison.OrdinalIgnoreCase))
            {
                var node = MapDefinition.ParseNode(reader);
                this.nodes.Add(node.Index, node);
            }
        }

        /// <summary>
        /// Reads the ways from the file.
        /// </summary>
        /// <returns>The ways.</returns>
        public IEnumerable<MapWay> ReadWays()
        {
            do {
                switch (reader.Name)
                {
                    case "node":
                        var node = MapDefinition.ParseNode(reader);
                        this.nodes.Add(node.Index, node);
                        break;
                    case "way":
                        var nextWay = ParseMapWay();
                        this.ways.Add(nextWay.Id, nextWay);
                        yield return nextWay;
                        break;
                    case "relation":
                        {
                            if (true == this.TryParseRelation(out var nextRelation))
                            {
                                this.relations.Add(nextRelation.Id, nextRelation);
                            }
                        }
                        break;
                }
            } while (reader.Read());

            yield break;
        }

        /// <summary>
        /// Gets the more complex polygons.
        /// </summary>
        /// <returns>The complex polygons.</returns>
        public IEnumerable<MapPolyWay> ReadPolyWays()
        {
            return this.relations.Values;
        }

        private bool TryParseRelation(out MapPolyWay result)
        {
            if (false == string.Equals("relation", this.reader.Name, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Excpected a 'relation' but got {this.reader.Name}");
            }

            long id = 0;
            while (reader.MoveToNextAttribute())
            {
                switch (this.reader.Name)
                {
                    case "id":
                        id = long.Parse(reader.Value);
                        break;
                }
            }

            List<MapWay> outer = new List<MapWay>();
            List<MapWay> inner = new List<MapWay>();
            List<KeyValuePair<string, string>> tags = new List<KeyValuePair<string, string>>();

            while (this.reader.Read())
            {
                switch (this.reader.Name)
                {
                    case "relation":
                        goto loopexit;
                    case "member":
                        {
                            if (this.TryParseMember(out string role, out var mapWay))
                            {
                                if (string.Equals("outer", role, StringComparison.OrdinalIgnoreCase))
                                {
                                    outer.Add(mapWay);
                                }
                                else if(string.Equals("inner", role, StringComparison.OrdinalIgnoreCase))
                                {
                                    inner.Add(mapWay);
                                }
                            }
                        }
                        break;
                    case "tag":
                        {
                            var tag = MapDefinition.ParseTag(reader);
                            tags.Add(tag);
                        }
                        break;
                    default:
                        break;
                }
            }
            loopexit:
            if (nodes.Count > 0)
            {
                result = new MapPolyWay(id, outer, inner, tags);
                return true;
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Parse a map way.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="map">The map.</param>
        /// <returns>The map way.</returns>
        private MapWay ParseMapWay()
        {
            if (false == string.Equals("way", this.reader.Name, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Excpected a 'way' but got {this.reader.Name}");
            }

            long id = 0;
            List<MapNode> nodes = new List<MapNode>();
            List<KeyValuePair<string, string>> tags = new List<KeyValuePair<string, string>>();

            while (reader.MoveToNextAttribute())
            {
                switch (this.reader.Name)
                {
                    case "id":
                        id = long.Parse(reader.Value);
                        break;
                }
            }

            while (this.reader.Read())
            {
                switch (this.reader.Name)
                {
                    case "way":
                        goto loopexit;
                    case "nd":
                        {
                            var node = this.ParseNodeRef();
                            nodes.Add(node);
                        }
                        break;
                    case "tag":
                        {
                            var tag = MapDefinition.ParseTag(reader);
                            tags.Add(tag);
                        }
                        break;
                    default:
                        break;
                }
            }

            loopexit:
            return new MapWay(id, nodes, tags);
        }

        /// <summary>
        /// Parse a node on the map.
        /// </summary>
        /// <param name="reader">The xml reader.</param>
        /// <returns>The map node.</returns>
        private static MapNode ParseNode(XmlReader reader)
        {
            long id = 0;
            Vector2 coordinates = new Vector2();

            var isEmpty = reader.IsEmptyElement;

            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name)
                {
                    case "lat":
                        coordinates.Y = double.Parse(reader.Value);
                        break;
                    case "lon":
                        coordinates.X = double.Parse(reader.Value);
                        break;
                    case "id":
                        id = long.Parse(reader.Value);
                        break;
                }
            }

            if (false == isEmpty)
            {
                while (reader.Read())
                {
                    if (reader.Name.Equals("node"))
                    {
                        break;
                    }
                }
            }

            return new MapNode(id, coordinates);
        }

        /// <summary>
        /// Parse the nd attribute for a node.
        /// </summary>
        /// <param name="map">The map definition.</param>
        /// <param name="reader">The reader.</param>
        /// <returns>The node.</returns>
        private MapNode ParseNodeRef()
        {
            if (false == string.Equals("nd", this.reader.Name))
            {
                throw new ArgumentException($"This {reader.Name} is not a nd attribute.");
            }

            // Has one attribute.
            this.reader.MoveToFirstAttribute();

            if (false == string.Equals("ref", this.reader.Name))
            {
                throw new ArgumentException($"This attribute: '{this.reader.Name}' is not a nd attribute.");
            }

            var refId = long.Parse(this.reader.Value);
            if (false == this.nodes.TryGetValue(refId, out var node))
            {
                throw new ArgumentException("The map could not be parsed");
            }

            return node;
        }

        /// <summary>
        /// Parses a member node from the map.
        /// </summary>
        /// <returns>The member node.</returns>
        private bool TryParseMember(out string role, out MapWay mapWay)
        {
            if (false == string.Equals("member", this.reader.Name))
            {
                throw new ArgumentException($"This {reader.Name} is not a member attribute.");
            }

            long refId = 0;
            string refType = null;
            role = null;

            while (this.reader.MoveToNextAttribute())
            {
                switch (this.reader.Name)
                {
                    case "type":
                        refType = this.reader.Value;
                        break;
                    case "ref":
                        refId = long.Parse(this.reader.Value);
                        break;
                    case "role":
                        role = this.reader.Value;
                        break;
                }
            }

            if (false == string.Equals(refType, "way") 
                || false == this.ways.TryGetValue(refId, out mapWay)
                || role == null)
            {
                role = null;
                mapWay = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parse a tag.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>A tag.</returns>
        private static KeyValuePair<string, string> ParseTag(XmlReader reader)
        {
            string key = null;
            string value = null;

            if (false == string.Equals("tag", reader.Name))
            {
                throw new ArgumentException($"This {reader.Name} is not a tag attribute.");
            }

            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name[0])
                {
                    case 'k':
                    case 'K':
                        key = reader.Value;
                        break;
                    case 'v':
                    case 'V':
                        value = reader.Value;
                        break;
                    default:
                        throw new ArgumentException($"Attribute {reader.Name} is invalid in a tag.");
                }
            }

            if (key == null || value == null)
            {
                throw new ArgumentException($"Either Key='{key}' or Value='{value}' are null in this tag.");
            }

            return new KeyValuePair<string, string>(key, value);
        }
    }
}
