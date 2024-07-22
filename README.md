# SignalR Chat Room

## Overview

SignalR Chat Room is a sophisticated real-time chat application designed to leverage SignalR for seamless, bidirectional communication between clients and servers. This solution comprises two primary projects:

1. **SignalRChatRoom.Client**: A front-end application developed using Blazor WebAssembly.
2. **SignalRChatRoom.Server**: A back-end application built with ASP.NET Core.

## Project Structure

### SignalRChatRoom.Client

This project handles the client-side operations of the chat room application.

- **Connected Services**: Contains references to connected services.
- **Dependencies**: Manages project dependencies.
- **Properties**
  - `launchSettings.json`: Configuration settings for application launch.
- **wwwroot**: Hosts static files.
  - `bootstrap`
  - `app.css`
  - `favicon.png`
- **Components**
  - **Layout**: Defines layout components.
  - **Pages**: Contains Blazor Razor pages.
    - `_Imports.razor`
    - `App.razor`
    - `Routes.razor`
- **Models**: Defines data models.
  - `ClientDto.cs`
  - `ClientMessage.cs`
  - `GroupDto.cs`
- **Configuration**
  - `appsettings.json`
- **Entry Point**
  - `Program.cs`: Main entry point for the client application.

### SignalRChatRoom.Server

This project handles the server-side logic of the chat room application.

- **Connected Services**: Contains references to connected services.
- **Dependencies**: Manages project dependencies.
- **Properties**
- **Controllers**: Defines API controllers for handling HTTP requests.
  - `ClientController.cs`
  - `GenericController.cs`
  - `HomeController.cs`
- **DbContexts**: Defines database context classes.
  - `ChatDbContext.cs`
- **Hubs**: Implements SignalR hubs for real-time communication.
  - `ChatHub.cs`
- **Repositories**: Contains repository interfaces and implementations.
  - `IRepository.cs`
- **Migrations**: Manages Entity Framework Core migrations.
  - `20240721134326_init.cs`
  - `ChatDbContextModelSnapshot.cs`
- **Models**: Defines data models.
- **Configuration**
  - `appsettings.json`: Contains application settings.
- **Entry Point**
  - `Program.cs`: Main entry point for the server application.

## Getting Started

### Prerequisites

- .NET 6.0 SDK or later
- Visual Studio 2022 or later

### Setup Instructions

1. **Clone the repository**:
    ```bash
    git clone https://github.com/yourusername/SignalRChatRoom.git
    cd SignalRChatRoom
    ```

2. **Restore dependencies**:
    ```bash
    dotnet restore
    ```

3. **Update the database**:
    ```bash
    cd SignalRChatRoom.Server
    dotnet ef database update
    ```

4. **Run the Server**:
    ```bash
    dotnet run --project SignalRChatRoom.Server
    ```

5. **Run the Client**:
    ```bash
    dotnet run --project SignalRChatRoom.Client
    ```

### Usage

- Open your browser and navigate to `https://localhost:5001` to access the chat room.
- Multiple browser tabs can be opened to simulate different users joining the chat room.

## Contributing

We welcome and appreciate contributions from the community. To contribute, please follow these steps:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/YourFeature`).
3. Commit your changes (`git commit -am 'Add some feature'`).
4. Push to the branch (`git push origin feature/YourFeature`).
5. Submit a Pull Request for review.

## License

This project is licensed under the MIT License. For more details, please refer to the LICENSE.md file.

## Acknowledgements

- The project architecture is influenced by standard ASP.NET Core and Blazor WebAssembly application structures.
- Special thanks to the .NET community for their extensive resources and contributions.

---

For further information or detailed documentation, please refer to the project's repository or contact the Alireza_Askari_Kochi@outlook.com directly.
