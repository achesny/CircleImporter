using Xunit;
using System.Linq;
using CircleImporter.Core.Models;
using CircleImporter.Parsers.World;

namespace CircleImporter.Tests.Parsers
{
    public class RoomParserTests
    {
        private readonly RoomParser _parser;

        public RoomParserTests()
        {
            _parser = new RoomParser();
        }

        #region Basic Room Parsing Tests

        [Fact]
        public void ParseSingle_WithValidRoom_ReturnsRoom()
        {
            // Arrange
            var input = "#3001\n" +
                        "The Great Hall~\n" +
                        "This is a grand hall.~\n" +
                        "0 0\n" +
                        "S\n";

            // Act
            var room = _parser.ParseSingle(input);

            // Assert
            Assert.NotNull(room);
            Assert.Equal(3001, room.Vnumber);
            Assert.Equal("The Great Hall", room.Name);
            Assert.Equal("This is a grand hall.", room.Description);
            Assert.Equal(0, room.RoomFlags);
            Assert.Equal(0, room.SectorType);
            Assert.Empty(room.Exits);
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
            var input = "#3002\n" +
                        "Tavern~\n" +
                        "A warm, welcoming tavern with a roaring fireplace.\n" +
                        "The smell of ale and roasted meat fills the air.~\n" +
                        "0 1\n" +
                        "S\n";

            // Act
            var room = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(3002, room.Vnumber);
            Assert.Equal("Tavern", room.Name);
            Assert.Contains("warm, welcoming tavern", room.Description);
            Assert.Contains("smell of ale and roasted meat", room.Description);
        }

        #endregion

        #region Room Flags and Sector Type Tests

        [Fact]
        public void ParseSingle_WithRoomFlags_ParsesCorrectly()
        {
            // Arrange
            var input = "#3003\n" +
                        "Dark Room~\n" +
                        "A dark and foreboding chamber.~\n" +
                        "1 0\n" +
                        "S\n";

            // Act
            var room = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(1, room.RoomFlags); // Dark flag
            Assert.Equal(0, room.SectorType); // Inside
        }

        [Fact]
        public void ParseSingle_WithMultipleFlagsAndSectorType_ParsesCorrectly()
        {
            // Arrange
            var input = "#3004\n" +
                        "Forest Path~\n" +
                        "A winding path through the forest.~\n" +
                        "32 3\n" +
                        "S\n";

            // Act
            var room = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(32, room.RoomFlags); // Soundproof
            Assert.Equal(3, room.SectorType); // Forest
        }

        [Theory]
        [InlineData(0)] // Inside
        [InlineData(1)] // City
        [InlineData(2)] // Field
        [InlineData(3)] // Forest
        [InlineData(4)] // Hills
        [InlineData(5)] // Mountains
        [InlineData(6)] // Water Swim
        [InlineData(7)] // Water No Swim
        [InlineData(8)] // Underwater
        [InlineData(9)] // Flying
        public void ParseSingle_WithVariousSectorTypes_ParsesCorrectly(int sectorType)
        {
            // Arrange
            var input = $"#3005\n" +
                        "Test Room~\n" +
                        "Test description.~\n" +
                        $"0 {sectorType}\n" +
                        "S\n";

            // Act
            var room = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(sectorType, room.SectorType);
        }

        #endregion

        #region Exit Parsing Tests

        [Fact]
        public void ParseSingle_WithSingleExit_ParsesCorrectly()
        {
            // Arrange
            var input = "#3006\n" +
                        "Room with Exit~\n" +
                        "A room with an exit.~\n" +
                        "0 0\n" +
                        "D0\n" +
                        "~\n" +
                        "~\n" +
                        "0 0 3007\n" +
                        "S\n";

            // Act
            var room = _parser.ParseSingle(input);

            // Assert
            Assert.Single(room.Exits);
            var exit = room.Exits.First();
            Assert.Equal(0, exit.Direction); // North
            Assert.Equal(3007, exit.DestinationVnum);
            Assert.Empty(exit.Description);
            Assert.Empty(exit.Keywords);
        }

