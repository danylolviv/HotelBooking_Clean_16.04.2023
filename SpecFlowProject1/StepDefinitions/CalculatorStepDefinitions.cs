using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;

namespace SpecFlowProject1.StepDefinitions

{
    [Binding]
    public sealed class CalculatorStepDefinitions
    {

        public DateTime startDate;
        public DateTime endDate;
        //private IBookingManager bookingManager;
        public bool bookingCreated;


        IRepository<Booking> bookingRepositoryDDT = new FakeBookingRepository(DateTime.Today.AddDays(10), DateTime.Today.AddDays(20));
        IRepository<Room> roomRepositoryDDT = new FakeRoomRepository();
        public BookingManager bookingManager = new BookingManager(new FakeBookingRepository(DateTime.Today.AddDays(10), DateTime.Today.AddDays(20)), new FakeRoomRepository());

        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        [Given("when the first booking is (.*)")]
        public void GivenTheFirstBookingDateIs(int day)
        {
            startDate = DateTime.Today.AddDays(day);
        }

        [Given("the second booking is (.*)")]
        public void GivenTheSecondBookingDateIs(int day)
        {
            endDate = DateTime.Today.AddDays(day);
        }

        [When("the booking is created")]
        public void WhenTheBookingIsCreated()
        {
            // Arrange
            Booking booking = new Booking
            {
                StartDate = startDate,
                EndDate = endDate,
                IsActive = true,
                CustomerId = 1,
                RoomId = 1
            };

            bookingCreated = bookingManager.CreateBooking(booking);
        }

        [Then("the result should be (.*)")]
        public void ThenTheResultShouldBe(bool result)
        {
            Assert.Equal(result, actual: bookingCreated);
        }
    }
}