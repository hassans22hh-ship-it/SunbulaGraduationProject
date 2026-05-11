# Sunbula Backend Setup Guide

Welcome to the Sunbula Backend project. This guide will help you set up the database and configure your local environment so you can run the application successfully.

## Prerequisites

Before you begin, ensure you have the following installed on your machine:
- **.NET 10 SDK** or higher.
- **SQL Server** (LocalDB, Express, or Developer edition).
- **SQL Server Management Studio (SSMS)** or Azure Data Studio.

## 1. Configure the Server Name in `appsettings.json`

The backend needs to know where your SQL Server is hosted. You must update the connection string in the `appsettings.json` file.

1. Open the file located at: `Sunbula/appsettings.json`.
2. Locate the `ConnectionStrings` section.
3. Update the `Server=` portion of the `SunbulaDb` connection string with your SQL Server instance name.
   - If you are using the default local instance, `Server=.;` or `Server=localhost;` is usually sufficient.
   - If you are using SQL Server Express, it might be `Server=.\SQLEXPRESS;`.

**Example:**
```json
"ConnectionStrings": {
  "SunbulaDb": "Server=.;Database=Sunbula;Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=30"
}
```

## 2. Email Service Configuration (Required for Registration)

The application requires an email service to send confirmation emails during user registration.

1. Open `Sunbula/appsettings.json`.
2. Locate the `EmailSettings` section.
3. Update the `Email` and `Password` fields.
   - If using **Gmail**, you should use an **App Password** rather than your regular account password for security.

```json
"EmailSettings": {
  "Email": "your-email@gmail.com",
  "Password": "your-app-password",
  "Host": "smtp.gmail.com",
  "Port": 587
}
```

## 3. Apply Database Migrations (Update-Database)

The project uses a modular architecture with multiple `DbContext` classes. You need to apply migrations for **each** context to fully create the database schema.

### Using Visual Studio
Open the **Package Manager Console** (`View` -> `Other Windows` -> `Package Manager Console`), ensure the `Default project` is set to the correct infrastructure project or the main `Sunbula` API project, and run the following commands sequentially:

```powershell
Update-Database -Context UserIdentityDbContext
Update-Database -Context TimeTrackingDbContext
Update-Database -Context TaskManagementDbContext
Update-Database -Context FinanceDbContext
Update-Database -Context DebtDbContext
```

### Using .NET CLI
Run these commands from the root directory:
```bash
dotnet ef database update --context UserIdentityDbContext --project Sunbula
dotnet ef database update --context TimeTrackingDbContext --project Sunbula
dotnet ef database update --context TaskManagementDbContext --project Sunbula
dotnet ef database update --context FinanceDbContext --project Sunbula
dotnet ef database update --context DebtDbContext --project Sunbula
```

## 4. Validate the Database in SSMS (SQL Server Management Studio)

Once the migrations are successfully applied, verify that the database and tables have been created correctly.

1. Open **SQL Server Management Studio (SSMS)**.
2. In the **Connect to Server** dialog, enter the same Server Name you used in `appsettings.json` (e.g., `.`, `localhost`, or `.\SQLEXPRESS`) and click **Connect**.
3. In the **Object Explorer** on the left panel, expand the **Databases** folder.
4. Look for the **Sunbula** database.
5. Expand **Sunbula** -> **Tables**.
6. **Validation:** You should see various tables created by the different modules, for instance:
   - `dbo.Users` (from `UserIdentityDbContext`)
   - Task-related tables (from `TaskManagementDbContext`)
   - Finance-related tables (from `FinanceDbContext`)
   - Debt-related tables (from `DebtDbContext`)
   - Time-tracking tables (from `TimeTrackingDbContext`)

## 5. Running the Application

To run the backend, navigate to the `Sunbula` project folder and run:

```bash
dotnet run
```
Or simply press **F5** in Visual Studio.

### API Documentation (Swagger)
Once the application is running, you can access the Swagger UI to explore and test the API endpoints at:
`http://localhost:5142/swagger` (or the port shown in your terminal).

## 6. Frontend Integration
The frontend project is located in the `SunbulaFrontEnd` repository. Ensure it is running on `http://localhost:4200` to allow CORS communication with this backend.

