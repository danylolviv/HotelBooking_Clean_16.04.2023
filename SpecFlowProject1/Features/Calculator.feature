Feature: Calculator
![Calculator](https://specflow.org/wp-content/uploads/2020/09/calculator.png)
Simple calculator for adding **two** numbers
<11>
Link to a feature: [Calculator](SpecFlowProject1/Features/Calculator.feature)
***Further read***: **[Learn more about how to generate Living Documentation](https://docs.specflow.org/projects/specflow-livingdoc/en/latest/LivingDocGenerator/Generating-Documentation.html)**
	
 @mytag
  Scenario: Create a booking
    Given when the first booking is 5
    And the second booking is 11
    When the booking is created
    Then the result should be false

  @mytag
  Scenario: Create a booking that overlaps existing booking
    Given when the first booking is 12
    And the second booking is 15
    When the booking is created
    Then the result should be false

  @mytag
  Scenario: Create a booking that doesn't overlap existing booking
    Given when the first booking is 25
    And the second booking is 30
    When the booking is created
    Then the result should be true