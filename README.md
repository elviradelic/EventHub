EventHub

EventHub is a console-based event booking application built with C# and .NET.

I built this project to practice object-oriented design and work with a more structured C# application. It covers event management, ticket bookings, seat availability, and different pricing rules.

The application currently runs in the console and uses in-memory storage.

Features

Create and manage concerts, conferences, and workshops

Publish or cancel events

Book Standard, VIP, and Student tickets

Track available seats and event capacity

Cancel bookings and release reserved seats

Search and filter events

Generate basic event and booking reports

Validate bookings and handle invalid operations

Project Structure

The solution is split into four main projects:

EventHub/
├── src/
│   ├── EventHub.Domain/
│   ├── EventHub.Application/
│   ├── EventHub.Infrastructure/
│   └── EventHub.Presentation/
├── tests/
│   └── EventHub.Tests/
└── EventHub.slnx

Domain - Event models, bookings, ticket types, and business rules.

Application - Services that handle event and booking operations.

Infrastructure - In-memory repositories, event creation, and ticket pricing.

Presentation - Console menus and user interaction.

Tests - Automated tests for the main application components.

Implementation Details

A few parts of the implementation I focused on:

Using an abstract Event class for shared behavior across different event types.

Keeping booking and capacity rules inside the domain model.

Using repository interfaces to separate data access from application logic.

Handling event creation through a factory.

Implementing ticket pricing with separate strategies.

Writing automated tests for business rules and application services.

The project uses OOP and SOLID principles, along with Repository, Factory, and Strategy patterns where they fit the implementation.

Tech Stack

C#

.NET 10

xUnit

Git

Getting Started

You'll need the .NET 10 SDK installed.

Clone the repository:

git clone https://github.com/elviradelic/EventHub.git
cd EventHub

Restore dependencies and build:

dotnet restore
dotnet build

Run the application:

dotnet run --project src/EventHub.Presentation

Run the tests:

dotnet test

Notes

EventHub currently uses in-memory repositories, so data is not persisted between application runs.

Database storage and a REST API are possible next steps, but for now I've kept the project focused on the booking logic and application structure.