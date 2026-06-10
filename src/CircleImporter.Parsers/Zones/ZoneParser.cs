using CircleImporter.Core.Interfaces;
using CircleImporter.Core.Models;

namespace CircleImporter.Parsers.Zones;

/// <summary>
/// Parser for Circle MUD zone files.
/// </summary>
public class ZoneParser : IParser<Zone>
{
    /// <summary>
    /// Parses a single zone from raw zone data.
    /// </summary>
    /// <param name="rawData">Raw zone data string</param>
    /// <returns>Parsed Zone object</returns>
    public Zone ParseSingle(string rawData)
    {
        if (string.IsNullOrWhiteSpace(rawData))
        {
            throw new ArgumentException("Raw data cannot be null or empty.", nameof(rawData));
        }

        var lines = rawData.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        var index = 0;

        return ParseZoneBlock(lines, ref index);
    }

    /// <summary>
    /// Parses multiple zones from raw zone data.
    /// </summary>
    /// <param name="rawData">Raw zone data string containing multiple zones</param>
    /// <returns>Enumerable of parsed Zone objects</returns>
    public IEnumerable<Zone> Parse(string rawData)
    {
        if (string.IsNullOrWhiteSpace(rawData))
        {
            return new List<Zone>();
        }

        var zones = new List<Zone>();
        var lines = rawData.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        var index = 0;

        while (index < lines.Length)
        {
            // Skip empty lines
            while (index < lines.Length && string.IsNullOrWhiteSpace(lines[index]))
            {
                index++;
            }

            if (index >= lines.Length)
            {
                break;
            }

            // Check for zone marker
            if (lines[index].StartsWith("#"))
            {
                zones.Add(ParseZoneBlock(lines, ref index));
            }
            else
            {
                index++;
            }
        }

        return zones;
    }

    /// <summary>
    /// Parses a single zone block from the file.
    /// </summary>
    private Zone ParseZoneBlock(string[] lines, ref int index)
    {
        var zone = new Zone();

        // Parse vnumber
        if (index >= lines.Length || !lines[index].StartsWith("#"))
        {
            throw new InvalidOperationException("Expected zone vnumber starting with #");
        }

        var vnumLine = lines[index++];
        if (!int.TryParse(vnumLine.AsSpan(1), out var vnumber))
        {
            throw new InvalidOperationException($"Invalid vnumber format: {vnumLine}");
        }

        zone.Vnumber = vnumber;

        // Parse name
        if (index >= lines.Length)
        {
            throw new InvalidOperationException("Missing zone name");
        }

        var nameLine = lines[index++];
        var tildeIndex = nameLine.IndexOf("~");
        if (tildeIndex < 0)
        {
            throw new InvalidOperationException($"Zone name must end with ~: {nameLine}");
        }

        zone.Name = nameLine.Substring(0, tildeIndex).Trim();

        // Parse zone info line: bottom top lifespan reset_mode
        if (index >= lines.Length)
        {
            throw new InvalidOperationException("Missing zone info line");
        }

        var infoLine = lines[index++].Trim();
        var infoValues = infoLine.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        if (infoValues.Length < 4)
        {
            throw new InvalidOperationException($"Invalid zone info line, expected 4 values: {infoLine}");
        }

        if (!int.TryParse(infoValues[0], out var bottom))
            throw new InvalidOperationException($"Invalid bottom: {infoValues[0]}");
        if (!int.TryParse(infoValues[1], out var top))
            throw new InvalidOperationException($"Invalid top: {infoValues[1]}");
        if (!int.TryParse(infoValues[2], out var lifespan))
            throw new InvalidOperationException($"Invalid lifespan: {infoValues[2]}");
        if (!int.TryParse(infoValues[3], out var resetMode))
            throw new InvalidOperationException($"Invalid reset_mode: {infoValues[3]}");

        zone.Bottom = bottom;
        zone.Top = top;
        zone.Lifespan = lifespan;
        zone.ResetMode = resetMode;

        // Parse commands until end marker
        while (index < lines.Length)
        {
            var cmdLine = lines[index].Trim();

            // Check for end marker
            if (cmdLine == "S" || cmdLine.StartsWith("S "))
            {
                index++;
                break;
            }

            if (string.IsNullOrWhiteSpace(cmdLine))
            {
                index++;
                continue;
            }

            // Parse command
            var command = ParseCommand(cmdLine);
            zone.Commands.Add(command);
            index++;
        }

        return zone;
    }

    /// <summary>
    /// Parses a single zone command line.
    /// </summary>
    private ZoneCommand ParseCommand(string line)
    {
        var command = new ZoneCommand();

        // Extract comment if present
        var commentIndex = line.IndexOf("*");
        string commandPart = commentIndex >= 0 ? line.Substring(0, commentIndex).Trim() : line.Trim();
        if (commentIndex >= 0)
        {
            command.Comment = line.Substring(commentIndex).Trim();
        }

        var parts = commandPart.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 5)
        {
            throw new InvalidOperationException($"Invalid zone command, expected at least 5 values: {line}");
        }

        if (!int.TryParse(parts[0], out var ifFlag))
            throw new InvalidOperationException($"Invalid if_flag: {parts[0]}");
        if (!char.TryParse(parts[1], out var cmd))
            throw new InvalidOperationException($"Invalid command: {parts[1]}");
        
        // Validate command is one of the valid types
        if ("MOPGERD".IndexOf(cmd) < 0 && cmd != 'S')
            throw new InvalidOperationException($"Invalid command type: {cmd}");
        
        if (!int.TryParse(parts[2], out var arg1))
            throw new InvalidOperationException($"Invalid arg1: {parts[2]}");
        if (!int.TryParse(parts[3], out var arg2))
            throw new InvalidOperationException($"Invalid arg2: {parts[3]}");
        if (!int.TryParse(parts[4], out var arg3))
            throw new InvalidOperationException($"Invalid arg3: {parts[4]}");

        command.IfFlag = ifFlag;
        command.Command = cmd;
        command.Arg1 = arg1;
        command.Arg2 = arg2;
        command.Arg3 = arg3;

        return command;
    }
}
