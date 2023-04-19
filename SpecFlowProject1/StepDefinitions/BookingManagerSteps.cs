using HotelBooking.Core;
using Xunit;

namespace SpecFlowProject1.StepDefinitions;

public class BookingManagerSteps
{
    private CalculatorStepDefinitions _calculatorSteps;
        private int _startDaysOffset;
        private int _endDaysOffset;
        private int _result;
        private bool _bookingResult;
        private Exception _exception;

        public BookingManagerSteps(CalculatorStepDefinitions calculatorSteps)
        {
            _calculatorSteps = calculatorSteps;
        }

        [Given(@"a start date offset of (.*)")]
        public void GivenAStartDateOffsetOf(int startDaysOffset)
        {
            _startDaysOffset = startDaysOffset;
        }

        [Given(@"an end date offset of (.*)")]
        public void GivenAnEndDateOffsetOf(int endDaysOffset)
        {
            _endDaysOffset = endDaysOffset;
        }

        [When(@"FindAvailableRoom is called")]
        public void WhenFindAvailableRoomIsCalled()
        {
            try
            {
                var startDate = DateTime.Today.AddDays(_startDaysOffset);
                var endDate = DateTime.Today.AddDays(_endDaysOffset);
                _result = _calculatorSteps.bookingManager.FindAvailableRoom(startDate, endDate);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [Then(@"the result should be (.*)")]
        public void ThenTheResultShouldBe(int expectedRoomId)
        {
            Assert.Equal(expectedRoomId, _result);
        }

        [When(@"CreateBooking is called")]
        public void WhenCreateBookingIsCalled()
        {
            var startDate = DateTime.Today.AddDays(_startDaysOffset);
            var endDate = DateTime.Today.AddDays(_endDaysOffset);
            Booking booking = new()
            {
                StartDate = startDate,
                EndDate = endDate
            };
            _bookingResult = _calculatorSteps.bookingManager.CreateBooking(booking);
        }

        [Then(@"the result should be (.*)")]
        public void ThenTheResultShouldBe(bool expectedResult)
        {
            Assert.Equal(expectedResult, _bookingResult);
        }

        [When(@"GetFullyOccupiedDates is called")]
        public void WhenGetFullyOccupiedDatesIsCalled()
        {
            try
            {
                var startDate = DateTime.Today.AddDays(_startDaysOffset);
                var endDate = DateTime.Today.AddDays(_endDaysOffset);
                _result = _calculatorSteps.bookingManager.GetFullyOccupiedDates(startDate, endDate).Count;
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [Then(@"the expected result should be (.*)")]
        public void ThenTheExpectedResultShouldBe(string expectedResult)
        {
            if (expectedResult == "ArgumentException")
            {
                Assert.IsType<ArgumentException>(_exception);
            }
            else
            {
                int expectedCount = int.Parse(expectedResult);
                Assert.Equal(expectedCount, _result);
            }
        }
}