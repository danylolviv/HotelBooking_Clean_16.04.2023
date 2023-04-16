using Moq;

namespace HotelBooking.Core.Test;

public class BookingManager_Test
{
    private IBookingManager bookingManager;
    private Mock<IRepository<Booking>> bookingRepository;
    private Mock<IRepository<Room>> roomRepository;

    public BookingManager_Test() {
        bookingRepository = new Mock<IRepository<Booking>>();
        roomRepository = new Mock<IRepository<Room>>();

        var rooms = new List<Room>
        {
            new Room { Id=1, Description="A" },
            new Room { Id=2, Description="B" },
        };

        DateTime fullyOccupiedStartDate = DateTime.Today.AddDays(10);
        DateTime fullyOccupiedEndDate = DateTime.Today.AddDays(20);

        List<Booking> bookings = new List<Booking>
        {
            new Booking { Id=1, StartDate=fullyOccupiedStartDate, EndDate=fullyOccupiedEndDate, IsActive=true, CustomerId=1, RoomId=1 },
            new Booking { Id=2, StartDate=fullyOccupiedStartDate, EndDate=fullyOccupiedEndDate, IsActive=true, CustomerId=2, RoomId=2 },
        };

        roomRepository.Setup(x => x.GetAll()).Returns(rooms);
        bookingRepository.Setup(x => x.GetAll()).Returns(bookings);

        bookingManager = new BookingManager(bookingRepository.Object, roomRepository.Object);
    }
    
    [Fact]
    public void IBookingManager_IsAvailable()
    {
        var _bookingManager = new Mock<IBookingManager>();
        Assert.NotNull(_bookingManager);
    }

