
### .NET Developer Test Task

####  Fix existing unit tests:

- Please fix the issue in the `TestTask.API.Tests.PurchaseTests.cs.BuyAsync_ShouldHandleConcurrentPurchases` test without modifying the test itself.

#### 'Report' Task:
Enhance an existing application by modifying its data models and creating an API endpoint to generate a specific report.

1. **Update the Data Model:**
    - Modify the `UserItem` class to include a `DateTime` property named `PurchaseDate`. This property should capture the date and time when each item is purchased by the user.
   ```csharp
   public DateTime PurchaseDate { get; set; }
   ```

2. **Develop an API Endpoint:**
    - Create an API endpoint that generates a report of the 3 most popular items each year. The popularity of an item is defined by the number of identical items bought in a single day by one user.
    - The report should return statistics for all years present in the data.
    - The endpoint should return a list where each entry contains:
        - Year
        - Item name
        - The number of times it was bought in its most popular single day

3. **Additional Deliverables:**
    - Provide unit tests to cover your API endpoint functionality.
