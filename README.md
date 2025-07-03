# Plane Crash Visualization

Eine Full-Stack-Webanwendung zur Visualisierung und Analyse von Flugzeugabsturzdaten. Das Projekt besteht aus einem ASP.NET Core Backend mit SQL Server LocalDB und einem Vue.js Frontend.

## 📋 Voraussetzungen

### Erforderliche Software

#### .NET Development
- **.NET 9.0 SDK** oder höher
  - Download: [https://dotnet.microsoft.com/download/dotnet/9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
  - Überprüfung: `dotnet --version`

#### Frontend Development
- **Node.js** (Version 18.x oder höher)
  - Download: [https://nodejs.org/](https://nodejs.org/)
  - Überprüfung: `node --version`
- **npm** (kommt mit Node.js)
  - Überprüfung: `npm --version`

#### Datenbank
- **SQL Server LocalDB** (empfohlen) oder SQL Server Express
  - Kommt mit Visual Studio oder separat installierbar
  - Alternative: SQL Server Express
  - Download: [https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)

#### Entwicklungsumgebung (Optional)
- **Visual Studio 2022** oder **Visual Studio Code**
- **Git** für Versionskontrolle

## 🏗️ Projektstruktur

```
Plane-Crash-Visualization/
├── Controllers/              # ASP.NET Core API Controllers
├── Data/                    # Datenbank-Kontext und CSV-Daten
├── Models/                  # C# Datenmodelle
├── Services/                # Business Logic Services
├── Migrations/              # Entity Framework Migrationen
├── PlaneCrashVisualizationClient/  # Vue.js Frontend
│   ├── src/
│   │   ├── components/      # Vue-Komponenten
│   │   ├── views/          # Seiten/Views
│   │   ├── services/       # API-Services
│   │   └── utils/          # Hilfsfunktionen
│   ├── package.json
│   └── vite.config.js
├── Program.cs              # Backend Entry Point
├── appsettings.json        # Konfiguration
└── README.md
```

## 🚀 Installation und Setup

### 1. Repository klonen

```bash
git clone <repository-url>
cd Plane-Crash-Visualization
```

### 2. Backend Setup

#### Abhängigkeiten installieren
```bash
dotnet restore
```

#### Datenbank konfigurieren
Die Anwendung verwendet SQL Server LocalDB. Die Verbindungszeichenfolge in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PlaneCrashDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

#### Datenbank erstellen
**Wichtig**: Führen Sie diesen Schritt vor dem ersten Start aus!

```bash
sqlcmd -S "(localdb)\mssqllocaldb" -i "Data\skript.sql"
```

#### Entity Framework Tools installieren (falls nicht vorhanden)
```bash
dotnet tool install --global dotnet-ef
```

#### Backend bauen und testen
```bash
dotnet build
```

### 3. Frontend Setup

```bash
cd PlaneCrashVisualizationClient
npm install
```

### 4. Anwendung starten

#### Backend starten (Terminal 1)
```bash
# Im Hauptverzeichnis
dotnet run
```
Das Backend läuft auf: `http://localhost:5021`

#### Frontend starten (Terminal 2)
```bash
# Im PlaneCrashVisualizationClient Verzeichnis
cd PlaneCrashVisualizationClient
npm run dev
```
Das Frontend läuft auf: `http://localhost:5173`

## 📊 Features

### Backend (ASP.NET Core Web API)
- **RESTful API** für Flugzeugabsturzdaten
- **Entity Framework Core** für Datenbankzugriff
- **SQL Server LocalDB** Integration
- **Automatischer CSV-Import** beim ersten Start
- **Geocoding Service** für Koordinaten
- **CORS-Unterstützung** für Frontend-Integration

### Frontend (Vue.js 3)
- **Interactive Map** mit Leaflet.js
- **Charts und Visualisierungen** mit Chart.js
- **Responsive Design** mit Bootstrap 5
- **Erweiterte Suchfunktionen**
- **Trend-Analysen und Statistiken**
- **Echtzeit-Datenfilterung**

## 🔧 Verwendete Technologien

### Backend
- **ASP.NET Core 9.0** - Web API Framework
- **Entity Framework Core 9.0** - ORM
- **SQL Server LocalDB** - Datenbank
- **CsvHelper** - CSV-Datenverarbeitung
- **Microsoft.Data.SqlClient** - Datenbankverbindung

### Frontend
- **Vue.js 3** - Frontend Framework
- **Vite** - Build Tool
- **Vue Router** - Navigation
- **Axios** - HTTP Client
- **Leaflet.js** - Interaktive Karten
- **Chart.js** - Datenvisualisierung
- **Bootstrap 5** - UI Framework
- **Bootstrap Icons** - Icons

## 🗃️ Datenbank

### Manuelle Datenbankinitialisierung
**Wichtig**: Die Datenbank muss vor dem ersten Start der Anwendung manuell erstellt werden.

#### Datenbank mit SQL-Skript erstellen
Führen Sie den folgenden Befehl im Terminal/Eingabeaufforderung aus:

```bash
sqlcmd -S "(localdb)\mssqllocaldb" -i "Data\skript.sql"
```

**Hinweise:**
- Stellen Sie sicher, dass SQL Server LocalDB installiert und gestartet ist
- Der Pfad zur `skript.sql` muss relativ zum aktuellen Verzeichnis korrekt sein
- Das Skript erstellt die Datenbank `PlaneCrashDB` mit allen Tabellen und Daten

#### Alternative: SQL Server Management Studio (SSMS)
1. Öffnen Sie SSMS
2. Verbinden Sie sich mit `(localdb)\mssqllocaldb`
3. Öffnen Sie die Datei `Data/skript.sql`
4. Führen Sie das Skript aus (F5)

### Datenmodell
- **Crashes** - Haupttabelle mit Absturzdaten
- **Felder**: Datum, Ort, Fluggesellschaft, Flugzeugtyp, Passagiere, Opfer, etc.
- **Koordinaten** für Kartenvisualisierung
- **Hersteller/Modell-Extraktion** aus Flugzeugtyp

## 🌐 API-Endpunkte

### Hauptendpunkte
- `GET /api/crashes` - Alle Abstürze mit Filteroptionen
- `GET /api/crashes/map-data` - Daten für Kartenvisualisierung
- `GET /api/crashes/summary` - Zusammenfassungsstatistiken
- `GET /api/crashes/by-year` - Abstürze nach Jahr
- `GET /api/crashes/by-operator` - Abstürze nach Fluggesellschaft
- `GET /api/crashes/most-common-aircraft` - Häufigste Flugzeugmodelle
- `GET /api/crashes/most-common-manufacturers` - Häufigste Hersteller
- `GET /api/crashes/by-continent` - Abstürze nach Kontinent

## 🔍 Troubleshooting

### Häufige Probleme

#### Backend startet nicht
- Überprüfen Sie, ob .NET 9.0 SDK installiert ist: `dotnet --version`
- Stellen Sie sicher, dass Port 5021 nicht belegt ist

#### Datenbankfehler
- Überprüfen Sie, ob SQL Server LocalDB läuft
- Stellen Sie sicher, dass die Datenbank mit dem SQL-Skript erstellt wurde:
  ```bash
  sqlcmd -S "(localdb)\mssqllocaldb" -i "Data\skript.sql"
  ```
- Prüfen Sie die Verbindungszeichenfolge in `appsettings.json`

#### Frontend lädt nicht
- Überprüfen Sie, ob Node.js installiert ist: `node --version`
- Löschen Sie `node_modules` und führen Sie `npm install` erneut aus
- Stellen Sie sicher, dass das Backend auf Port 5021 läuft

#### API-Fehler (400 Bad Request)
- Stellen Sie sicher, dass die Datenbank mit dem SQL-Skript erstellt wurde
- Das Backend benötigt Daten in der Datenbank - diese sind im SQL-Skript enthalten
- Überprüfen Sie die Browser-Konsole für detaillierte Fehlermeldungen

## 📝 Entwicklung

### Neue Features hinzufügen
1. Backend: Controller/Services erweitern
2. Frontend: Neue Views/Komponenten erstellen
3. API-Integration mit Axios

### Datenbank-Änderungen
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

## 📄 Lizenz

[Lizenzinformationen hier einfügen]

## 🤝 Beiträge

Beiträge sind willkommen! Bitte erstellen Sie einen Pull Request oder öffnen Sie ein Issue.

---

**Hinweis**: Diese Anwendung dient zu Bildungs- und Analysezwecken. Die Daten stammen aus öffentlichen Quellen und dienen der historischen Dokumentation von Luftfahrtereignissen.