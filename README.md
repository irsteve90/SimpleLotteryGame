# Simple Lottery Game

## Description

**LotteryGame** is a .NET 8-based C# application that simulates a lottery system. It provides functionality for managing players, purchasing tickets, and drawing prizes, 
while adhering to **SOLID principles** to ensure a clean, scalable, and easily maintainable codebase. The application leverages **Entity Framework** for database operations, **Dependency Injection** for loose coupling, and includes **unit tests** to guarantee reliable performance.

## Features

- **Player Management**: Supports 10–15 players with individual balances and the ability to buy lottery tickets.
- **Ticket Purchasing**: Players can purchase between 1 and 10 tickets, each priced at **$1**.
- **Prize Allocation**:
  - **Grand Prize**: 50% of total ticket sales.
  - **Second Tier**: 30% of sales, allocated to 10% of ticket holders.
  - **Third Tier**: 10% of sales, shared among 20% of ticket holders.

## Technologies Used

- **.NET 8** for application development
- **Entity Framework** for data access
- **Dependency Injection** for clean, testable architecture
- **Unit Testing** to ensure functionality and stability

## Getting Started

1. Clone this repository to your local machine.
2. Open the project in **Visual Studio**.
3. Build and run the application using **`dotnet run`**.
4. Follow the prompts to play the lottery, purchase tickets, and view the prize distribution.

## Contributing

If you'd like to contribute:
1. Fork the repository and create a new branch for your changes.
2. Implement your feature or fix, ensuring code follows the project's structure and guidelines.
3. Submit a pull request for review.
