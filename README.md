# GildedRose

## Getting Started

1. Install .NET 7
2. Clone the repo.
3. Run the project GildedRoseTests
4. Run the tests (GildedRoseTest.cs contains list of Unit tests to validate)

## Problem
Method UpdateQuality_Old() contains functionality to calculate Item quality.
Though it addresses the functionality, the code can be optimized.

## Solution
Optimized the code using method mentioned approaches
* Code Metrics - Restructred the class with maximum "Cyclomatic Complexity" score of 4 (Old is 19), highest maintainability index of 73 (Old is 45)
* Strategy pattern - Implemented strategy pattern to calculate Quality. Used delegates to apply strategy.
* foreach -  Iterate over the collection and process item by item
* Hard-Coded - Removed the hard-coded values and moved those to a class called Constants
* Enum - Item categories are moved to Enum for better comparision and arrest possible errors
* Logging - Created a class to handle logging
* ValidateItem - To arrest the incoming data
* Prerequisite - Collect the data to be process; code reusability
* MaxQuality - Dynamic maximum quality value
* Unit Tests - Possible test cases to cover all the scenarios
