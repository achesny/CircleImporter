using CircleImporter.Core.Interfaces;
using CircleImporter.Core.Models;

namespace CircleImporter.Parsers.Mobs;

/// <summary>
/// Parser for Circle MUD mob files.
/// </summary>
public class MobParser : IParser<Mob>
{
    /// <summary>
    /// Parses a single mob from raw mob data.
    /// </summary>
    /// <param name="rawData">Raw mob data string</param>
    /// <returns>Parsed Mob object</returns>
    public Mob ParseSingle(string rawData)
    {
        if (string.IsNullOrWhiteSpace(rawData))
        {
            throw new ArgumentException("Raw data cannot be null or empty.", nameof(rawData));
        }

        var lines = rawData.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        var index = 0;

        return ParseMobBlock(lines, ref index);
    }

    /// <summary>
    /// Parses multiple mobs from raw mob data.
    /// </summary>
    /// <param name="rawData">Raw mob data string containing multiple mobs</param>
    /// <returns>Enumerable of parsed Mob objects</returns>
    public IEnumerable<Mob> Parse(string rawData)
    {
        if (string.IsNullOrWhiteSpace(rawData))
        {
            return new List<Mob>();
        }

        var mobs = new List<Mob>();
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

            // Check for mob marker
            if (lines[index].StartsWith("#"))
            {
                mobs.Add(ParseMobBlock(lines, ref index));
            }
            else
            {
                index++;
            }
        }

        return mobs;
    }

    /// <summary>
    /// Parses a single mob block from the file.
    /// </summary>
    private Mob ParseMobBlock(string[] lines, ref int index)
    {
        var mob = new Mob();

        // Parse vnumber
        if (index >= lines.Length || !lines[index].StartsWith("#"))
        {
            throw new InvalidOperationException("Expected mob vnumber starting with #");
        }

        var vnumLine = lines[index++];
        if (!int.TryParse(vnumLine.AsSpan(1), out var vnumber))
        {
            throw new InvalidOperationException($"Invalid vnumber format: {vnumLine}");
        }

        mob.Vnumber = vnumber;

        // Parse name
        mob.Name = ReadUntilTilde(lines, ref index);

        // Parse short description
        mob.ShortDescription = ReadUntilTilde(lines, ref index);

        // Parse long description
        mob.LongDescription = ReadUntilTilde(lines, ref index);

        // Parse action description
        mob.ActionDescription = ReadUntilTilde(lines, ref index);

        // Parse stats line: race sex class level hitroll damroll ac hp mana move
        if (index >= lines.Length)
        {
            throw new InvalidOperationException("Missing mob stats line");
        }

        var statsLine = lines[index++].Trim();
        var statsValues = statsLine.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        if (statsValues.Length < 10)
        {
            throw new InvalidOperationException($"Invalid mob stats line, expected at least 10 values: {statsLine}");
        }

        if (!int.TryParse(statsValues[0], out var race))
            throw new InvalidOperationException($"Invalid race: {statsValues[0]}");
        if (!int.TryParse(statsValues[1], out var sex))
            throw new InvalidOperationException($"Invalid sex: {statsValues[1]}");
        if (!int.TryParse(statsValues[2], out var mobClass))
            throw new InvalidOperationException($"Invalid class: {statsValues[2]}");
        if (!int.TryParse(statsValues[3], out var level))
            throw new InvalidOperationException($"Invalid level: {statsValues[3]}");
        if (!int.TryParse(statsValues[4], out var hitroll))
            throw new InvalidOperationException($"Invalid hitroll: {statsValues[4]}");
        if (!int.TryParse(statsValues[5], out var damroll))
            throw new InvalidOperationException($"Invalid damroll: {statsValues[5]}");
        if (!int.TryParse(statsValues[6], out var ac))
            throw new InvalidOperationException($"Invalid ac: {statsValues[6]}");
        if (!int.TryParse(statsValues[7], out var hp))
            throw new InvalidOperationException($"Invalid hp: {statsValues[7]}");
        if (!int.TryParse(statsValues[8], out var mana))
            throw new InvalidOperationException($"Invalid mana: {statsValues[8]}");
        if (!int.TryParse(statsValues[9], out var move))
            throw new InvalidOperationException($"Invalid move: {statsValues[9]}");

        mob.Race = race;
        mob.Sex = sex;
        mob.Class = mobClass;
        mob.Level = level;
        mob.Hitroll = hitroll;
        mob.Damroll = damroll;
        mob.Ac = ac;
        mob.Hp = hp;
        mob.Mana = mana;
        mob.Move = move;

        // Parse additional values line: gold experience position default_position flags alignment
        if (index >= lines.Length)
        {
            throw new InvalidOperationException("Missing mob additional values line");
        }

        var additionalLine = lines[index++].Trim();
        var additionalValues = additionalLine.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        if (additionalValues.Length < 6)
        {
            throw new InvalidOperationException($"Invalid mob additional values line, expected at least 6 values: {additionalLine}");
        }

        if (!int.TryParse(additionalValues[0], out var gold))
            throw new InvalidOperationException($"Invalid gold: {additionalValues[0]}");
        if (!int.TryParse(additionalValues[1], out var experience))
            throw new InvalidOperationException($"Invalid experience: {additionalValues[1]}");
        if (!int.TryParse(additionalValues[2], out var position))
            throw new InvalidOperationException($"Invalid position: {additionalValues[2]}");
        if (!int.TryParse(additionalValues[3], out var defaultPosition))
            throw new InvalidOperationException($"Invalid default_position: {additionalValues[3]}");
        if (!int.TryParse(additionalValues[4], out var flags))
            throw new InvalidOperationException($"Invalid flags: {additionalValues[4]}");
        if (!int.TryParse(additionalValues[5], out var alignment))
            throw new InvalidOperationException($"Invalid alignment: {additionalValues[5]}");

        mob.Gold = gold;
        mob.Experience = experience;
        mob.Position = position;
        mob.DefaultPosition = defaultPosition;
        mob.Flags = flags;
        mob.Alignment = alignment;

        // Skip until end marker
        while (index < lines.Length && lines[index].Trim() != "$")
        {
            index++;
        }

        if (index < lines.Length && lines[index].Trim() == "$")
        {
            index++;
        }

        return mob;
    }

    /// <summary>
    /// Reads a tilde-delimited string from the lines array.
    /// </summary>
    private string ReadUntilTilde(string[] lines, ref int index)
    {
        var sb = new System.Text.StringBuilder();

        while (index < lines.Length)
        {
            var line = lines[index++];

            if (line.Contains("~"))
            {
                var tildeIndex = line.IndexOf("~");
                sb.Append(line.AsSpan(0, tildeIndex));
                break;
            }

            sb.AppendLine(line);
        }

        return sb.ToString().TrimEnd();
    }
}