        [Fact]
        public void ParseSingle_WithMultipleExits_ParsesCorrectly()
        {
            // Arrange
            var input = "#3008\n" +
                        "Hub Room~\n" +
                        "A central hub with multiple exits.~\n" +
                        "0 0\n" +
                        "D0\n" +
                        "~\n" +
                        "~\n" +
                        "0 0 3009\n" +
                        "D1\n" +
                        "~\n" +
                        "~\n" +
                        "1 0 3010\n" +
                        "D2\n" +
                        "An ornate door blocks the way east.~\n" +
                        "door ornate~\n" +
                        "2 100 3011\n" +
                        "S\n";

            // Act
            var room = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(3, room.Exits.Count);
            
            // Check first exit (North)
            Assert.Equal(0, room.Exits[0].Direction);
            Assert.Equal(3009, room.Exits[0].DestinationVnum);
            
            // Check second exit (South)
            Assert.Equal(1, room.Exits[1].Direction);
            Assert.Equal(3010, room.Exits[1].DestinationVnum);
            
            // Check third exit (East) with description and keywords
            Assert.Equal(2, room.Exits[2].Direction);
            Assert.Equal(3011, room.Exits[2].DestinationVnum);
            Assert.Contains("ornate door", room.Exits[2].Description);
            Assert.Contains("door", room.Exits[2].Keywords);
        }

        [Fact]
        public void ParseSingle_WithExitDescription_ParsesCorrectly()
        {
            // Arrange
            var input = "#3012\n" +
                        "Starting Room~\n" +
                        "You are in a starting room.~\n" +
                        "0 0\n" +
                        "D0\n" +
                        "You see a narrow passage leading north.~\n" +
                        "passage narrow~\n" +
                        "0 0 3013\n" +
                        "S\n";

            // Act
            var room = _parser.ParseSingle(input);

            // Assert
            var exit = room.Exits.First();
            Assert.Equal("You see a narrow passage leading north.", exit.Description);
            Assert.Equal("passage narrow", exit.Keywords);
        }

        [Fact]
        public void ParseSingle_WithMultilineExitDescription_ParsesCorrectly()
        {
            // Arrange
            var input = "#3014\n" +
                        "Room with Detailed Exit~\n" +
                        "A room with a detailed exit description.~\n" +
                        "0 0\n" +
                        "D3\n" +
                        "A heavy iron door blocks the way west.\n" +
                        "It appears to be locked from the other side.~\n" +
                        "door iron locked~\n" +
                        "3 50 3015\n" +
                        "S\n";

            // Act
            var room = _parser.ParseSingle(input);

            // Assert
            var exit = room.Exits.First();
            Assert.Equal(3, exit.Direction); // West
            Assert.Contains("heavy iron door", exit.Description);
            Assert.Contains("locked from the other side", exit.Description);
        }

        [Theory]
        [InlineData(0)] // North
        [InlineData(1)] // South
        [InlineData(2)] // East
        [InlineData(3)] // West
        [InlineData(4)] // Up
        [InlineData(5)] // Down
        public void ParseSingle_WithVariousDirections_ParsesCorrectly(int direction)
        {
            // Arrange
            var input = $"#3020\n" +
                        "Test Room~\n" +
                        "Test description.~\n" +
                        "0 0\n" +
                        $"D{direction}\n" +
                        "~\n" +
                        "~\n" +
                        $"{direction} 0 3021\n" +
                        "S\n";

            // Act
            var room = _parser.ParseSingle(input);

            // Assert
            Assert.Single(room.Exits);
            Assert.Equal(direction, room.Exits[0].Direction);
        }

        #endregion

        #region Multiple Rooms Parsing Tests

        [Fact]
        public void Parse_WithMultipleRooms_ReturnsAllRooms()
        {
            // Arrange
            var input = "#3030\n" +
                        "Room One~\n" +
                        "First room.~\n" +
                        "0 0\n" +
                        "S\n" +
                        "#3031\n" +
                        "Room Two~\n" +
                        "Second room.~\n" +
                        "0 1\n" +
                        "S\n" +
                        "#3032\n" +
                        "Room Three~\n" +
                        "Third room.~\n" +
                        "1 2\n" +
                        "S\n";

            // Act
            var rooms = _parser.Parse(input).ToList();

            // Assert
            Assert.Equal(3, rooms.Count);
            Assert.Equal(3030, rooms[0].Vnumber);
            Assert.Equal(3031, rooms[1].Vnumber);
            Assert.Equal(3032, rooms[2].Vnumber);
        }

