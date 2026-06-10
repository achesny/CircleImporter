using CircleImporter.Core.Models;
using CircleImporter.Parsers.Mobs;
using Xunit;

namespace CircleImporter.Tests;

public class MobParserTests
{
    private readonly MobParser _parser = new();

    #region Basic Parsing Tests

    [Fact]
    public void ParseSingle_WithValidMob_ParsesCorrectly()
    {
        // Arrange
        var input = "#3001\n" +
                    "guard captain~\n" +
                    "Guard Captain~\n" +
                    "A burly captain of the guards~\n" +
                    "~\n" +
                    "0 1 3 20 2 3 -10 150 50 80\n" +
                    "500 2000 8 8 0 50\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(3001, mob.Vnumber);
        Assert.Equal("guard captain", mob.Name);
        Assert.Equal("Guard Captain", mob.ShortDescription);
        Assert.Equal("A burly captain of the guards", mob.LongDescription);
        Assert.Empty(mob.ActionDescription);
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

    #region Multi-line Description Tests

    [Fact]
    public void ParseSingle_WithMultilineDescription_ParsesCorrectly()
    {
        // Arrange
        var input = "#3002\n" +
                    "warrior veteran~\n" +
                    "Warrior Veteran~\n" +
                    "This grizzled veteran bears the scars\n" +
                    "of many battles fought in distant lands.~\n" +
                    "~\n" +
                    "1 0 3 25 5 6 -15 200 100 100\n" +
                    "1000 5000 8 8 2 100\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Contains("scars", mob.LongDescription);
        Assert.Contains("many battles", mob.LongDescription);
    }

    #endregion

    #region Stats Line Tests

    [Fact]
    public void ParseSingle_WithAllStats_ParsesCorrectly()
    {
        // Arrange
        var input = "#3003\n" +
                    "mage~\n" +
                    "Mage~\n" +
                    "An arcane mage~\n" +
                    "~\n" +
                    "2 1 0 30 1 2 -20 180 200 60\n" +
                    "2000 8000 8 8 4 0\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(2, mob.Race);
        Assert.Equal(1, mob.Sex);
        Assert.Equal(0, mob.Class);
        Assert.Equal(30, mob.Level);
        Assert.Equal(1, mob.Hitroll);
        Assert.Equal(2, mob.Damroll);
        Assert.Equal(-20, mob.Ac);
        Assert.Equal(180, mob.Hp);
        Assert.Equal(200, mob.Mana);
        Assert.Equal(60, mob.Move);
    }

    [Fact]
    public void ParseSingle_WithNegativeStats_ParsesCorrectly()
    {
        // Arrange
        var input = "#3004\n" +
                    "cursed~\n" +
                    "Cursed One~\n" +
                    "A cursed creature~\n" +
                    "~\n" +
                    "5 2 1 15 -5 -3 -5 100 75 40\n" +
                    "100 500 8 8 0 -100\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(-5, mob.Hitroll);
        Assert.Equal(-3, mob.Damroll);
        Assert.Equal(-100, mob.Alignment);
    }

    [Fact]
    public void ParseSingle_WithZeroStats_ParsesCorrectly()
    {
        // Arrange
        var input = "#3005\n" +
                    "nothing~\n" +
                    "Nothing~\n" +
                    "A nothing~\n" +
                    "~\n" +
                    "0 0 0 0 0 0 0 1 0 0\n" +
                    "0 0 8 8 0 0\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(0, mob.Race);
        Assert.Equal(0, mob.Sex);
        Assert.Equal(0, mob.Class);
        Assert.Equal(0, mob.Level);
        Assert.Equal(0, mob.Gold);
        Assert.Equal(0, mob.Experience);
    }

    [Fact]
    public void ParseSingle_WithLargeStats_ParsesCorrectly()
    {
        // Arrange
        var input = "#3006\n" +
                    "demon lord~\n" +
                    "Demon Lord~\n" +
                    "An ancient demon lord~\n" +
                    "~\n" +
                    "6 1 3 50 15 20 -50 5000 2000 500\n" +
                    "100000 500000 8 8 1024 -1000\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(50, mob.Level);
        Assert.Equal(5000, mob.Hp);
        Assert.Equal(100000, mob.Gold);
        Assert.Equal(500000, mob.Experience);
    }

    #endregion

    #region Additional Values Tests

    [Fact]
    public void ParseSingle_WithAllPositions_ParsesCorrectly()
    {
        // Arrange
        var input = "#3007\n" +
                    "sleeper~\n" +
                    "Sleeper~\n" +
                    "A sleeping figure~\n" +
                    "~\n" +
                    "1 1 1 10 0 0 0 50 30 20\n" +
                    "100 1000 4 5 0 0\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(4, mob.Position);
        Assert.Equal(5, mob.DefaultPosition);
    }

    [Fact]
    public void ParseSingle_WithMobFlags_ParsesCorrectly()
    {
        // Arrange
        var input = "#3008\n" +
                    "sentinel~\n" +
                    "Sentinel~\n" +
                    "An immobile sentinel~\n" +
                    "~\n" +
                    "0 0 0 10 0 0 0 50 30 20\n" +
                    "0 1000 8 8 2 0\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(2, mob.Flags);
    }

    [Fact]
    public void ParseSingle_WithPositiveAlignment_ParsesCorrectly()
    {
        // Arrange
        var input = "#3009\n" +
                    "paladin~\n" +
                    "Holy Paladin~\n" +
                    "A righteous paladin~\n" +
                    "~\n" +
                    "1 1 5 20 3 4 -15 150 100 80\n" +
                    "1000 3000 8 8 0 1000\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(1000, mob.Alignment);
    }

    [Fact]
    public void ParseSingle_WithNegativeAlignment_ParsesCorrectly()
    {
        // Arrange
        var input = "#3010\n" +
                    "rogue~\n" +
                    "Evil Rogue~\n" +
                    "A treacherous rogue~\n" +
                    "~\n" +
                    "1 1 2 18 2 3 -10 100 50 70\n" +
                    "800 2500 8 8 0 -500\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(-500, mob.Alignment);
    }

    #endregion

    #region Action Description Tests

    [Fact]
    public void ParseSingle_WithActionDescription_ParsesCorrectly()
    {
        // Arrange
        var input = "#3011\n" +
                    "dragon~\n" +
                    "Ancient Dragon~\n" +
                    "An ancient wyrm~\n" +
                    "A stream of dragon fire erupts from its maw!~\n" +
                    "4 1 3 40 10 15 -30 400 300 200\n" +
                    "5000 15000 8 8 16 100\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Equal("A stream of dragon fire erupts from its maw!", mob.ActionDescription);
    }

    [Fact]
    public void ParseSingle_WithEmptyActionDescription_ParsesCorrectly()
    {
        // Arrange
        var input = "#3012\n" +
                    "commoner~\n" +
                    "Commoner~\n" +
                    "A simple commoner~\n" +
                    "~\n" +
                    "0 0 0 5 0 0 0 30 20 10\n" +
                    "50 250 8 8 0 0\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Empty(mob.ActionDescription);
    }

    #endregion

    #region Multiple Mobs Tests

    [Fact]
    public void Parse_WithMultipleMobs_ParsesCorrectly()
    {
        // Arrange
        var input = "#3020\n" +
                    "guard~\n" +
                    "Guard~\n" +
                    "A guard~\n" +
                    "~\n" +
                    "0 0 3 10 0 0 0 50 30 20\n" +
                    "100 500 8 8 0 0\n" +
                    "$\n" +
                    "#3021\n" +
                    "mage~\n" +
                    "Mage~\n" +
                    "A mage~\n" +
                    "~\n" +
                    "2 1 0 15 1 1 -5 60 80 30\n" +
                    "200 1000 8 8 0 0\n" +
                    "$\n";

        // Act
        var mobs = _parser.Parse(input).ToList();

        // Assert
        Assert.Equal(2, mobs.Count);
        Assert.Equal(3020, mobs[0].Vnumber);
        Assert.Equal(3021, mobs[1].Vnumber);
    }

    [Fact]
    public void Parse_WithMultipleMobsWithAffects_ParsesCorrectly()
    {
        // Arrange
        var input = "#3030\n" +
                    "warrior~\n" +
                    "Warrior~\n" +
                    "A warrior~\n" +
                    "~\n" +
                    "1 0 3 20 2 3 -10 150 50 80\n" +
                    "500 2000 8 8 0 50\n" +
                    "$\n" +
                    "#3031\n" +
                    "cleric~\n" +
                    "Cleric~\n" +
                    "A cleric~\n" +
                    "~\n" +
                    "0 1 1 18 1 2 -8 120 100 60\n" +
                    "300 1500 8 8 0 0\n" +
                    "$\n" +
                    "#3032\n" +
                    "thief~\n" +
                    "Thief~\n" +
                    "A thief~\n" +
                    "~\n" +
                    "1 0 2 16 3 1 -5 80 40 100\n" +
                    "250 1200 8 8 0 -100\n" +
                    "$\n";

        // Act
        var mobs = _parser.Parse(input).ToList();

        // Assert
        Assert.Equal(3, mobs.Count);
        Assert.All(mobs, m => Assert.NotEqual(0, m.Vnumber));
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void ParseSingle_WithSpecialCharactersInName_ParsesCorrectly()
    {
        // Arrange
        var input = "#3040\n" +
                    "orc warrior!~\n" +
                    "Orc Warrior!~\n" +
                    "An orc with strange symbols~\n" +
                    "~\n" +
                    "9 0 3 22 4 5 -12 180 60 90\n" +
                    "750 3500 8 8 2 -250\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Contains("!", mob.Name);
        Assert.Contains("!", mob.ShortDescription);
    }

    [Fact]
    public void ParseSingle_WithEmptyNameFields_ParsesCorrectly()
    {
        // Arrange
        var input = "#3041\n" +
                    "~\n" +
                    "~\n" +
                    "Something~\n" +
                    "~\n" +
                    "0 0 0 5 0 0 0 30 20 10\n" +
                    "50 250 8 8 0 0\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Empty(mob.Name);
        Assert.Empty(mob.ShortDescription);
        Assert.NotEmpty(mob.LongDescription);
    }

    [Fact]
    public void ParseSingle_WithNumbersInDescriptions_ParsesCorrectly()
    {
        // Arrange
        var input = "#3042\n" +
                    "level 50 dragon~\n" +
                    "Ancient Dragon~\n" +
                    "A dragon of level 50~\n" +
                    "~\n" +
                    "6 1 3 50 10 15 -50 5000 2000 500\n" +
                    "100000 500000 8 8 1024 -1000\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Contains("50", mob.Name);
        Assert.Contains("50", mob.LongDescription);
    }

    [Fact]
    public void ParseSingle_WithLargeVnumber_ParsesCorrectly()
    {
        // Arrange
        var input = "#99999\n" +
                    "high vnum~\n" +
                    "High Vnumber~\n" +
                    "A mob with high vnumber~\n" +
                    "~\n" +
                    "0 0 0 10 0 0 0 50 30 20\n" +
                    "100 1000 8 8 0 0\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(99999, mob.Vnumber);
    }

    [Fact]
    public void ParseSingle_WithZeroVnumber_ParsesCorrectly()
    {
        // Arrange
        var input = "#0\n" +
                    "none~\n" +
                    "None~\n" +
                    "Nothing~\n" +
                    "~\n" +
                    "0 0 0 1 0 0 0 1 0 0\n" +
                    "0 0 8 8 0 0\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(0, mob.Vnumber);
    }

    [Fact]
    public void ParseSingle_WithWhitespaceAndNewlines_ParsesCorrectly()
    {
        // Arrange
        var input = "#3050\n" +
                    "  spaced  ~\n" +
                    "Spaced~\n" +
                    "  Description  ~\n" +
                    "~\n" +
                    "  0  0  0  5  0  0  0  30  20  10  \n" +
                    "  50  250  8  8  0  0  \n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.NotEmpty(mob.Name);
        Assert.NotEmpty(mob.ShortDescription);
        Assert.Equal(5, mob.Level);
    }

    [Fact]
    public void ParseSingle_WithComplexMultilineDescription_ParsesCorrectly()
    {
        // Arrange
        var input = "#3051\n" +
                    "noble~\n" +
                    "Merchant Noble~\n" +
                    "This wealthy merchant wears fine silks\n" +
                    "and carries a staff adorned with jewels.\n" +
                    "He exudes an air of nobility and power.~\n" +
                    "~\n" +
                    "2 1 0 25 2 2 -12 140 150 70\n" +
                    "2000 5000 8 8 0 200\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Contains("silks", mob.LongDescription);
        Assert.Contains("jewels", mob.LongDescription);
        Assert.Contains("nobility", mob.LongDescription);
    }

    #endregion

    #region Various Race/Sex/Class Tests

    [Theory]
    [InlineData(0)] // Undefined
    [InlineData(1)] // Human
    [InlineData(3)] // Dwarf
    [InlineData(6)] // Dragon
    public void ParseSingle_WithVariousRaces_ParsesCorrectly(int race)
    {
        // Arrange
        var input = "#3060\n" +
                    "creature~\n" +
                    "Creature~\n" +
                    "A creature~\n" +
                    "~\n" +
                    $"{race} 1 0 15 1 1 -5 60 80 30\n" +
                    "200 1000 8 8 0 0\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(race, mob.Race);
    }

    [Theory]
    [InlineData(0)] // Neutral
    [InlineData(1)] // Male
    [InlineData(2)] // Female
    public void ParseSingle_WithVariousSexes_ParsesCorrectly(int sex)
    {
        // Arrange
        var input = "#3061\n" +
                    "being~\n" +
                    "Being~\n" +
                    "A being~\n" +
                    "~\n" +
                    $"0 {sex} 1 15 1 1 -5 60 80 30\n" +
                    "200 1000 8 8 0 0\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(sex, mob.Sex);
    }

    [Theory]
    [InlineData(0)] // Magic User
    [InlineData(1)] // Cleric
    [InlineData(2)] // Thief
    [InlineData(3)] // Warrior
    public void ParseSingle_WithVariousClasses_ParsesCorrectly(int mobClass)
    {
        // Arrange
        var input = "#3062\n" +
                    "adventurer~\n" +
                    "Adventurer~\n" +
                    "An adventurer~\n" +
                    "~\n" +
                    $"1 1 {mobClass} 15 1 1 -5 60 80 30\n" +
                    "200 1000 8 8 0 0\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(mobClass, mob.Class);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public void ParseSingle_WithMissingVnumber_ThrowsException()
    {
        // Arrange
        var input = "no_vnumber\n" +
                    "guard~\n" +
                    "Guard~\n" +
                    "A guard~\n" +
                    "~\n" +
                    "0 0 3 10 0 0 0 50 30 20\n" +
                    "100 500 8 8 0 0\n" +
                    "$\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithInvalidVnumber_ThrowsException()
    {
        // Arrange
        var input = "#abc\n" +
                    "guard~\n" +
                    "Guard~\n" +
                    "A guard~\n" +
                    "~\n" +
                    "0 0 3 10 0 0 0 50 30 20\n" +
                    "100 500 8 8 0 0\n" +
                    "$\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithMissingStatsLine_ThrowsException()
    {
        // Arrange
        var input = "#3070\n" +
                    "guard~\n" +
                    "Guard~\n" +
                    "A guard~\n" +
                    "~\n" +
                    "$\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithInvalidRace_ThrowsException()
    {
        // Arrange
        var input = "#3071\n" +
                    "guard~\n" +
                    "Guard~\n" +
                    "A guard~\n" +
                    "~\n" +
                    "abc 0 3 10 0 0 0 50 30 20\n" +
                    "100 500 8 8 0 0\n" +
                    "$\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithInvalidClass_ThrowsException()
    {
        // Arrange
        var input = "#3072\n" +
                    "guard~\n" +
                    "Guard~\n" +
                    "A guard~\n" +
                    "~\n" +
                    "0 0 xyz 10 0 0 0 50 30 20\n" +
                    "100 500 8 8 0 0\n" +
                    "$\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithInvalidLevel_ThrowsException()
    {
        // Arrange
        var input = "#3073\n" +
                    "guard~\n" +
                    "Guard~\n" +
                    "A guard~\n" +
                    "~\n" +
                    "0 0 3 not_a_number 0 0 0 50 30 20\n" +
                    "100 500 8 8 0 0\n" +
                    "$\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithMissingAdditionalLine_ThrowsException()
    {
        // Arrange
        var input = "#3074\n" +
                    "guard~\n" +
                    "Guard~\n" +
                    "A guard~\n" +
                    "~\n" +
                    "0 0 3 10 0 0 0 50 30 20\n" +
                    "$\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithInvalidGold_ThrowsException()
    {
        // Arrange
        var input = "#3075\n" +
                    "guard~\n" +
                    "Guard~\n" +
                    "A guard~\n" +
                    "~\n" +
                    "0 0 3 10 0 0 0 50 30 20\n" +
                    "invalid 500 8 8 0 0\n" +
                    "$\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithInvalidPosition_ThrowsException()
    {
        // Arrange
        var input = "#3076\n" +
                    "guard~\n" +
                    "Guard~\n" +
                    "A guard~\n" +
                    "~\n" +
                    "0 0 3 10 0 0 0 50 30 20\n" +
                    "100 500 bad_position 8 0 0\n" +
                    "$\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithFewerThanTenStatsValues_ThrowsException()
    {
        // Arrange
        var input = "#3077\n" +
                    "guard~\n" +
                    "Guard~\n" +
                    "A guard~\n" +
                    "~\n" +
                    "0 0 3 10 0 0\n" +
                    "100 500 8 8 0 0\n" +
                    "$\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    [Fact]
    public void ParseSingle_WithFewerThanSixAdditionalValues_ThrowsException()
    {
        // Arrange
        var input = "#3078\n" +
                    "guard~\n" +
                    "Guard~\n" +
                    "A guard~\n" +
                    "~\n" +
                    "0 0 3 10 0 0 0 50 30 20\n" +
                    "100 500 8 8\n" +
                    "$\n";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _parser.ParseSingle(input));
    }

    #endregion

    #region Parse Method Tests

    [Fact]
    public void Parse_WithEmptyInput_ReturnsEmpty()
    {
        // Act
        var mobs = _parser.Parse(string.Empty);

        // Assert
        Assert.Empty(mobs);
    }

    [Fact]
    public void Parse_WithWhitespaceOnly_ReturnsEmpty()
    {
        // Arrange
        var input = "   \n   \n   \n";

        // Act
        var mobs = _parser.Parse(input);

        // Assert
        Assert.Empty(mobs);
    }

    [Fact]
    public void Parse_WithNoMobMarker_ReturnsEmpty()
    {
        // Arrange
        var input = "random text\n" +
                    "more text\n" +
                    "no mobs here\n";

        // Act
        var mobs = _parser.Parse(input);

        // Assert
        Assert.Empty(mobs);
    }

    #endregion

    #region Specific Field Tests

    [Fact]
    public void ParseSingle_WithMaximumValues_ParsesCorrectly()
    {
        // Arrange
        var input = "#100000\n" +
                    "maximum~\n" +
                    "Maximum~\n" +
                    "Maximum values~\n" +
                    "~\n" +
                    "20 2 9 100 99 99 -99 9999 9999 9999\n" +
                    "999999 999999 8 8 1048575 1000\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(100000, mob.Vnumber);
        Assert.Equal(100, mob.Level);
        Assert.Equal(9999, mob.Hp);
        Assert.Equal(999999, mob.Gold);
        Assert.Equal(1048575, mob.Flags);
    }

    [Fact]
    public void ParseSingle_AllMobFlagsPresets_ParsesCorrectly()
    {
        // Arrange - test with various flag combinations
        var input = "#3090\n" +
                    "sentinel scavenger~\n" +
                    "Sentinel Scavenger~\n" +
                    "A mob with multiple flags~\n" +
                    "~\n" +
                    "0 0 0 10 0 0 0 50 30 20\n" +
                    "100 1000 8 8 6 0\n" +
                    "$\n";

        // Act
        var mob = _parser.ParseSingle(input);

        // Assert
        Assert.Equal(6, mob.Flags); // Sentinel (2) + Scavenger (4)
    }

    #endregion
}
