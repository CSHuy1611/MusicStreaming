# 🎵 Music Streaming Platform

A robust and scalable Music Streaming Web Application built using **ASP.NET Core** and **Clean Architecture**. This platform allows users to stream music, create playlists, authenticate via Google, and upgrade to VIP subscriptions using the PayOS payment gateway. The project also includes an Admin dashboard for system management and real-time updates via SignalR.

---

## 🚀 Key Features

* **User Authentication & Authorization**: Secure login system using ASP.NET Core Identity, JWT, Cookie Authentication, and **Google OAuth** integration.
* **Music Player & Library**: Browse, search, and play songs. Users can create custom playlists and mark songs as favorites.
* **VIP Subscription System**: Integrated with **PayOS** API to process real-time payments via QR Code webhooks for VIP status upgrades.
* **Real-time Notifications**: Utilizes **SignalR** to push real-time updates to the dashboard without page reloads.
* **Admin Management Panel**: A dedicated `AdminApp` to manage songs, generic data, subscriptions, and singers.
* **Clean Architecture & CQRS**: High maintainability by separating Domain, Application, Infrastructure, and Presentation layers, combined with the MediatR/CQRS pattern.

---

## 🏗️ Technical Stack

* **Backend Framework**: ASP.NET Core (MVC / Web API)
* **Architecture**: Clean Architecture, CQRS, Unit of Work & Repository Pattern
* **Database**: Microsoft SQL Server with **Entity Framework Core**
* **Real-time Communication**: SignalR
* **Authentication**: ASP.NET Core Identity, JWT (JSON Web Tokens)
* **External Integrations**: PayOS (Payment Gateway), Google Identity (OAuth)

---

## 📁 Project Structure

The solution `MusicStreaming.sln` is divided into the following sub-projects:

* **`MusicStreaming.Domain`**: Contains core enterprise logic, Entities (e.g., `ApplicationUser`, `Song`, `Playlist`), Enums, and custom exceptions.
* **`MusicStreaming.Application`**: The business logic layer containing CQRS Commands, Queries, Handlers, DTOs, and Interfaces (e.g., `IGenericRepository`).
* **`MusicStreaming.Infrastructure`**: Contains the data access implementation (`ApplicationDbContext`), Entity configurations, Identity setups, and external service implementations (PayOS, Email).
* **`MusicStreaming.Web`**: The main user-facing application (ASP.NET Core MVC). Includes Controllers, Views, and static files for the end-user music player interface.
* **`MusicStreaming.AdminApp`**: A separate application dedicated to administrators for managing the platform's resources.
* **`MusicStreaming.TcpServer`**: An auxiliary TCP server project (potentially for specific socket-based streaming or sync tasks).

---

## 🛠️ Getting Started

### Prerequisites

* [.NET 8.0 SDK](https://dotnet.microsoft.com/download) (or later)
* [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or standard instance)
* Visual Studio 2022 or Visual Studio Code

### Installation & Setup

1. **Clone the repository** (if not already local):
   ```bash
   git clone https://github.com/YOUR_USERNAME/MusicStreaming.git
   cd MusicStreaming
   ```

2. **Database Configuration**:
   Open `MusicStreaming.Web/appsettings.json` (and the admin app's settings) and update the `DefaultConnection` string to point to your SQL Server instance:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MusicStreamingDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

3. **External Services Configuration**:
   Create a `appsettings.Development.json` or use User Secrets to securely add your API keys:
   ```json
   "Google": {
     "ClientId": "YOUR_GOOGLE_CLIENT_ID",
     "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
   },
   "JwtSettings": {
     "Secret": "YOUR_JWT_SECRET_KEY",
     "Issuer": "...",
     "Audience": "..."
   },
   "PayOS": {
     "ClientId": "...",
     "ApiKey": "...",
     "ChecksumKey": "..."
   }
   ```

4. **Apply EF Core Migrations**:
   Open the Package Manager Console or use the .NET CLI to update the database:
   ```bash
   dotnet ef database update --project MusicStreaming.Infrastructure --startup-project MusicStreaming.Web
   ```
   *(Note: The application includes a `ApplicationDbContextInitialiser` which may automatically seed the initial database upon startup).*

5. **Run the Application**:
   Set `MusicStreaming.Web` as the Startup Project and hit **F5** in Visual Studio, or run:
   ```bash
   dotnet run --project MusicStreaming.Web/MusicStreaming.Web.csproj
   ```

---

## 🤝 Contributing

Contributions, issues, and feature requests are welcome! 

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📝 License

Distributed under the MIT License. See `LICENSE` for more information.
