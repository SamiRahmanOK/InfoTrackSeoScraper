# InfoTrack SEO Scraper

A web application that scrapes search engine results to check the ranking position of a specified URL for a given search query.

## Features

- Search for a target URL's position in search engine results
- Support for multiple search engines (currently Bing, with Google implementation disabled due to Terms of Service)
- Save and view historical search results
- Clean, responsive UI built with React
- RESTful API built with ASP.NET Core

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (v14 or later)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or full version)
- Visual Studio 2022 or Visual Studio Code
- A modern web browser (Chrome, Edge, or Firefox recommended)

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/SamiRahmanOK/InfoTrackSeoScraper.git
cd InfoTrackSeoScraper
```

### Database Setup

The application uses Entity Framework Core with SQL Server. Follow these steps to set up the database:

1. Open the `appsettings.json` file in the InfoTrackSeoScraper.Main project and ensure the connection string is appropriate for your environment:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=InfoTrackSeoScraper;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

2. Run the following commands from the project root to create the database:

```bash
cd InfoTrackSeoScraper.Main
dotnet ef database update
```

Alternatively, if you're using Visual Studio:
- Open the Package Manager Console
- Set the Default Project to InfoTrackSeoScraper.Main
- Run: `Update-Database`

### Running the Application

#### Using Visual Studio:

1. Open the solution file (`InfoTrackSeoScraper.sln`) in Visual Studio
2. Set InfoTrackSeoScraper.Main as the startup project
3. Press F5 or click the "Start" button to run the application
4. The React frontend will automatically build and launch in your default browser

#### Using Command Line:

1. Start the backend API:
```bash
cd InfoTrackSeoScraper.Main
dotnet run
```

2. In a new terminal, start the React development server:
```bash
cd InfoTrackSeoScraper.Main/ClientApp
npm install
npm start
```

3. The application should now be running at:
   - API: https://localhost:7210 (based on your launchSettings.json file)
   - Frontend: https://localhost:44495 (based on your SpaProxyServerUrl in your project file)

## Project Structure

- **InfoTrackSeoScraper.Main** - Main application including API and frontend
  - `/Core` - Domain entities and interfaces
  - `/Application` - Service implementations and DTOs
  - `/Infrastructure` - Data access, external services
  - `/Presentation` - API Controllers
  - `/ClientApp` - React frontend
- **InfoTrackSeoScraper.Tests** - Unit and integration tests

## Running Tests

You can run the tests using Visual Studio's Test Explorer or via the command line:

```bash
cd InfoTrackSeoScraper.Tests
dotnet test
```

The test suite includes:
- Controller tests for API endpoints
- Repository tests for data access
- Service tests for business logic

## Browser Compatibility

The application is designed to work best in modern browsers:
- Google Chrome
- Microsoft Edge
- Mozilla Firefox
- Safari

Internet Explorer is not supported.

## Usage

1. Enter a search query (e.g., "land registry searches")
2. Enter a target URL (e.g., "infotrack.co.uk")
3. Select a search engine (currently only Bing is available for use)
4. Click "Search" to find the ranking positions of the target URL
5. View your search history by clicking "View Search History"

## Technologies

- **Backend**:
  - ASP.NET Core 8
  - Entity Framework Core
  - Polly for resilience
  - HtmlAgilityPack for HTML parsing
  
- **Frontend**:
  - React
  - React Router
  - CSS for styling