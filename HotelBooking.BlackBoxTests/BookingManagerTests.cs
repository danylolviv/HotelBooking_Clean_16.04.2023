using HotelBooking.Core;
using HotelBooking.Infrastructure.Repositories;
using HotelBooking.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace HotelBooking.BlackBoxTests;

public class BookingManagerTests
{
    
    private IBookingManager bookingManager;
    private Mock<IRepository<Booking>> bookingRepository;
    private Mock<IRepository<Room>> roomRepository;
    private Mock<IRepository<Customer>> customerRepository;
    
    private IBookingManager _manager;
    private BookingsController _controller;
    private Mock<IBookingManager> _bookingManager;

    public BookingManagerTests()
    {
        // Initialize the mock for the BookingManager dependency
        _bookingManager= new Mock<IBookingManager>();
        bookingRepository = new Mock<IRepository<Booking>>();
        roomRepository = new Mock<IRepository<Room>>();
        customerRepository = new Mock<IRepository<Customer>>();
        
        // Initialize your controller with the mock dependency
        _controller = new BookingsController(bookingRepository.Object, roomRepository.Object, customerRepository.Object, _bookingManager.Object);
        
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

        _manager = new BookingManager(bookingRepository.Object, roomRepository.Object);
    }
    
    /*
    Specification-based black-box testing techniques focus on testing the functionality of the software without considering its internal structure. 
    The tester derives test cases from the software's requirements or specifications, ensuring that the software behaves as expected in various scenarios.

    In this case, we have derived 10 test cases for the "Create booking" feature, which represents a range of possible scenarios and input conditions:

    1. Post_ValidBookingRequest_ReturnsCreated: Test if a valid booking request returns a Created result.
    2. Post_InvalidBookingRequest_ReturnsBadRequest: Test if an invalid booking request (null booking) returns a BadRequest result.
    3. Post_BookingConflict_ReturnsConflict: Test if a booking conflict returns a Conflict result.
    4. Post_BookingWithInvalidRoomId_ReturnsBadRequest: Test if a booking with an invalid room ID returns a BadRequest result.
    5. Post_BookingWithInvalidEndDate_ReturnsBadRequest: Test if a booking with an invalid end date returns a BadRequest result.
    6. Post_BookingWithInvalidStartDate_ReturnsBadRequest: Test if a booking with an invalid start date returns a BadRequest result.
    7. Post_BookingWithSameStartDateAndEndDate_ReturnsBadRequest: Test if a booking with the same start and end date returns a BadRequest result.
    8. Post_BookingWithOverlappingDates_ReturnsConflict: Test if a booking with overlapping dates returns a Conflict result.
    9. Post_BookingWithEndDateInPast_ReturnsBadRequest: Test if a booking with an end date in the past returns a BadRequest result.
    10. Post_BookingWithStartDateInPast_ReturnsBadRequest: Test if a booking with a start date in the past returns a BadRequest result.

    These test cases cover various input conditions and expected outcomes, validating the "Create booking" feature's behavior according to its specifications. 
    By using black-box testing techniques, we can ensure the software meets the requirements and behaves as expected for the end-users.
    */
    
