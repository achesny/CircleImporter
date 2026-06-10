using System;
using System.Collections.Generic;
using System.Linq;
using CircleImporter.Core.Interfaces;
using CircleImporter.Core.Models;

namespace CircleImporter.Parsers.World
{
    /// <summary>
    /// Parser for Circle MUD world files (room data)
    /// </summary>
    public class RoomParser : IParser<Room>
    {
        private const char TildeDelimiter = '~';
        private const char HashPrefix = '#';
        private const char ExitMarker = 'D';
        private const char ExtraDescMarker = 'E';
        private const char RoomEndMarker = 'S';

        private const int DirectionNorth = 0;
        private const int DirectionSouth = 1;
        private const int DirectionEast = 2;
        private const int DirectionWest = 3;
        private const int DirectionUp = 4;
        private const int DirectionDown = 5;

        /// <summary>
        /// Parse multiple rooms from Circle world file format
        /// </summary>
        public IEnumerable<Room> Parse(string rawData)
        {
            if (string.IsNullOrWhiteSpace(rawData))
                return Enumerable.Empty<Room>();

            var rooms = new List<Room>();
            var lines = rawData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            
            int i = 0;
            while (i < lines.Length)
            {
                var line = lines[i].Trim();

                // Look for room start marker
                if (line.StartsWith("#"))
                {
                    try
                    {
                        var room = ParseRoomBlock(lines, ref i);
                        if (room != null)
                        {
                            rooms.Add(room);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Error parsing room at line {i}: {ex.Message}", ex);
                    }
                }
                else
                {
                    i++;
                }
            }

            return rooms;
        }

        /// <summary>
        /// Parse a single room from Circle world file format
        /// </summary>
        public Room ParseSingle(string rawData)
        {
            if (string.IsNullOrWhiteSpace(rawData))
                throw new ArgumentNullException(nameof(rawData));

            var lines = rawData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int i = 0;

            return ParseRoomBlock(lines, ref i) 
                ?? throw new InvalidOperationException("Unable to parse room data");
        }

        /// <summary>
        /// Parse a complete room block including headers, description, exits, and extra descriptions
        /// </summary>
        private Room? ParseRoomBlock(string[] lines, ref int lineIndex)
        {
            var room = new Room();

            // Parse room header: #<vnumber>
            var headerLine = lines[lineIndex].Trim();
            if (!headerLine.StartsWith("#"))
                return null;

            if (!int.TryParse(headerLine.Substring(1), out int vnumber))
                throw new InvalidOperationException($"Invalid room vnumber: {headerLine}");

            room.Vnumber = vnumber;
            lineIndex++;

            // Parse room name (terminated by ~)
            room.Name = ReadUntilTilde(lines, ref lineIndex);

            // Parse room description (terminated by ~)
            room.Description = ReadUntilTilde(lines, ref lineIndex);

            // Parse room flags and sector type
            if (lineIndex >= lines.Length)
                throw new InvalidOperationException("Unexpected end of file while parsing room flags");

            var flagsLine = lines[lineIndex].Trim();
            var flagsParts = flagsLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (flagsParts.Length < 2)
                throw new InvalidOperationException($"Invalid room flags line: {flagsLine}");

            if (!int.TryParse(flagsParts[0], out int roomFlags))
                throw new InvalidOperationException($"Invalid room flags value: {flagsParts[0]}");

            if (!int.TryParse(flagsParts[1], out int sectorType))
                throw new InvalidOperationException($"Invalid sector type value: {flagsParts[1]}");

            room.RoomFlags = roomFlags;
            room.SectorType = sectorType;
            lineIndex++;

            // Parse exits and extra descriptions
            while (lineIndex < lines.Length)
            {
                var line = lines[lineIndex].Trim();

                if (line.StartsWith("D"))
                {
                    ParseExit(lines, ref lineIndex, room);
                }
                else if (line.StartsWith("E"))
                {
                    ParseExtraDescription(lines, ref lineIndex);
                }
                else if (line.StartsWith("S"))
                {
                    lineIndex++;
                    break;
                }
                else if (line.StartsWith("#"))
                {
                    // Next room encountered
                    break;
                }
                else
                {
                    lineIndex++;
                }
            }

            return room;
        }

        /// <summary>
        /// Parse an exit directive (D)
        /// </summary>
        private void ParseExit(string[] lines, ref int lineIndex, Room room)
        {
            var line = lines[lineIndex].Trim();
            if (!line.StartsWith("D"))
                throw new InvalidOperationException($"Invalid exit line: {line}");

            lineIndex++;

            // Exit description (terminated by ~)
            var exitDescription = ReadUntilTilde(lines, ref lineIndex);

            // Exit keywords (terminated by ~)
            var exitKeywords = ReadUntilTilde(lines, ref lineIndex);

            // Exit direction, key, and destination
            if (lineIndex >= lines.Length)
                throw new InvalidOperationException("Unexpected end of file while parsing exit data");

            var exitDataLine = lines[lineIndex].Trim();
            var exitParts = exitDataLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (exitParts.Length < 3)
                throw new InvalidOperationException($"Invalid exit data line: {exitDataLine}");

            if (!int.TryParse(exitParts[0], out int direction))
                throw new InvalidOperationException($"Invalid exit direction: {exitParts[0]}");

            if (!int.TryParse(exitParts[1], out int key))
                throw new InvalidOperationException($"Invalid exit key: {exitParts[1]}");

            if (!int.TryParse(exitParts[2], out int destinationVnum))
                throw new InvalidOperationException($"Invalid destination vnum: {exitParts[2]}");

            var exit = new Exit
            {
                Direction = direction,
                Description = exitDescription,
                Keywords = exitKeywords,
                DestinationVnum = destinationVnum
            };

            room.Exits.Add(exit);
            lineIndex++;
        }

        /// <summary>
        /// Parse extra description (E) - typically skipped but handled
        /// </summary>
        private void ParseExtraDescription(string[] lines, ref int lineIndex)
        {
            var line = lines[lineIndex].Trim();
            if (!line.StartsWith("E"))
                throw new InvalidOperationException($"Invalid extra description line: {line}");

            lineIndex++;

            // Skip extra description keywords
            ReadUntilTilde(lines, ref lineIndex);

            // Skip extra description text
            ReadUntilTilde(lines, ref lineIndex);
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
                        // Put the remaining content back for next read
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