    [Fact]
    public void FindAvailableRoom_StartDateNotToday_ThrowsArgumentException()
    {
        // Arrange
        DateTime dateToday = DateTime.Today;
        DateTime dateTomorrow = DateTime.Today.AddDays(1);

        // Act
        Action act = () => bookingManager.FindAvailableRoom(dateToday, dateTomorrow);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void FindAvailableRoom_StartDateBeforeToday_ThrowsArgumentException()
    {
        // Arrange
        DateTime dateYesterday = DateTime.Today.AddDays(-1);
        DateTime dateToday = DateTime.Today;

        // Act
        Action act = () => bookingManager.FindAvailableRoom(dateYesterday, dateToday);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void FindAvailableRoom_EndDateNotOlderThanStartDate_ThrowsArgumentException()
    {
        // Arrange
        DateTime dateStart = DateTime.Today.AddDays(1);
        DateTime dateEnd = DateTime.Today;

        // Act
        Action act = () => bookingManager.FindAvailableRoom(dateStart, dateEnd);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne()
    {
        // Arrange
        DateTime dateStart = DateTime.Today.AddDays(1);
        DateTime dateEnd = DateTime.Today.AddDays(2);

        // Act
        int roomId = bookingManager.FindAvailableRoom(dateStart, dateEnd);

        // Assert
        Assert.NotEqual(-1, roomId);
    }

    [Fact]
    public void CreateBooking_IncorrectStartDate_ReturnsFalse()
    {
        // Arrange
        Booking booking = new()
        {
            StartDate = DateTime.Today.AddDays(11),
            EndDate = DateTime.Today.AddDays(12)
        };

        // Act
        bool isCreated = bookingManager.CreateBooking(booking);

        // Assert
        Assert.False(isCreated);
    }

    [Fact]
    public void CreateBooking_CorrectStartDate_ReturnsTrue()
    {
        // Arrange
        Booking booking = new()
        {
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(2)
        };

        // Act
        bool isCreated = bookingManager.CreateBooking(booking);

        // Assert
        Assert.True(isCreated);
    }

    [Fact]
    public void GetFullyOccupiedDates_StartDateIsMoreThanEndDate_ThrowArgumentException()
    {
        //Arrange
        DateTime startDate = DateTime.Today.AddDays(5);
        DateTime endDate = DateTime.Today.AddDays(2);
        Action act = () => bookingManager.GetFullyOccupiedDates(startDate, endDate);
        //Act
        var rec = Record.Exception(act);
        //Assert
        Assert.IsType<ArgumentException>(rec);
    }

    [Fact]
    public void GetFullyOccupiedDates_StartDayAdd21EndDateAdd25_ReturnsEmptyList()
    {
        //Arrange
        DateTime startDate = DateTime.Today.AddDays(21);
        DateTime endDate = DateTime.Today.AddDays(25);
        //Act
        List<DateTime> fullyOccupiedDates = bookingManager.GetFullyOccupiedDates(startDate, endDate);
        //Assert
        Assert.Empty(fullyOccupiedDates);
    }

    [Fact]
    public void GetFullyOccupiedDates_StartDayAdd10EndDateAdd20_ReturnsListCount10()
    {
        //Arrange
        DateTime startDate = DateTime.Today.AddDays(10);
        DateTime endDate = DateTime.Today.AddDays(20);
        //Act
        List<DateTime> fullyOccupiedDates = bookingManager.GetFullyOccupiedDates(startDate, endDate);
        //Assert
        Assert.Equal(11, fullyOccupiedDates.Count);
    }

    [Fact]
    public void FindAvailableRoom_RoomNOTAvailable_MinusOne()
    {
        // Arrange
        DateTime dateStart = DateTime.Today.AddDays(10);
        DateTime dateEnd = DateTime.Today.AddDays(20);

        // Act
        int roomId = bookingManager.FindAvailableRoom(dateStart, dateEnd);

        // Assert
        Assert.Equal(-1, roomId);
    }
    
    [Theory]
    [InlineData(11, 12, false)] // Incorrect start date
    [InlineData(1, 2, true)] // Correct start date
    public void CreateBooking_VariousDates_SuccessStatus(int startDaysOffset, int endDaysOffset, bool expectedResult)
    {
        // Arrange
        Booking booking = new()
        {
            StartDate = DateTime.Today.AddDays(startDaysOffset),
            EndDate = DateTime.Today.AddDays(endDaysOffset)
        };

        // Act
        bool isCreated = bookingManager.CreateBooking(booking);

        // Assert
        Assert.Equal(expectedResult, isCreated);
    }
    
    [Theory]
    [InlineData(1, 2, -1)] // Room not available
    [InlineData(21, 22, -1)] // Room available (Room 1)
    [InlineData(30, 40, 1)] // Room available (Room 2)
    public void FindAvailableRoom_VariousDates_RoomAvailability(int startDaysOffset, int endDaysOffset, int expectedRoomId)
    {
        // Arrange
        DateTime dateStart = DateTime.Today.AddDays(startDaysOffset);
        DateTime dateEnd = DateTime.Today.AddDays(endDaysOffset);

        // Act
        int roomId = bookingManager.FindAvailableRoom(dateStart, dateEnd);

        // Assert
        Assert.Equal(expectedRoomId, roomId);
    }
    
    [Theory]
    [InlineData(5, 2, typeof(ArgumentException))] // StartDate is more than EndDate, throws ArgumentException
    [InlineData(21, 25, 0)] // No fully occupied dates, returns empty list
    [InlineData(10, 20, 11)] // Fully occupied dates, returns list with 11 items
    public void GetFullyOccupiedDates_VariousDateRanges_Result(int startDaysOffset, int endDaysOffset, object expectedResult)
    {
        // Arrange
        DateTime startDate = DateTime.Today.AddDays(startDaysOffset);
        DateTime endDate = DateTime.Today.AddDays(endDaysOffset);

        if (expectedResult is Type exceptionType)
        {
            // Act
            Action act = () => bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            Assert.Throws(exceptionType, act);
        }
        else
        {
            // Act
            List<DateTime> fullyOccupiedDates = bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            Assert.Equal((int)expectedResult, fullyOccupiedDates.Count);
        }
    }
}