    /*
    In addition to the original requirements, we have added extra validation conditions to the "Create booking" feature to handle more 
    edge cases and improve the robustness of the application. These additional conditions include:

    1. Checking if the room ID is greater than 0, ensuring the booking is associated with a valid room.
    2. Validating that the end date is after the start date, ensuring the booking duration is positive.
    3. Verifying that the start and end dates are not set to a default minimum value (e.g., DateTime.MinValue), which might indicate invalid input.
    4. Ensuring that the start and end dates are not in the past, preventing bookings with dates that have already passed.

    These extra conditions enhance the overall quality and reliability of the "Create booking" feature by addressing potential issues
    */

[Fact]
public void Post_ValidBookingRequest_ReturnsCreated()
{
    // Arrange
    Booking booking = new()
    {
        StartDate = DateTime.Today.AddDays(1),
        EndDate = DateTime.Today.AddDays(2)
    };

    // Act
    bool isCreated = _manager.CreateBooking(booking);

    // Assert
    Assert.True(isCreated);
}

[Fact]
public void Post_InvalidBookingRequest_ReturnsBadRequest()
{
    // Arrange
    Booking booking = null;

    // Act
    IActionResult result = _controller.Post(booking);

    // Assert
    Assert.IsType<BadRequestResult>(result);
}

[Fact]
public void Post_BookingConflict_ReturnsConflict()
{
    // Arrange
    Booking booking = new()
    {
        RoomId = 2,
        StartDate = DateTime.Today.AddDays(15),
        EndDate = DateTime.Today.AddDays(17)
    };
    _bookingManager.Setup(m => m.CreateBooking(booking)).Returns(false);

    // Act
    IActionResult result = _controller.Post(booking);

    // Assert
    Assert.IsType<ConflictObjectResult>(result);
    Assert.Equal("The booking could not be created. All rooms are occupied. Please try another period.", ((ConflictObjectResult)result).Value);
}

[Fact]
public void Post_BookingWithInvalidRoomId_ReturnsBadRequest()
{
    // Arrange
    Booking booking = new Booking
    {
        RoomId = -1, 
        StartDate = DateTime.Today.AddDays(1),
        EndDate = DateTime.Today.AddDays(2)
    };

    // Act
    IActionResult result = _controller.Post(booking);

    // Assert
    Assert.IsType<BadRequestResult>(result);
}

[Fact]
public void Post_BookingWithInvalidEndDate_ReturnsBadRequest()
{
    // Arrange
    Booking booking = new Booking
    {
        Id = 5, 
        RoomId = 101, 
        StartDate = DateTime.Parse("2023-05-10"), 
        EndDate = DateTime.MinValue
    };

    // Act
    IActionResult result = _controller.Post(booking);

    // Assert
    Assert.IsType<BadRequestResult>(result);
}

[Fact]
public void Post_BookingWithInvalidStartDate_ReturnsBadRequest()
{
    // Arrange
    Booking booking = new Booking
    {
        RoomId = 4, 
        StartDate = DateTime.MinValue, 
        EndDate = DateTime.Parse("2023-05-12")
    };

    // Act
    IActionResult result = _controller.Post(booking);

    // Assert
    Assert.IsType<BadRequestResult>(result);
}

[Fact]
public void Post_BookingWithSameStartDateAndEndDate_ReturnsBadRequest()
{
    // Arrange
    Booking booking = new Booking
    {
        Id = 7, 
        RoomId = 101, 
        StartDate = DateTime.Parse("2023-05-10"), 
        EndDate = DateTime.Parse("2023-05-10")
    };

    // Act
    IActionResult result = _controller.Post(booking);

    // Assert
    Assert.IsType<BadRequestResult>(result);
}

[Fact]
public void Post_BookingWithOverlappingDates_ReturnsConflict()
{
    // Arrange
    Booking booking1 = new Booking { Id = 8, RoomId = 101, StartDate = DateTime.Parse("2023-05-10"), EndDate = DateTime.Parse("2023-05-15") };
    Booking booking2 = new Booking { Id = 9, RoomId = 101, StartDate = DateTime.Parse("2023-05-12"), EndDate = DateTime.Parse("2023-05-17") };
    _bookingManager.Setup(m => m.CreateBooking(booking1)).Returns(true);
    _bookingManager.Setup(m => m.CreateBooking(booking2)).Returns(false);

    _controller.Post(booking1); // Create the first booking

    // Act
    IActionResult result = _controller.Post(booking2); // Try to create the second booking with overlapping dates

    // Assert
    Assert.IsType<ConflictObjectResult>(result);
}

[Fact]
public void Post_BookingWithEndDateInPast_ReturnsBadRequest()
{
    // Arrange
    Booking booking = new Booking
    {
        RoomId = 3, 
        StartDate = DateTime.Today.AddDays(-3),
        EndDate = DateTime.Today.AddDays(-1)
    };

    // Act
    IActionResult result = _controller.Post(booking);

    // Assert
    Assert.IsType<BadRequestResult>(result);
}

[Fact]
public void Post_BookingWithStartDateInPast_ReturnsBadRequest()
{
    // Arrange
    Booking booking = new Booking
    {
        RoomId = 3, 
        StartDate = DateTime.Today.AddDays(-3),
        EndDate = DateTime.Today.AddDays(1)
    };

    // Act
    IActionResult result = _controller.Post(booking);

    // Assert
    Assert.IsType<BadRequestResult>(result);
}





}