# KaunasBusStop

KaunasBusStop is a web application providing detailed information about bus stops and routes in Kaunas, Lithuania. It is built with **ASP.NET Core** and **React**, using **GTFS (General Transit Feed Specification)** files for transit data and **OpenStreetMap** for interactive visualization of bus stops and routes.

## Features

- **Route Information**: View bus routes and schedules.
- **Interactive Map**: Explore bus stops and routes on an OpenStreetMap.
- **Stop Details**: Access details for specific bus stops.
- **Route visualization**: Visualize route between stops.

## Technologies Used

- **Backend**: ASP.NET Core
- **Frontend**: React
- **Data Source**: GTFS files for transit data
- **Map Integration**: OpenStreetMap
- **Database**: MSSQL
- **Development Environment**: Visual Studio

## Screenshots

### 1. Bus Routes Overview
![Routes](https://github.com/user-attachments/assets/b2c72546-35b0-4dc2-9afb-426f2d4e6020)

### 2. Map with Bus Stops
![map with stops data](https://github.com/user-attachments/assets/a2395b1c-69ef-4ee5-b42a-b488e121901e)


### 3. Route Between Stops
![Route stops data and driver roate](https://github.com/user-attachments/assets/b23ddfed-b407-41f4-8c2e-1dc1d924aa7c)


## Getting Started

### Prerequisites
- .NET SDK (for ASP.NET Core)
- Node.js (for React)
- Visual Studio (recommended) or any code editor
- Database (e.g., SQL Server) if using Entity Framework Core

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/kajuss8/KaunasBusStop.git

2. Navigate to the project directory:
   ```bash
   cd KaunasBusStop

3. Restore .NET dependencies:
   ```bash
   dotnet restore

4. Install Node.js dependencies for React:
   ```bash
   npm install

5. Configure GTFS file path and database connection in appsettings.json (e.g., connection string for Entity Framework Core).

6. Apply database migrations (if using Entity Framework Core):
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
Note: Ensure the database server is running and connection settings are correct.

7. Run the application:
   ```bash
   dotnet run
