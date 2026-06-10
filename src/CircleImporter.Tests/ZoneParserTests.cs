using CircleImporter.Core.Models;
using CircleImporter.Parsers.Zones;
using Xunit;

namespace CircleImporter.Tests;

public class ZoneParserTests
{
    private readonly ZoneParser _parser = new();

    #region Basic Parsing Tests

    [Fact]
    public void ParseSingle_WithValidZone_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "The Beginner Zone~\n" +
                    "0 99 30 1\n" +
                    "1 M 3001 2 3010\n" +
                    "1 O 3001 2 3010\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(100, zone.Vnumber);
        Assert.Equal("The Beginner Zone", zone.Name);
        Assert.Equal(0, zone.Bottom);
        Assert.Equal(99, zone.Top);
        Assert.Equal(30, zone.Lifespan);
        Assert.Equal(1, zone.ResetMode);
    }

    [Fact]
    public void ParseSingle_WithNullInput_ThrowsException()
    {
        // Act & Assert
#pragma warning disable CS8625
        Assert.Throws<ArgumentException>(() => _parser.ParseSingle(null));
#pragma warning restore CS8625
    }

    [Fact]
    public void ParseSingle_WithEmptyInput_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _parser.ParseSingle(string.Empty));
    }

    #endregion

    #region Zone Info Tests

    [Fact]
    public void ParseSingle_WithAllZoneInfo_ParsesCorrectly()
    {
        // Arrange
        var input = "#200\n" +
                    "Mid-Level Zone~\n" +
                    "100 199 45 2\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(200, zone.Vnumber);
        Assert.Equal("Mid-Level Zone", zone.Name);
        Assert.Equal(100, zone.Bottom);
        Assert.Equal(199, zone.Top);
        Assert.Equal(45, zone.Lifespan);
        Assert.Equal(2, zone.ResetMode);
    }

    [Fact]
    public void ParseSingle_WithZeroValues_ParsesCorrectly()
    {
        // Arrange
        var input = "#0\n" +
                    "Empty Zone~\n" +
                    "0 0 0 0\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(0, zone.Vnumber);
        Assert.Equal(0, zone.Bottom);
        Assert.Equal(0, zone.Top);
        Assert.Equal(0, zone.Lifespan);
        Assert.Equal(0, zone.ResetMode);
    }

    [Fact]
    public void ParseSingle_WithLargeZoneValues_ParsesCorrectly()
    {
        // Arrange
        var input = "#99999\n" +
                    "Massive Zone~\n" +
                    "90000 99999 120 2\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(99999, zone.Vnumber);
        Assert.Equal(90000, zone.Bottom);
        Assert.Equal(99999, zone.Top);
        Assert.Equal(120, zone.Lifespan);
    }

    #endregion

    #region Command Parsing Tests

    [Fact]
    public void ParseSingle_WithLoadMobCommand_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "1 M 3001 2 3010\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Single(zone.Commands);
        var cmd = zone.Commands[0];
        Assert.Equal(1, cmd.IfFlag);
        Assert.Equal('M', cmd.Command);
        Assert.Equal(3001, cmd.Arg1);
        Assert.Equal(2, cmd.Arg2);
        Assert.Equal(3010, cmd.Arg3);
    }

    [Fact]
    public void ParseSingle_WithLoadObjectCommand_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "0 O 3005 1 3010\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Single(zone.Commands);
        var cmd = zone.Commands[0];
        Assert.Equal('O', cmd.Command);
        Assert.Equal(0, cmd.IfFlag);
    }

    [Fact]
    public void ParseSingle_WithPutObjectCommand_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "1 P 3020 1 3015\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Single(zone.Commands);
        Assert.Equal('P', zone.Commands[0].Command);
    }

    [Fact]
    public void ParseSingle_WithGiveObjectCommand_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "1 G 3025 1 0\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Single(zone.Commands);
        Assert.Equal('G', zone.Commands[0].Command);
    }

    [Fact]
    public void ParseSingle_WithEquipObjectCommand_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "1 E 3030 1 5\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Single(zone.Commands);
        var cmd = zone.Commands[0];
        Assert.Equal('E', cmd.Command);
        Assert.Equal(5, cmd.Arg3); // Position
    }

    [Fact]
    public void ParseSingle_WithRemoveObjectCommand_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "1 D 3010 3001 0\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Single(zone.Commands);
        Assert.Equal('D', zone.Commands[0].Command);
    }

    [Fact]
    public void ParseSingle_WithRemoveObjectRoomCommand_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "1 R 3010 3005 0\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Single(zone.Commands);
        Assert.Equal('R', zone.Commands[0].Command);
    }

    [Fact]
    public void ParseSingle_WithMultipleCommands_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "1 M 3001 2 3010\n" +
                    "1 O 3005 1 3010\n" +
                    "1 E 3030 1 5\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(3, zone.Commands.Count);
        Assert.Equal('M', zone.Commands[0].Command);
        Assert.Equal('O', zone.Commands[1].Command);
        Assert.Equal('E', zone.Commands[2].Command);
    }

    #endregion

    #region If Flag Tests

    [Fact]
    public void ParseSingle_WithIfFlagZero_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "0 M 3001 2 3010\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(0, zone.Commands[0].IfFlag);
    }

    [Fact]
    public void ParseSingle_WithIfFlagOne_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "1 M 3001 2 3010\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(1, zone.Commands[0].IfFlag);
    }

    [Fact]
    public void ParseSingle_WithIfFlagTwo_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "2 M 3001 2 3010\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(2, zone.Commands[0].IfFlag);
    }

    #endregion

    #region Command Arguments Tests

    [Fact]
    public void ParseSingle_WithZeroArguments_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "1 M 0 0 0\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        var cmd = zone.Commands[0];
        Assert.Equal(0, cmd.Arg1);
        Assert.Equal(0, cmd.Arg2);
        Assert.Equal(0, cmd.Arg3);
    }

    [Fact]
    public void ParseSingle_WithNegativeArguments_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "1 M -100 -50 -25\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        var cmd = zone.Commands[0];
        Assert.Equal(-100, cmd.Arg1);
        Assert.Equal(-50, cmd.Arg2);
        Assert.Equal(-25, cmd.Arg3);
    }

    [Fact]
    public void ParseSingle_WithLargeArguments_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "1 M 99999 100 50000\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        var cmd = zone.Commands[0];
        Assert.Equal(99999, cmd.Arg1);
        Assert.Equal(100, cmd.Arg2);
        Assert.Equal(50000, cmd.Arg3);
    }

    #endregion

    #region Command Comments Tests

    [Fact]
    public void ParseSingle_WithCommandComment_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "1 M 3001 2 3010 * Load the guard\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        var cmd = zone.Commands[0];
        Assert.NotEmpty(cmd.Comment);
        Assert.Contains("Load the guard", cmd.Comment);
    }

    [Fact]
    public void ParseSingle_WithoutCommandComment_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "1 M 3001 2 3010\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Empty(zone.Commands[0].Comment);
    }

    #endregion

    #region Multiple Zones Tests

    [Fact]
    public void Parse_WithMultipleZones_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone One~\n" +
                    "0 99 30 1\n" +
                    "1 M 3001 2 3010\n" +
                    "S\n" +
                    "#200\n" +
                    "Zone Two~\n" +
                    "100 199 45 2\n" +
                    "1 O 3005 1 3010\n" +
                    "S\n";

        // Act
        var zones = _parser.Parse(input).ToList();

        // Assert
        Assert.Equal(2, zones.Count);
        Assert.Equal(100, zones[0].Vnumber);
        Assert.Equal(200, zones[1].Vnumber);
    }

    [Fact]
    public void Parse_WithThreeZones_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone One~\n" +
                    "0 99 30 1\n" +
                    "S\n" +
                    "#200\n" +
                    "Zone Two~\n" +
                    "100 199 45 2\n" +
                    "S\n" +
                    "#300\n" +
                    "Zone Three~\n" +
                    "200 299 60 1\n" +
                    "S\n";

        // Act
        var zones = _parser.Parse(input).ToList();

        // Assert
        Assert.Equal(3, zones.Count);
    }

    [Fact]
    public void Parse_WithMultipleZonesWithCommands_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone One~\n" +
                    "0 99 30 1\n" +
                    "1 M 3001 2 3010\n" +
                    "1 O 3005 1 3010\n" +
                    "S\n" +
                    "#200\n" +
                    "Zone Two~\n" +
                    "100 199 45 2\n" +
                    "1 E 3030 1 5\n" +
                    "1 G 3025 1 0\n" +
                    "S\n";

        // Act
        var zones = _parser.Parse(input).ToList();

        // Assert
        Assert.Equal(2, zones.Count);
        Assert.Equal(2, zones[0].Commands.Count);
        Assert.Equal(2, zones[1].Commands.Count);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void ParseSingle_WithSpecialCharactersInName_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "The Tavern's Rest~\n" +
                    "0 99 30 1\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Contains("Tavern", zone.Name);
    }

    [Fact]
    public void ParseSingle_WithExtraSpacesInName_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "   Zone   With   Spaces   ~\n" +
                    "0 99 30 1\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.NotEmpty(zone.Name);
    }

    [Fact]
    public void ParseSingle_WithTabsInCommands_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0\t99\t30\t1\n" +
                    "1\tM\t3001\t2\t3010\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Single(zone.Commands);
        Assert.Equal('M', zone.Commands[0].Command);
    }

    [Fact]
    public void ParseSingle_WithExtraSpacesInInfo_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "  0   99   30   1  \n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(0, zone.Bottom);
        Assert.Equal(99, zone.Top);
    }

    [Fact]
    public void ParseSingle_WithComplexZone_ParsesCorrectly()
    {
        // Arrange
        var input = "#3005\n" +
                    "The Dwarven Halls~\n" +
                    "3000 3099 30 2\n" +
                    "0 M 3001 2 3004 * Load the dwarven king\n" +
                    "1 M 3002 3 3005 * Load guards\n" +
                    "1 O 3001 1 3010 * Load treasure\n" +
                    "2 P 3015 1 3001 * Put in container\n" +
                    "1 E 3020 1 4 * Equip weapon\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(3005, zone.Vnumber);
        Assert.Equal("The Dwarven Halls", zone.Name);
        Assert.Equal(5, zone.Commands.Count);
    }

    [Fact]
    public void ParseSingle_WithLargeZone_ParsesCorrectly()
    {
        // Arrange
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("#9000");
        sb.AppendLine("Large Zone~");
        sb.AppendLine("9000 9099 30 1");
        for (int i = 0; i < 20; i++)
        {
            sb.AppendLine($"1 M {3000 + i} 1 {9000 + i}");
        }
        sb.AppendLine("S");

        // Act
        var zone = _parser.ParseSingle(sb.ToString());

        // Assert
        Assert.Equal(20, zone.Commands.Count);
    }

    #endregion

    #region Zone Name Tests

    [Fact]
    public void ParseSingle_WithEmptyName_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "~\n" +
                    "0 99 30 1\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Empty(zone.Name);
    }

    [Fact]
    public void ParseSingle_WithLongName_ParsesCorrectly()
    {
        // Arrange
        var input = "#100\n" +
                    "The Kingdom of the Northern Dwarven Halls of Moria~\n" +
                    "0 99 30 1\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Contains("Northern", zone.Name);
        Assert.Contains("Dwarven", zone.Name);
    }

    #endregion

    #region Various Reset Modes Tests

    [Theory]
    [InlineData(0)] // Never
    [InlineData(1)] // Only if empty
    [InlineData(2)] // Always
    public void ParseSingle_WithVariousResetModes_ParsesCorrectly(int resetMode)
    {
        // Arrange
        var input = $"#100\n" +
                    "Zone~\n" +
                    $"0 99 30 {resetMode}\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(resetMode, zone.ResetMode);
    }

    #endregion

    #region Various Command Types Tests

    [Theory]
    [InlineData('M')] // Load mob
    [InlineData('O')] // Load object
    [InlineData('P')] // Put in container
    [InlineData('G')] // Give to mob
    [InlineData('E')] // Equip on mob
    [InlineData('D')] // Remove object
    [InlineData('R')] // Remove from room
    public void ParseSingle_WithVariousCommandTypes_ParsesCorrectly(char cmdType)
    {
        // Arrange
        var input = $"#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    $"1 {cmdType} 100 1 50\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(cmdType, zone.Commands[0].Command);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public void ParseSingle_WithMissingVnumber_ThrowsException()
    {
        // Arrange
        var input = "no_vnumber\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "S\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithInvalidVnumber_ThrowsException()
    {
        // Arrange
        var input = "#abc\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "S\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithMissingName_ThrowsException()
    {
        // Arrange
        var input = "#100\n" +
                    "0 99 30 1\n" +
                    "S\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithNameWithoutTilde_ThrowsException()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone Name\n" +
                    "0 99 30 1\n" +
                    "S\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithMissingZoneInfo_ThrowsException()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "S\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithInvalidBottom_ThrowsException()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "abc 99 30 1\n" +
                    "S\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithInvalidTop_ThrowsException()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 xyz 30 1\n" +
                    "S\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithInvalidLifespan_ThrowsException()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 invalid 1\n" +
                    "S\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithInvalidResetMode_ThrowsException()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 bad\n" +
                    "S\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithInvalidCommand_ThrowsException()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "1 X 3001 2 3010\n" +
                    "S\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithInvalidIfFlag_ThrowsException()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "invalid M 3001 2 3010\n" +
                    "S\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithInvalidArg1_ThrowsException()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "1 M invalid 2 3010\n" +
                    "S\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithInsufficientCommandValues_ThrowsException()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30 1\n" +
                    "1 M 3001 2\n" +
                    "S\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithFewerThanFourInfoValues_ThrowsException()
    {
        // Arrange
        var input = "#100\n" +
                    "Zone~\n" +
                    "0 99 30\n" +
                    "S\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    #endregion

    #region Parse Method Tests

    [Fact]
    public void Parse_WithEmptyInput_ReturnsEmpty()
    {
        // Act
        var zones = _parser.Parse(string.Empty);

        // Assert
        Assert.Empty(zones);
    }

    [Fact]
    public void Parse_WithWhitespaceOnly_ReturnsEmpty()
    {
        // Arrange
        var input = "   \n   \n   \n";

        // Act
        var zones = _parser.Parse(input);

        // Assert
        Assert.Empty(zones);
    }

    [Fact]
    public void Parse_WithNoZoneMarker_ReturnsEmpty()
    {
        // Arrange
        var input = "random text\n" +
                    "more text\n" +
                    "no zones here\n";

        // Act
        var zones = _parser.Parse(input);

        // Assert
        Assert.Empty(zones);
    }

    #endregion

    #region Complex Zone Structure Tests

    [Fact]
    public void ParseSingle_WithComplexZoneStructure_ParsesCorrectly()
    {
        // Arrange
        var input = "#4000\n" +
                    "The Ancient Temple~\n" +
                    "4000 4099 60 2\n" +
                    "0 M 4001 1 4010 * Load the guardian\n" +
                    "1 M 4002 3 4015 * Load the priests\n" +
                    "1 M 4003 2 4020 * Load the monks\n" +
                    "1 O 4001 1 4010 * Load the sacred stone\n" +
                    "2 P 4001 1 4005 * Put in altar\n" +
                    "1 E 4010 1 0 * Equip to guardian\n" +
                    "1 D 4010 4001 0 * Remove if already present\n" +
                    "2 G 4020 1 0 * Give to priest\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(4000, zone.Vnumber);
        Assert.Equal("The Ancient Temple", zone.Name);
        Assert.Equal(8, zone.Commands.Count);
        Assert.All(zone.Commands, cmd => Assert.NotEmpty(cmd.Comment));
    }

    [Fact]
    public void ParseSingle_WithAllCommandTypesInOne_ParsesCorrectly()
    {
        // Arrange
        var input = "#5000\n" +
                    "Mixed Zone~\n" +
                    "5000 5099 45 1\n" +
                    "0 M 5001 1 5010\n" +
                    "1 O 5001 1 5010\n" +
                    "1 P 5002 1 5001\n" +
                    "1 G 5003 1 0\n" +
                    "1 E 5004 1 3\n" +
                    "1 D 5010 5001 0\n" +
                    "1 R 5020 5002 0\n" +
                    "S\n";

        // Act
        var zone = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(7, zone.Commands.Count);
        var cmds = zone.Commands.Select(c => c.Command).ToList();
        Assert.Contains('M', cmds);
        Assert.Contains('O', cmds);
        Assert.Contains('P', cmds);
        Assert.Contains('G', cmds);
        Assert.Contains('E', cmds);
        Assert.Contains('D', cmds);
        Assert.Contains('R', cmds);
    }

    #endregion
}
