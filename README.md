# EventHub

EventHub is a console-based event booking application developed in C# and .NET.

I built this project to practice object-oriented programming and organize a C# application into separate layers. It handles event management, ticket reservations, seat availability, and pricing rules.

The application currently runs in the console and uses in-memory storage.

## Features

- Create and manage concerts, conferences, and workshops
- Publish and cancel events
- Book Standard, VIP, and Student tickets
- Track event capacity and available seats
- Cancel bookings and release reserved seats
- Search and filter events
- Generate basic event and booking reports
- Validate user input and booking operations

## Tech Stack

- **Language:** C#
- **Framework:** .NET 10
- **Testing:** xUnit
- **Version Control:** Git

## Project Structure

The solution is organized into separate projects for domain logic, application services, infrastructure, and the console interface.

```text
EventHub/
├── src/
│   ├── EventHub.Domain/
│   ├── EventHub.Application/
│   ├── EventHub.Infrastructure/
│   └── EventHub.Presentation/
├── tests/
│   └── EventHub.Tests/
└── EventHub.slnx
```

### Domain

Contains the core business models and rules, including events, bookings, customers, ticket types, and event capacity management.

### Application

Handles application operations such as event creation, reservations, cancellations, and event searches.

### Infrastructure

Contains repository implementations, in-memory data storage, and supporting components for event creation and ticket pricing.

### Presentation

Provides the console interface for interacting with the application.

### Tests

Contains automated tests covering business rules and application behavior.

## Design and Implementation

I used object-oriented programming throughout the project, with a focus on keeping business logic separate from the console interface and data storage.

Some of the main implementation decisions include:

- **Inheritance and polymorphism:** Different event types share common behavior through an abstract `Event` class.
- **Encapsulation:** Event capacity, reservations, and status changes are managed through domain methods.
- **Repository Pattern:** Repository interfaces separate data access from application logic.
- **Factory Pattern:** Event creation is handled through a dedicated factory.
- **Strategy Pattern:** Ticket pricing rules are implemented separately for different ticket types.
- **Dependency Injection:** Components depend on abstractions rather than concrete implementations.

The project also applies SOLID principles where they help keep the code organized and easier to maintain.

## Getting Started

### Prerequisites

- .NET 10 SDK
- Git

### 1. Clone the repository

```bash
git clone https://github.com/elviradelic/EventHub.git
cd EventHub
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Build the solution

```bash
dotnet build
```

### 4. Run the application

```bash
dotnet run --project src/EventHub.Presentation
```

### 5. Run the tests

```bash
dotnet test
```

## Current Limitations

EventHub currently uses in-memory repositories, meaning that application data is not saved between runs.

The project focuses on implementing the booking logic and organizing the application using a layered architecture.

## Possible Improvements

- Add persistent database storage
- Expose application functionality through a REST API
- Add authentication and user accounts
- Expand automated test coverage

