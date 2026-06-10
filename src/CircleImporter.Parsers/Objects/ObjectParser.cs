using System;
using System.Collections.Generic;
using System.Linq;
using CircleImporter.Core.Interfaces;
using CircleImporter.Core.Models;

namespace CircleImporter.Parsers.Objects
{
    /// <summary>
    /// Parser for Circle MUD object files
    /// </summary>
    public class ObjectParser : IParser<CircleImporter.Core.Models.Object>
    {
        private const char TildeDelimiter = '~';
        private const char AffectMarker = 'A';
        private const char ObjectEndMarker = '$';

        /// <summary>
        /// Parse multiple objects from Circle object file format
        /// </summary>
        public IEnumerable<CircleImporter.Core.Models.Object> Parse(string rawData)
        {
            if (string.IsNullOrWhiteSpace(rawData))
                return Enumerable.Empty<CircleImporter.Core.Models.Object>();

            var objects = new List<CircleImporter.Core.Models.Object>();
            var lines = rawData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            
            int i = 0;
            while (i < lines.Length)
            {
                var line = lines[i].Trim();

                // Look for object start marker
                if (line.StartsWith("#"))
                {
                    try
                    {
                        var obj = ParseObjectBlock(lines, ref i);
                        if (obj != null)
                        {
                            objects.Add(obj);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Error parsing object at line {i}: {ex.Message}", ex);
                    }
                }
                else
                {
                    i++;
                }
            }

            return objects;
        }

        /// <summary>
        /// Parse a single object from Circle object file format
        /// </summary>
        public CircleImporter.Core.Models.Object ParseSingle(string rawData)
        {
            if (string.IsNullOrWhiteSpace(rawData))
                throw new ArgumentNullException(nameof(rawData));

            var lines = rawData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int i = 0;

            return ParseObjectBlock(lines, ref i)
                ?? throw new InvalidOperationException("Unable to parse object data");
        }

        /// <summary>
        /// Parse a complete object block
        /// </summary>
        private CircleImporter.Core.Models.Object? ParseObjectBlock(string[] lines, ref int lineIndex)
        {
            var obj = new CircleImporter.Core.Models.Object();

            // Parse object header: #<vnumber>
            var headerLine = lines[lineIndex].Trim();
            if (!headerLine.StartsWith("#"))
                return null;

            if (!int.TryParse(headerLine.Substring(1), out int vnumber))
                throw new InvalidOperationException($"Invalid object vnumber: {headerLine}");

            obj.Vnumber = vnumber;
            lineIndex++;

            // Parse object name (terminated by ~)
            obj.Name = ReadUntilTilde(lines, ref lineIndex);

            // Parse short description (terminated by ~)
            obj.ShortDescription = ReadUntilTilde(lines, ref lineIndex);

            // Parse long description (terminated by ~)
            obj.LongDescription = ReadUntilTilde(lines, ref lineIndex);

            // Parse action description (terminated by ~)
            obj.ActionDescription = ReadUntilTilde(lines, ref lineIndex);

            // Parse object type, flags, and wear flags
            if (lineIndex >= lines.Length)
                throw new InvalidOperationException("Unexpected end of file while parsing object type and flags");

            var typeLine = lines[lineIndex].Trim();
            var typeParts = typeLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (typeParts.Length < 3)
                throw new InvalidOperationException($"Invalid object type line: {typeLine}");

            if (!int.TryParse(typeParts[0], out int type))
                throw new InvalidOperationException($"Invalid object type value: {typeParts[0]}");

            if (!int.TryParse(typeParts[1], out int extraFlags))
                throw new InvalidOperationException($"Invalid extra flags value: {typeParts[1]}");

            if (!int.TryParse(typeParts[2], out int wearFlags))
                throw new InvalidOperationException($"Invalid wear flags value: {typeParts[2]}");

            obj.Type = type;
            obj.ExtraFlags = extraFlags;
            obj.WearFlags = wearFlags;
            lineIndex++;

            // Parse values (weight, value, rent)
            if (lineIndex >= lines.Length)
                throw new InvalidOperationException("Unexpected end of file while parsing object values");

            var valuesLine = lines[lineIndex].Trim();
            var valueParts = valuesLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (valueParts.Length < 4)
                throw new InvalidOperationException($"Invalid object values line: {valuesLine}");

            for (int v = 0; v < 4; v++)
            {
                if (!int.TryParse(valueParts[v], out int value))
                    throw new InvalidOperationException($"Invalid object value at index {v}: {valueParts[v]}");
                obj.Values.Add(value);
            }

            // Extract standard values
            obj.Weight = obj.Values[0];
            obj.Value = obj.Values[1];
            obj.Rent = obj.Values[2];

            lineIndex++;

            // Parse affects and end marker
            while (lineIndex < lines.Length)
            {
                var line = lines[lineIndex].Trim();

                if (line.StartsWith("A"))
                {
                    ParseAffect(lines, ref lineIndex, obj);
                }
                else if (line.StartsWith("$"))
                {
                    lineIndex++;
                    break;
                }
                else if (line.StartsWith("#"))
                {
                    // Next object encountered
                    break;
                }
                else
                {
                    lineIndex++;
                }
            }

            return obj;
        }

        /// <summary>
        /// Parse an affect directive (A)
        /// </summary>
        private void ParseAffect(string[] lines, ref int lineIndex, CircleImporter.Core.Models.Object obj)
        {
            var line = lines[lineIndex].Trim();
            if (!line.StartsWith("A"))
                throw new InvalidOperationException($"Invalid affect line: {line}");

            lineIndex++;

            // Parse affect location and modifier
            if (lineIndex >= lines.Length)
                throw new InvalidOperationException("Unexpected end of file while parsing affect data");

            var affectLine = lines[lineIndex].Trim();
            var affectParts = affectLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (affectParts.Length < 2)
                throw new InvalidOperationException($"Invalid affect data line: {affectLine}");

            if (!int.TryParse(affectParts[0], out int location))
                throw new InvalidOperationException($"Invalid affect location: {affectParts[0]}");

            if (!int.TryParse(affectParts[1], out int modifier))
                throw new InvalidOperationException($"Invalid affect modifier: {affectParts[1]}");

            var affect = new ObjectAffect
            {
                Location = location,
                Modifier = modifier
            };

            obj.Affects.Add(affect);
            lineIndex++;
        }

        /// <summary>
        /// Read text until a tilde (~) delimiter is encountered
        /// </summary>
        private string ReadUntilTilde(string[] lines, ref int lineIndex)
        {
            var content = new List<string>();

            while (lineIndex < lines.Length)
            {
                var line = lines[lineIndex];
                var tildeIndex = line.IndexOf(TildeDelimiter);

                if (tildeIndex >= 0)
                {
                    // Tilde found on this line
                    content.Add(line.Substring(0, tildeIndex));
                    
                    // Handle case where tilde is not the only thing on the line
                    var remainingLine = line.Substring(tildeIndex + 1).Trim();
                    if (!string.IsNullOrEmpty(remainingLine))
                    {
                        lines[lineIndex] = remainingLine;
                    }
                    else
                    {
                        lineIndex++;
                    }

                    break;
                }
                else
                {
                    content.Add(line);
                    lineIndex++;
                }
            }

            return string.Join("\n", content).Trim();
        }
    }
}
