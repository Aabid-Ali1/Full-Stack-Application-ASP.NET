# Interactive Full Stack Web Application (.NET)

A full stack web application built with an interactive user interface and an ASP.NET backend. The application uses asynchronous client–server communication and a SQL database to support dynamic, database-driven features.

---

## Technologies Used

### Frontend
- HTML5
- CSS
- JavaScript (AJAX)

### Backend
- ASP.NET (C#)
- REST-style endpoints

### Database
- SQL (SQL Server / MySQL)

### Tools
- Visual Studio
- Git
- GitHub

---

## Features

- Interactive user interface with dynamic updates
- Asynchronous client–server communication using AJAX
- Create, read, update, and delete (CRUD) operations
- Server-side validation and error handling
- SQL-based data persistence

---

## How It Works

1. Users interact with the frontend UI.
2. JavaScript sends asynchronous AJAX requests to ASP.NET endpoints.
3. The backend processes requests using C# and communicates with the SQL database.
4. Query results are returned as structured responses (JSON).
5. The frontend updates the UI dynamically without requiring a full page reload.

This project follows a clear separation of concerns between the UI, server logic, and data layers.

---

## Running the Project Locally

1. Install the .NET SDK.
2. Clone this repository:
   ```bash
   git clone https://github.com/Aabid-Ali1/Full-Stack-Application-ASP.NET
   
   To run this project locally, you will need:
- .NET 8 SDK
- SQL Server (local or containerized)

Update the connection string in `appsettings.Development.json`
with your own database credentials.

## Configuration
This project uses `appsettings.json` for placeholder configuration.
Sensitive database credentials are stored locally in `appsettings.Development.json`
and are not included in this repository.
