🚀 Elevator Simulator

A production-style, console-based elevator simulation built with .NET, demonstrating architecture, SOLID principles, and configurable dispatch strategies such as ETA-based selection. 
This project focuses on design correctness, extensibility, and testability, rather than real-world hardware concerns.


📌 Features

- Multi-elevator simulation (configurable floors & elevators)
- Real-time elevator movement with configurable timing
- Direction-aware elevator behavior (Up / Down / Idle)
- Accurate ETA-based elevator dispatch
- Strategy selection via Factory Pattern
- Configuration-driven behavior (appsettings.json)
- Clean separation of concerns (UI, Engine, Domain, Strategy)
- Unit tests using xUnit and Moq


🧠 Architecture Overview
<img width="828" height="273" alt="image" src="https://github.com/user-attachments/assets/ef87e90c-d132-49c5-a837-171f717713ea" />


Design Principles Used

- SOLID
- Strategy Pattern
- Factory Pattern
- Dependency Injection
- Separation of Concerns
- Testable Business Logic


⚙ Configuration (appsettings.json)
{
  "ElevatorConfig": {
    "DispatchStrategy": "ETA",
    "Building": {
      "Floors": 10,
      "Elevators": 4
    },
    "SimulationTiming": {
      "MoveDurationInSeconds": 10,
      "StopDurationInSeconds": 10,
      "RandomRequestIntervalInSeconds": 12
    }
  }
}


🎮 How to Run the Application

Prerequisites
.NET 7 or .NET 8 SDK

Run locally
dotnet restore
dotnet run



🧪 Unit Testing

Frameworks Used
1. xUnit
2. Moq

Run tests
dotnet test

Test Coverage Includes
- ETA calculation logic
- Dispatch strategy behavior
- Factory strategy selection

  <img width="1468" height="761" alt="image" src="https://github.com/user-attachments/assets/0eecb1cd-b69a-4436-88aa-46a8c2ba83d9" />

<img width="1468" height="761" alt="image" src="https://github.com/user-attachments/assets/0eecb1cd-b69a-4436-88aa-46a8c2ba83d9" />

