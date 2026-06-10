using Xunit;
using System.Linq;
using CircleImporter.Core.Models;
using CircleImporter.Parsers.Objects;

namespace CircleImporter.Tests.Parsers
{
    public class ObjectParserTests
    {
        private readonly ObjectParser _parser;

        public ObjectParserTests()
        {
            _parser = new ObjectParser();
        }

        #region Basic Object Parsing Tests

        [Fact]
        public void ParseSingle_WithValidObject_ReturnsObject()
        {
            // Arrange
            var input = "#1000\n" +
                        "a simple object~\n" +
                        "A simple object is here.~\n" +
                        "This is a simple object used for testing.~\n" +
                        "~\n" +
                        "12 0 1\n" +
                        "100 50 10 0\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.NotNull(obj);
            Assert.Equal(1000, obj.Vnumber);
            Assert.Equal("a simple object", obj.Name);
            Assert.Equal("A simple object is here.", obj.ShortDescription);
            Assert.Equal("This is a simple object used for testing.", obj.LongDescription);
            Assert.Equal(12, obj.Type);
            Assert.Equal(0, obj.ExtraFlags);
            Assert.Equal(1, obj.WearFlags);
            Assert.Equal(100, obj.Weight);
            Assert.Equal(50, obj.Value);
            Assert.Equal(10, obj.Rent);
        }

        [Fact]
        public void ParseSingle_WithNullInput_ThrowsException()
        {
            // Act & Assert
#pragma warning disable CS8625
            Assert.Throws<System.ArgumentNullException>(() => _parser.ParseSingle(null));
#pragma warning restore CS8625
        }

