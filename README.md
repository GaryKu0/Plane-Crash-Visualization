# Plane Crash Visualization

A full-stack web application for visualizing and analyzing aircraft crash data. The project consists of an ASP.NET Core backend with SQL Server LocalDB and a Vue.js frontend.

## 📋 Prerequisites

### Required Software

#### .NET Development
- **.NET 9.0 SDK** or higher
  - Download: [https://dotnet.microsoft.com/download/dotnet/9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
  - Check: `dotnet --version`

#### Frontend Development
- **Node.js** (Version 18.x or higher)
  - Download: [https://nodejs.org/](https://nodejs.org/)
  - Check: `node --version`
- **npm** (comes with Node.js)
  - Check: `npm --version`

#### Database
- **SQL Server LocalDB** (recommended) or SQL Server Express
  - Comes with Visual Studio or can be installed separately
  - Alternative: SQL Server Express
  - Download: [https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)

#### Development Environment (Optional)
- **Visual Studio 2022** or **Visual Studio Code**
- **Git** for version control

## 🏗️ Project Structure

```
Plane-Crash-Visualization/
├── Controllers/              # ASP.NET Core API Controllers
├── Data/                    # Database context and CSV data
├── Models/                  # C# data models
├── Services/                # Business logic services
├── Migrations/              # Entity Framework migrations
├── PlaneCrashVisualizationClient/  # Vue.js Frontend
│   ├── src/
│   │   ├── components/      # Vue components
│   │   ├── views/          # Pages/Views
│   │   ├── services/       # API services
│   │   └── utils/          # Utility functions
│   ├── package.json
│   └── vite.config.js
├── Program.cs              # Backend entry point
├── appsettings.json        # Configuration
└── README.md
```

## 🚀 Installation and Setup

### 1. Clone Repository

```bash
git clone https://github.com/Argaros/Plane-Crash-Visualization
cd Plane-Crash-Visualization
```

### 2. Backend Setup

#### Install Dependencies
```bash
dotnet restore
```

#### Configure Database
The application uses SQL Server LocalDB. Connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PlaneCrashDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

#### Create Database
**Important**: Execute this step before first startup!

```bash
sqlcmd -S "(localdb)\mssqllocaldb" -i "Data\skript.sql"
```

#### Install Entity Framework Tools (if not present)
```bash
dotnet tool install --global dotnet-ef
```

#### Build and Test Backend
```bash
dotnet build
```

### 3. Frontend Setup

```bash
cd PlaneCrashVisualizationClient
npm install
```

### 4. Start Application

#### Start Backend (Terminal 1)
```bash
# In main directory
dotnet run
```
Backend runs on: `http://localhost:5021`

#### Start Frontend (Terminal 2)
```bash
# In PlaneCrashVisualizationClient directory
cd PlaneCrashVisualizationClient
npm run dev
```
Frontend runs on: `http://localhost:5173`

## 📊 Features

### Backend (ASP.NET Core Web API)
- **RESTful API** for aircraft crash data
- **Entity Framework Core** for database access
- **SQL Server LocalDB** integration
- **Automatic CSV import** on first startup
- **Geocoding service** for coordinates
- **CORS support** for frontend integration
- **Swagger/OpenAPI** documentation

### Frontend (Vue.js 3)
- **Interactive map** with Leaflet.js
- **Charts and visualizations** with Chart.js
- **Responsive design** with Bootstrap 5
- **Advanced search functions**
- **Trend analysis and statistics**
- **Real-time data filtering**

## 🔧 Technologies Used

### Backend
- **ASP.NET Core 9.0** - Web API framework
- **Entity Framework Core 9.0** - ORM
- **SQL Server LocalDB** - Database
- **CsvHelper** - CSV data processing
- **Swashbuckle.AspNetCore** - Swagger/OpenAPI documentation
- **Microsoft.Data.SqlClient** - Database connection

### Frontend
- **Vue.js 3** - Frontend framework
- **Vite** - Build tool
- **Vue Router** - Navigation
- **Axios** - HTTP client
- **Leaflet.js** - Interactive maps
- **Chart.js** - Data visualization
- **Bootstrap 5** - UI framework
- **Bootstrap Icons** - Icons

## 🗃️ Database

### Manual Database Initialization
**Important**: The database must be created manually before the first application startup.

#### Create Database with SQL Script
Run the following command in Terminal/Command Prompt:

```bash
sqlcmd -S "(localdb)\mssqllocaldb" -i "Data\skript.sql"
```

**Notes:**
- Ensure SQL Server LocalDB is installed and running
- The path to `skript.sql` must be correct relative to the current directory
- The script creates the `PlaneCrashDB` database with all tables and data

#### Alternative: SQL Server Management Studio (SSMS)
1. Open SSMS
2. Connect to `(localdb)\mssqllocaldb`
3. Open the file `Data/skript.sql`
4. Execute the script (F5)

### Data Model
- **Crashes** - Main table with crash data
- **Fields**: Date, location, airline, aircraft type, passengers, casualties, etc.
- **Coordinates** for map visualization
- **Manufacturer/model extraction** from aircraft type

## 🌐 API Endpoints

### API Documentation
- **Swagger UI**: `http://localhost:5021/swagger/index.html`
  - Interactive API documentation
  - Direct endpoint testing available
  - Complete request/response schemas

### Main Endpoints
- `GET /api/crashes` - All crashes with filter options
- `GET /api/crashes/map-data` - Data for map visualization
- `GET /api/crashes/summary` - Summary statistics
- `GET /api/crashes/by-year` - Crashes by year
- `GET /api/crashes/by-operator` - Crashes by airline
- `GET /api/crashes/most-common-aircraft` - Most common aircraft models
- `GET /api/crashes/most-common-manufacturers` - Most common manufacturers
- `GET /api/crashes/by-continent` - Crashes by continent

## 🔍 Troubleshooting

### Common Issues

#### Backend doesn't start
- Check if .NET 9.0 SDK is installed: `dotnet --version`
- Ensure port 5021 is not occupied

#### Database errors
- Check if SQL Server LocalDB is running
- Ensure the database was created with the SQL script:
  ```bash
  sqlcmd -S "(localdb)\mssqllocaldb" -i "Data\skript.sql"
  ```
- Check the connection string in `appsettings.json`

#### Frontend doesn't load
- Check if Node.js is installed: `node --version`
- Delete `node_modules` and run `npm install` again
- Ensure the backend is running on port 5021

#### API errors (400 Bad Request)
- Ensure the database was created with the SQL script
- The backend requires data in the database - this is included in the SQL script
- Check the browser console for detailed error messages

## 📝 Development

### Adding New Features
1. Backend: Extend controllers/services
2. Frontend: Create new views/components
3. API integration with Axios

### Database Changes
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

## 📄 License

[Insert license information here]

## 🤝 Contributing

Contributions are welcome! Please create a pull request or open an issue.

---

**Note**: This application serves educational and analytical purposes. The data comes from public sources and serves for historical documentation of aviation events.
