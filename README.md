# EventHub

EventHub is a console-based **Event Booking System** developed in **C# and .NET**.

The project was designed with a strong focus on **Object-Oriented Programming**, clean code, maintainability, extensibility, and separation of responsibilities. Rather than maximizing the number of features, the solution focuses on modeling the event booking domain through clear abstractions, business rules, SOLID principles, and appropriate design patterns.

## Features

EventHub supports the core workflow of an event booking system, including:

- Creating and managing events
- Publishing and cancelling events
- Multiple event types:
  - Concert
  - Conference
  - Workshop
- Multiple ticket types:
  - Standard
  - VIP
  - Student
- Event capacity and available-seat management
- Creating and cancelling bookings
- Organizer and Customer roles
- Event search and filtering
- Booking and event reports
- Validation of business rules
- Domain-specific exception handling
- Different pricing strategies depending on ticket type

## Architecture

The solution is divided into several projects with clearly separated responsibilities:

~~~text
EventHub
│
├── src
│   ├── EventHub.Domain
│   ├── EventHub.Application
│   ├── EventHub.Infrastructure
│   └── EventHub.Presentation
│
└── tests
    └── EventHub.Tests
~~~

### EventHub.Domain

Contains the core business model and business rules.

Responsibilities include:

- Domain entities
- Event hierarchy
- Booking and ticket models
- User roles
- Enumerations
- Domain validation
- Custom exceptions

The Domain layer does not depend on infrastructure or presentation concerns.

### EventHub.Application

Contains application-level use cases and coordinates interactions between domain objects.

Responsibilities include:

- Event management
- Booking management
- Search and reporting
- Application service interfaces
- Repository abstractions
- Factory abstractions
- Pricing abstractions

### EventHub.Infrastructure

Provides concrete implementations required by the application layer.

Responsibilities include:

- In-memory repositories
- Event factory implementation
- Ticket pricing strategies
- Pricing strategy resolution

Because the application depends on abstractions rather than concrete implementations, the current in-memory persistence could later be replaced by a database without changing the domain model.

### EventHub.Presentation

Contains the console-based user interface.

It is responsible for:

- User interaction
- Menu navigation
- Reading input
- Displaying results
- Calling application services

Business rules are intentionally kept outside the presentation layer.

### EventHub.Tests

Contains automated tests for the main parts of the system, including:

- Domain behavior
- Application services
- Repositories
- Event factory
- Pricing strategies

## Object-Oriented Design

The project demonstrates the main OOP principles throughout the implementation.

### Encapsulation

Domain objects are responsible for protecting their own state and enforcing relevant business rules.

For example, an event manages its capacity, available seats, lifecycle status, publishing, cancellation, reservation, and seat release behavior instead of allowing these values to be modified arbitrarily from outside the class.

### Abstraction

Common event behavior is represented through the abstract `Event` class, while infrastructure dependencies are represented through interfaces.

Examples include repository, factory, and pricing abstractions.

### Inheritance

Different event types inherit shared behavior from the base `Event` class while implementing event-specific behavior where necessary.

Examples:

- `Concert`
- `Conference`
- `Workshop`

### Polymorphism

Different event types can be handled through the common `Event` abstraction.

Pricing is also polymorphic: different ticket pricing strategies implement the same pricing contract and can be selected without changing the booking workflow.

## SOLID Principles

The design applies SOLID principles where they provide practical value.

**Single Responsibility Principle**  
Domain entities, application services, repositories, factories, pricing strategies, and presentation logic have separate responsibilities.

**Open/Closed Principle**  
The system can be extended with additional event types or pricing strategies without requiring major changes to existing booking logic.

**Liskov Substitution Principle**  
Concrete event types can be used wherever the base `Event` abstraction is expected.

**Interface Segregation Principle**  
Small, focused interfaces are used for repositories, factories, and pricing behavior.

**Dependency Inversion Principle**  
Application services depend on abstractions rather than concrete repository or infrastructure implementations.

## Design Patterns

Several design patterns are used where they simplify responsibilities and improve extensibility.

### Repository Pattern

Repository interfaces separate application logic from persistence concerns.

The current implementation uses in-memory repositories, but another persistence mechanism could be introduced behind the same abstractions.

### Factory Pattern

Event creation is delegated to an event factory instead of spreading concrete event construction throughout the application.

This centralizes creation logic and makes adding new event types easier.

### Strategy Pattern

Ticket pricing is implemented using separate pricing strategies.

Each pricing policy implements a common contract, allowing pricing rules to vary independently from the booking process.

## Business Rules and Validation

The system protects domain consistency through validation and controlled operations.

Examples include:

- Events cannot be created with invalid data
- Event capacity and prices must contain valid values
- Events follow a defined lifecycle
- Seats cannot be reserved beyond available capacity
- Only valid ticket types can be booked for an event
- Users must have the appropriate role for an operation
- Duplicate bookings are prevented
- Cancelling a booking releases reserved seats
- Invalid operations result in meaningful exceptions

## Extensibility

The application was intentionally designed so that future functionality can be introduced without major restructuring.

Possible extensions include:

- Database persistence
- Additional event types
- Additional ticket types
- New pricing policies
- Authentication
- REST API or graphical user interface
- Payment processing
- Notifications

These features are intentionally outside the current scope because the primary goal of the project is to demonstrate a clean and extensible OOP design.

## Technologies

- C#
- .NET 10
- xUnit
- Git
- GitHub

## Running the Application

From the repository root, restore and build the solution:

~~~bash
dotnet restore
dotnet build
~~~

Run the console application:

~~~bash
dotnet run --project src/EventHub.Presentation
~~~

## Running the Tests

From the repository root:

~~~bash
dotnet test
~~~

## Design Approach

The project was developed incrementally with an emphasis on **quality over quantity**.

The main design goals were:

1. Model the event booking domain clearly
2. Keep business rules inside the appropriate domain and application components
3. Use OOP principles as part of the implementation rather than only as structural concepts
4. Keep components loosely coupled and testable
5. Allow future functionality to be added without redesigning the entire application
6. Keep the codebase readable and easy to navigate

This approach reflects the idea that a maintainable and extensible solution is more valuable than adding a large number of loosely structured features.