        [Fact]
        public void Parse_WithEmptyInput_ReturnsEmptyList()
        {
            // Act
            var rooms = _parser.Parse(string.Empty);

            // Assert
            Assert.Empty(rooms);
        }

        [Fact]
        public void Parse_WithNullInput_ReturnsEmptyList()
        {
            // Act
#pragma warning disable CS8625
            var rooms = _parser.Parse(null);
#pragma warning restore CS8625

            // Assert
            Assert.Empty(rooms);
        }

        [Fact]
        public void Parse_WithMultilineDescriptionsAcrossRooms_ParsesCorrectly()
        {
            // Arrange
            var input = "#3040\n" +
                        "First Room~\n" +
                        "This is the first room\n" +
                        "with a multiline description.~\n" +
                        "0 0\n" +
                        "S\n" +
                        "#3041\n" +
                        "Second Room~\n" +
                        "This is the second room\n" +
                        "also with a multiline description\n" +
                        "spanning three lines.~\n" +
                        "0 1\n" +
                        "S\n";

            // Act
            var rooms = _parser.Parse(input).ToList();

            // Assert
            Assert.Equal(2, rooms.Count);
            Assert.Contains("multiline description", rooms[0].Description);
            Assert.Contains("spanning three lines", rooms[1].Description);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void ParseSingle_WithExtraDescription_IgnoresExtraDescription()
        {
            // Arrange - Extra descriptions are marked with 'E' and should be handled
            var input = "#3050\n" +
                        "Room with Extra Desc~\n" +
                        "A room description.~\n" +
                        "0 0\n" +
                        "E\n" +
                        "fireplace~\n" +
                        "There is a roaring fireplace.~\n" +
                        "S\n";

            // Act
            var room = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(3050, room.Vnumber);
            Assert.Equal("A room description.", room.Description);
        }

        [Fact]
        public void ParseSingle_WithWhitespaceAndNewlines_ParsesCorrectly()
        {
            // Arrange
            var input = "  #3051  \n" +
                        "  Whitespace Room  ~\n" +
                        "  A room with whitespace handling.  ~\n" +
                        "  0 0  \n" +
                        "  S  \n";

            // Act
            var room = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(3051, room.Vnumber);
            Assert.NotEmpty(room.Name);
        }

        [Fact]
        public void ParseSingle_WithLargeVnumber_ParsesCorrectly()
        {
            // Arrange
            var input = "#99999\n" +
                        "Large Vnum Room~\n" +
                        "A room with large vnum.~\n" +
                        "0 0\n" +
                        "S\n";

            // Act
            var room = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(99999, room.Vnumber);
        }

        [Fact]
        public void ParseSingle_WithZeroVnumber_ParsesCorrectly()
        {
            // Arrange
            var input = "#0\n" +
                        "Zero Vnum Room~\n" +
                        "A room with zero vnum.~\n" +
                        "0 0\n" +
                        "S\n";

            // Act
            var room = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(0, room.Vnumber);
        }

        [Fact]
        public void ParseSingle_WithInvalidVnumber_ThrowsException()
        {
            // Arrange
            var input = "#INVALID\n" +
                        "Bad Vnum~\n" +
                        "Bad vnum.~\n" +
                        "0 0\n" +
                        "S\n";

            // Act & Assert
            Assert.Throws<System.InvalidOperationException>(() => _parser.ParseSingle(input));
        }

        [Fact]
        public void ParseSingle_WithMissingFlagsLine_ThrowsException()
        {
            // Arrange
            var input = "#3060\n" +
                        "Missing Flags~\n" +
                        "Room without flags line.~\n" +
                        "S\n";

            // Act & Assert
            Assert.Throws<System.InvalidOperationException>(() => _parser.ParseSingle(input));
        }

        [Fact]
        public void ParseSingle_WithInvalidFlags_ThrowsException()
        {
            // Arrange
            var input = "#3061\n" +
                        "Bad Flags~\n" +
                        "Room with bad flags.~\n" +
                        "INVALID 0\n" +
                        "S\n";

            // Act & Assert
            Assert.Throws<System.InvalidOperationException>(() => _parser.ParseSingle(input));
        }

        [Fact]
        public void ParseSingle_WithInvalidSectorType_ThrowsException()
        {
            // Arrange
            var input = "#3062\n" +
                        "Bad Sector~\n" +
                        "Room with bad sector.~\n" +
                        "0 INVALID\n" +
                        "S\n";

            // Act & Assert
            Assert.Throws<System.InvalidOperationException>(() => _parser.ParseSingle(input));
        }

        [Fact]
        public void ParseSingle_WithComplexRoom_ParsesSuccessfully()
        {
            // Arrange - A realistic complex room
            var input = "#1200\n" +
                        "The Tavern~\n" +
                        "You stand inside a bustling tavern filled with the aroma of ale and food.\n" +
                        "The air is thick with smoke from the fireplace, and the sound of laughter\n" +
                        "and conversation fills the room. A long bar stretches along the east wall,\n" +
                        "and several tables are scattered throughout. The barkeep stands behind the bar,\n" +
                        "ready to serve those in need of refreshment.~\n" +
                        "8 1\n" +
                        "D0\n" +
                        "A heavy wooden door to the north.~\n" +
                        "door~\n" +
                        "0 0 1201\n" +
                        "D2\n" +
                        "An open archway to the east leads to a storage room.~\n" +
                        "archway storage~\n" +
                        "2 0 1250\n" +
                        "S\n";

            // Act
            var room = _parser.ParseSingle(input);

            // Assert
            Assert.Equal(1200, room.Vnumber);
            Assert.Equal("The Tavern", room.Name);
            Assert.Contains("bustling tavern", room.Description);
            Assert.Contains("long bar stretches", room.Description);
            Assert.Equal(8, room.RoomFlags);
            Assert.Equal(1, room.SectorType);
            Assert.Equal(2, room.Exits.Count);
            Assert.Equal(1201, room.Exits[0].DestinationVnum);
            Assert.Equal(1250, room.Exits[1].DestinationVnum);
        }

        #endregion

        #region Room Name and Description Tests

        [Fact]
        public void ParseSingle_WithSpecialCharactersInName_ParsesCorrectly()
        {
            // Arrange
            var input = "#3070\n" +
                        "The Castle's Hall~\n" +
                        "A magnificent hall.~\n" +
                        "0 0\n" +
                        "S\n";

            // Act
            var room = _parser.ParseSingle(input);

            // Assert
            Assert.Equal("The Castle's Hall", room.Name);
        }

        [Fact]
        public void ParseSingle_WithSpecialCharactersInDescription_ParsesCorrectly()
        {
            // Arrange
            var input = "#3071\n" +
                        "Special Room~\n" +
                        "A room with 'quotes', \"double quotes\", and #symbols.~\n" +
                        "0 0\n" +
                        "S\n";

            // Act
            var room = _parser.ParseSingle(input);

            // Assert
            Assert.Contains("'quotes'", room.Description);
            Assert.Contains("\"double quotes\"", room.Description);
            Assert.Contains("#symbols", room.Description);
        }

        [Fact]
        public void ParseSingle_WithEmptyName_ParsesCorrectly()
        {
            // Arrange
            var input = "#3072\n" +
                        "~\n" +
                        "A room with empty name.~\n" +
                        "0 0\n" +
                        "S\n";

            // Act
            var room = _parser.ParseSingle(input);

            // Assert
            Assert.Empty(room.Name);
            Assert.NotEmpty(room.Description);
        }

        [Fact]
        public void ParseSingle_WithEmptyDescription_ParsesCorrectly()
        {
            // Arrange
            var input = "#3073\n" +
                        "Empty Desc Room~\n" +
                        "~\n" +
                        "0 0\n" +
                        "S\n";

            // Act
            var room = _parser.ParseSingle(input);

            // Assert
            Assert.NotEmpty(room.Name);
            Assert.Empty(room.Description);
        }

        #endregion
    }
}
