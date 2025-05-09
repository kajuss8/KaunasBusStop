# KaunasBusStop

KaunasBusStop is a web application providing detailed information about bus stops and routes in Kaunas, Lithuania. It is built with **ASP.NET Core** and **React**, using **GTFS (General Transit Feed Specification)** files for transit data and **OpenStreetMap** for interactive visualization of bus stops and routes.

## Features

- **Route Information**: View bus routes and schedules.
- **Interactive Map**: Explore bus stops and routes on an OpenStreetMap-powered map.
- **Stop Details**: Access details for specific bus stops, including routes served.
- **Route Planning**: Visualize routes between selected stops with stop information.

## Technologies Used

- **Backend**: ASP.NET Core
- **Frontend**: React
- **Data Source**: GTFS files for transit data
- **Map Integration**: OpenStreetMap for geospatial visualization
- **Database**: Entity Framework Core (if applicable)
- **Development Environment**: Visual Studio

## Screenshots

### 1. Bus Routes Overview
![Routes Overview](images/routes.png)
*Displays available bus routes and their schedules.*

### 2. Map with Bus Stops
![Map with Stops](images/stops_map.png)
*Interactive map showing bus stop locations using OpenStreetMap.*

### 3. Route Between Stops
![Route and Stop Info](images/route_stops.png)
*Visualizes a route between selected stops with detailed stop information.*

## Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (for ASP.NET Core)
- [Node.js](https://nodejs.org/) (for React)
- Visual Studio (recommended) or any code editor
- GTFS files for Kaunas transit data (place in the project directory or configure the path)
- Database (e.g., SQL Server, SQLite) if using Entity Framework Core

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/KaunasBusStop.git

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
   npm install