        [Fact]
        public void ParseSingle_WithEmptyInput_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() => _parser.ParseSingle(""));
        }

        [Fact]
        public void ParseSingle_WithWhitespaceInput_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() => _parser.ParseSingle("   "));
        }

        #endregion

        #region Multi-line Description Tests

        [Fact]
        public void ParseSingle_WithMultilineDescription_ParsesCorrectly()
        {
            // Arrange
            var input = "#1001\n" +
                        "a sword~\n" +
                        "A shiny sword rests here.~\n" +
                        "A long, gleaming sword with intricate engravings.\n" +
                        "The blade appears to be well-maintained and ready for battle.~\n" +
                        "~\n" +
                        "5 0 8192\n" +
                        "50 250 25 0\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(1001, obj.Vnumber);
            Assert.Contains("well-maintained", obj.LongDescription);
            Assert.Contains("ready for battle", obj.LongDescription);
        }

        #endregion

        #region Type and Flags Tests

        [Fact]
        public void ParseSingle_WithArmorObjectType_ParsesCorrectly()
        {
            // Arrange
            var input = "#1002\n" +
                        "test item~\n" +
                        "A test item is here.~\n" +
                        "Test description.~\n" +
                        "~\n" +
                        "9 0 8\n" +
                        "50 100 10 0\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(9, obj.Type); // Armor
        }

        [Fact]
        public void ParseSingle_WithExtraFlags_ParsesCorrectly()
        {
            // Arrange
            var input = "#1003\n" +
                        "glowing ring~\n" +
                        "A glowing ring rests here.~\n" +
                        "The ring glows with a soft blue light.~\n" +
                        "~\n" +
                        "11 1 2\n" +
                        "10 500 50 0\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(1, obj.ExtraFlags); // Glow
        }

        [Fact]
        public void ParseSingle_WithWearFlags_ParsesCorrectly()
        {
            // Arrange
            var input = "#1004\n" +
                        "iron helmet~\n" +
                        "A sturdy iron helmet is here.~\n" +
                        "This iron helmet provides good protection.~\n" +
                        "~\n" +
                        "9 0 16\n" +
                        "100 200 20 0\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(16, obj.WearFlags); // Head
        }

        [Fact]
        public void ParseSingle_WithMultipleFlags_ParsesCorrectly()
        {
            // Arrange
            var input = "#1005\n" +
                        "blessed sword~\n" +
                        "A blessed sword rests here.~\n" +
                        "A holy sword blessed by the gods.~\n" +
                        "~\n" +
                        "5 256 8192\n" +
                        "60 500 50 0\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(256, obj.ExtraFlags); // Bless
            Assert.Equal(8192, obj.WearFlags); // Wield
        }

        #endregion

        #region Values Tests

        [Fact]
        public void ParseSingle_WithVariousValues_ParsesCorrectly()
        {
            // Arrange
            var input = "#1006\n" +
                        "test object~\n" +
                        "Test.~\n" +
                        "Test.~\n" +
                        "~\n" +
                        "12 0 1\n" +
                        "150 750 75 0\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(150, obj.Weight);
            Assert.Equal(750, obj.Value);
            Assert.Equal(75, obj.Rent);
            Assert.Equal(4, obj.Values.Count);
        }

        [Fact]
        public void ParseSingle_WithZeroValues_ParsesCorrectly()
        {
            // Arrange
            var input = "#1007\n" +
                        "free item~\n" +
                        "Free item.~\n" +
                        "Worthless item.~\n" +
                        "~\n" +
                        "12 0 1\n" +
                        "0 0 0 0\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(0, obj.Weight);
            Assert.Equal(0, obj.Value);
            Assert.Equal(0, obj.Rent);
        }

        [Fact]
        public void ParseSingle_WithLargeValues_ParsesCorrectly()
        {
            // Arrange
            var input = "#1008\n" +
                        "precious gem~\n" +
                        "A precious gem.~\n" +
                        "Extremely valuable.~\n" +
                        "~\n" +
                        "8 0 1\n" +
                        "5 50000 5000 0\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(5, obj.Weight);
            Assert.Equal(50000, obj.Value);
            Assert.Equal(5000, obj.Rent);
        }

        #endregion

        #region Affect Tests

        [Fact]
        public void ParseSingle_WithSingleAffect_ParsesCorrectly()
        {
            // Arrange
            var input = "#1009\n" +
                        "ring of strength~\n" +
                        "A ring of strength.~\n" +
                        "This ring glows with power.~\n" +
                        "~\n" +
                        "11 0 2\n" +
                        "10 500 50 0\n" +
                        "A\n" +
                        "0 2\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Single(obj.Affects);
            Assert.Equal(0, obj.Affects[0].Location);
            Assert.Equal(2, obj.Affects[0].Modifier);
        }

        [Fact]
        public void ParseSingle_WithMultipleAffects_ParsesCorrectly()
        {
            // Arrange
            var input = "#1010\n" +
                        "belt of power~\n" +
                        "A belt of power.~\n" +
                        "Glows with magical energy.~\n" +
                        "~\n" +
                        "11 64 2048\n" +
                        "20 1000 100 0\n" +
                        "A\n" +
                        "0 3\n" +
                        "A\n" +
                        "1 2\n" +
                        "A\n" +
                        "3 1\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(3, obj.Affects.Count);
            Assert.Equal(0, obj.Affects[0].Location);
            Assert.Equal(3, obj.Affects[0].Modifier);
            Assert.Equal(1, obj.Affects[1].Location);
            Assert.Equal(2, obj.Affects[1].Modifier);
            Assert.Equal(3, obj.Affects[2].Location);
            Assert.Equal(1, obj.Affects[2].Modifier);
        }

        [Fact]
        public void ParseSingle_WithNegativeAffectModifier_ParsesCorrectly()
        {
            // Arrange
            var input = "#1011\n" +
                        "cursed ring~\n" +
                        "A cursed ring.~\n" +
                        "This ring brings misfortune.~\n" +
                        "~\n" +
                        "11 0 2\n" +
                        "10 100 10 0\n" +
                        "A\n" +
                        "2 -1\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Single(obj.Affects);
            Assert.Equal(-1, obj.Affects[0].Modifier);
        }

        #endregion

        #region Multiple Objects Tests

        [Fact]
        public void Parse_WithMultipleObjects_ReturnsAllObjects()
        {
            // Arrange
            var input = "#1020\n" +
                        "first object~\n" +
                        "First.~\n" +
                        "First object.~\n" +
                        "~\n" +
                        "12 0 1\n" +
                        "50 100 10 0\n" +
                        "$\n" +
                        "#1021\n" +
                        "second object~\n" +
                        "Second.~\n" +
                        "Second object.~\n" +
                        "~\n" +
                        "12 0 1\n" +
                        "75 150 15 0\n" +
                        "$\n" +
                        "#1022\n" +
                        "third object~\n" +
                        "Third.~\n" +
                        "Third object.~\n" +
                        "~\n" +
                        "12 0 1\n" +
                        "100 200 20 0\n" +
                        "$\n";

            // Act
            var objects = _parser.Parse(input).ToList();

            // Assert
            Assert.Equal(3, objects.Count);
            Assert.Equal(1020, objects[0].Vnumber);
            Assert.Equal(1021, objects[1].Vnumber);
            Assert.Equal(1022, objects[2].Vnumber);
        }

        [Fact]
        public void Parse_WithEmptyInput_ReturnsEmptyList()
        {
            // Act
            var objects = _parser.Parse(string.Empty);

            // Assert
            Assert.Empty(objects);
        }

        [Fact]
        public void Parse_WithNullInput_ReturnsEmptyList()
        {
            // Act
#pragma warning disable CS8625
            var objects = _parser.Parse(null);
#pragma warning restore CS8625

            // Assert
            Assert.Empty(objects);
        }

        [Fact]
        public void Parse_WithMultipleObjectsAndAffects_ParsesCorrectly()
        {
            // Arrange
            var input = "#1030\n" +
                        "first item~\n" +
                        "First.~\n" +
                        "First.~\n" +
                        "~\n" +
                        "11 1 2\n" +
                        "10 500 50 0\n" +
                        "A\n" +
                        "0 1\n" +
                        "$\n" +
                        "#1031\n" +
                        "second item~\n" +
                        "Second.~\n" +
                        "Second.~\n" +
                        "~\n" +
                        "11 2 2\n" +
                        "15 750 75 0\n" +
                        "A\n" +
                        "1 2\n" +
                        "A\n" +
                        "3 1\n" +
                        "$\n";

            // Act
            var objects = _parser.Parse(input).ToList();

            // Assert
            Assert.Equal(2, objects.Count);
            Assert.Single(objects[0].Affects);
            Assert.Equal(2, objects[1].Affects.Count);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void ParseSingle_WithActionDescription_ParsesCorrectly()
        {
            // Arrange
            var input = "#1040\n" +
                        "glowing orb~\n" +
                        "A glowing orb rests here.~\n" +
                        "This magical orb radiates soft light.~\n" +
                        "It pulses with an otherworldly glow.~\n" +
                        "12 1 1\n" +
                        "20 300 30 0\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Equal("It pulses with an otherworldly glow.", obj.ActionDescription);
        }

        [Fact]
        public void ParseSingle_WithEmptyActionDescription_ParsesCorrectly()
        {
            // Arrange
            var input = "#1041\n" +
                        "simple item~\n" +
                        "Item.~\n" +
                        "Description.~\n" +
                        "~\n" +
                        "12 0 1\n" +
                        "50 100 10 0\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Empty(obj.ActionDescription);
        }

        [Fact]
        public void ParseSingle_WithSpecialCharactersInName_ParsesCorrectly()
        {
            // Arrange
            var input = "#1042\n" +
                        "King's Ring~\n" +
                        "A ring.~\n" +
                        "Description.~\n" +
                        "~\n" +
                        "11 0 2\n" +
                        "10 500 50 0\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Equal("King's Ring", obj.Name);
        }

        [Fact]
        public void ParseSingle_WithSpecialCharactersInDescription_ParsesCorrectly()
        {
            // Arrange
            var input = "#1043\n" +
                        "scroll~\n" +
                        "A scroll.~\n" +
                        "Written text: 'cast fireball' on the parchment.~\n" +
                        "~\n" +
                        "2 0 1\n" +
                        "5 50 5 0\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Contains("'cast fireball'", obj.LongDescription);
        }

        [Fact]
        public void ParseSingle_WithEmptyName_ParsesCorrectly()
        {
            // Arrange
            var input = "#1044\n" +
                        "~\n" +
                        "Something here.~\n" +
                        "A mysterious object.~\n" +
                        "~\n" +
                        "12 0 1\n" +
                        "50 100 10 0\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Empty(obj.Name);
        }

        [Fact]
        public void ParseSingle_WithEmptyShortDescription_ParsesCorrectly()
        {
            // Arrange
            var input = "#1045\n" +
                        "object~\n" +
                        "~\n" +
                        "Long description here.~\n" +
                        "~\n" +
                        "12 0 1\n" +
                        "50 100 10 0\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Empty(obj.ShortDescription);
        }

        [Fact]
        public void ParseSingle_WithLargeVnumber_ParsesCorrectly()
        {
            // Arrange
            var input = "#99999\n" +
                        "treasure~\n" +
                        "Treasure.~\n" +
                        "Valuable item.~\n" +
                        "~\n" +
                        "8 0 1\n" +
                        "100 10000 1000 0\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(99999, obj.Vnumber);
        }

        [Fact]
        public void ParseSingle_WithZeroVnumber_ParsesCorrectly()
        {
            // Arrange
            var input = "#0\n" +
                        "nothing~\n" +
                        "Nothing.~\n" +
                        "Empty.~\n" +
                        "~\n" +
                        "12 0 1\n" +
                        "0 0 0 0\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(0, obj.Vnumber);
        }

        [Fact]
        public void ParseSingle_WithWhitespaceAndNewlines_ParsesCorrectly()
        {
            // Arrange
            var input = "  #1050  \n" +
                        "  object name  ~\n" +
                        "  Short desc  ~\n" +
                        "  Long desc  ~\n" +
                        "  Action  ~\n" +
                        "  12 0 1  \n" +
                        "  50 100 10 0  \n" +
                        "  $  \n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(1050, obj.Vnumber);
            Assert.NotEmpty(obj.Name);
        }

        [Fact]
        public void ParseSingle_WithComplexObject_ParsesSuccessfully()
        {
            // Arrange - Realistic complex object
            var input = "#3000\n" +
                        "a bastard sword~\n" +
                        "A bastard sword lies on the ground.~\n" +
                        "This is a finely crafted bastard sword with intricate engravings.\n" +
                        "The blade gleams with a magical sheen, and the hilt is wrapped in\n" +
                        "soft leather. It appears to be both beautiful and deadly.~\n" +
                        "You feel power surge through you as you grip the sword.~\n" +
                        "5 64 8192\n" +
                        "80 750 75 0\n" +
                        "A\n" +
                        "0 2\n" +
                        "A\n" +
                        "1 1\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(3000, obj.Vnumber);
            Assert.Equal("a bastard sword", obj.Name);
            Assert.Equal(5, obj.Type);
            Assert.Equal(64, obj.ExtraFlags); // Magic
            Assert.Equal(8192, obj.WearFlags); // Wield
            Assert.Equal(80, obj.Weight);
            Assert.Equal(750, obj.Value);
            Assert.Equal(75, obj.Rent);
            Assert.Equal(2, obj.Affects.Count);
            Assert.Contains("intricate engravings", obj.LongDescription);
            Assert.Contains("magical sheen", obj.LongDescription);
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public void ParseSingle_WithInvalidVnumber_ThrowsException()
        {
            // Arrange
            var input = "#INVALID\n" +
                        "object~\n" +
                        "Obj.~\n" +
                        "Desc.~\n" +
                        "~\n" +
                        "12 0 1\n" +
                        "50 100 10\n" +
                        "$\n";

            // Act & Assert
            Assert.Throws<System.InvalidOperationException>(() => _parser.ParseSingle(input));
        }

        [Fact]
        public void ParseSingle_WithMissingTypeLine_ThrowsException()
        {
            // Arrange
            var input = "#1060\n" +
                        "object~\n" +
                        "Obj.~\n" +
                        "Desc.~\n" +
                        "~\n" +
                        "50 100 10\n" +
                        "$\n";

            // Act & Assert
            Assert.Throws<System.InvalidOperationException>(() => _parser.ParseSingle(input));
        }

        [Fact]
        public void ParseSingle_WithInvalidType_ThrowsException()
        {
            // Arrange
            var input = "#1061\n" +
                        "object~\n" +
                        "Obj.~\n" +
                        "Desc.~\n" +
                        "~\n" +
                        "INVALID 0 1\n" +
                        "50 100 10\n" +
                        "$\n";

            // Act & Assert
            Assert.Throws<System.InvalidOperationException>(() => _parser.ParseSingle(input));
        }

        [Fact]
        public void ParseSingle_WithInvalidExtraFlags_ThrowsException()
        {
            // Arrange
            var input = "#1062\n" +
                        "object~\n" +
                        "Obj.~\n" +
                        "Desc.~\n" +
                        "~\n" +
                        "12 INVALID 1\n" +
                        "50 100 10\n" +
                        "$\n";

            // Act & Assert
            Assert.Throws<System.InvalidOperationException>(() => _parser.ParseSingle(input));
        }

        [Fact]
        public void ParseSingle_WithInvalidWearFlags_ThrowsException()
        {
            // Arrange
            var input = "#1063\n" +
                        "object~\n" +
                        "Obj.~\n" +
                        "Desc.~\n" +
                        "~\n" +
                        "12 0 INVALID\n" +
                        "50 100 10\n" +
                        "$\n";

            // Act & Assert
            Assert.Throws<System.InvalidOperationException>(() => _parser.ParseSingle(input));
        }

        [Fact]
        public void ParseSingle_WithMissingValuesLine_ThrowsException()
        {
            // Arrange
            var input = "#1064\n" +
                        "object~\n" +
                        "Obj.~\n" +
                        "Desc.~\n" +
                        "~\n" +
                        "12 0 1\n" +
                        "$\n";

            // Act & Assert
            Assert.Throws<System.InvalidOperationException>(() => _parser.ParseSingle(input));
        }

        [Fact]
        public void ParseSingle_WithInvalidObjectValue_ThrowsException()
        {
            // Arrange
            var input = "#1065\n" +
                        "object~\n" +
                        "Obj.~\n" +
                        "Desc.~\n" +
                        "~\n" +
                        "12 0 1\n" +
                        "INVALID 100 10 0\n" +
                        "$\n";

            // Act & Assert
            Assert.Throws<System.InvalidOperationException>(() => _parser.ParseSingle(input));
        }

        [Fact]
        public void ParseSingle_WithInvalidAffectLocation_ThrowsException()
        {
            // Arrange
            var input = "#1066\n" +
                        "object~\n" +
                        "Obj.~\n" +
                        "Desc.~\n" +
                        "~\n" +
                        "12 0 1\n" +
                        "50 100 10\n" +
                        "A\n" +
                        "INVALID 1\n" +
                        "$\n";

            // Act & Assert
            Assert.Throws<System.InvalidOperationException>(() => _parser.ParseSingle(input));
        }

        [Fact]
        public void ParseSingle_WithInvalidAffectModifier_ThrowsException()
        {
            // Arrange
            var input = "#1067\n" +
                        "object~\n" +
                        "Obj.~\n" +
                        "Desc.~\n" +
                        "~\n" +
                        "12 0 1\n" +
                        "50 100 10\n" +
                        "A\n" +
                        "0 INVALID\n" +
                        "$\n";

            // Act & Assert
            Assert.Throws<System.InvalidOperationException>(() => _parser.ParseSingle(input));
        }

        #endregion

        #region Various Object Type Tests

        [Theory]
        [InlineData(1)] // Light
        [InlineData(2)] // Scroll
        [InlineData(5)] // Weapon
        [InlineData(9)] // Armor
        [InlineData(12)] // Other
        [InlineData(15)] // Container
        [InlineData(19)] // Food
        public void ParseSingle_WithVariousObjectTypes_ParsesCorrectly(int objectType)
        {
            // Arrange
            var input = "#1070\n" +
                        "item~\n" +
                        "Item.~\n" +
                        "Description.~\n" +
                        "~\n" +
                        $"{objectType} 0 1\n" +
                        "50 100 10 0\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(objectType, obj.Type);
        }

        #endregion

        #region Fourth Value Test

        [Fact]
        public void ParseSingle_WithAllFourValues_ParsesCorrectly()
        {
            // Arrange
            var input = "#1080\n" +
                        "container~\n" +
                        "A container.~\n" +
                        "A storage container.~\n" +
                        "~\n" +
                        "15 0 1\n" +
                        "100 200 20 50\n" +
                        "$\n";

            // Act
            var obj = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(4, obj.Values.Count);
            Assert.Equal(100, obj.Values[0]);
            Assert.Equal(200, obj.Values[1]);
            Assert.Equal(20, obj.Values[2]);
            Assert.Equal(50, obj.Values[3]);
        }

        #endregion
    }
}
