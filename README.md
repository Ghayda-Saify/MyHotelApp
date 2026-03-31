# MyHotelApp

A modern, professional Hotel Booking and Management application built with **.NET 9**, **Blazor WebAssembly**, and **MudBlazor**.

## 🚀 Overview
MyHotelApp provides a seamless experience for customers to browse hotels and rooms, while offering a robust administrative dashboard for managing cities, hotels, and inventory.

### Key Features
- **Public Website**: Featured deals, trending destinations, and advanced search for hotels and rooms.
- **Admin Dashboard**: Collapsible navigation sidebar, advanced data grids (MudTable) with sorting/filtering, and unified creation/editing forms.
- **Security**: JWT-based authentication with role-based access control (Admin/Customer).
- **Email Service**: Automatic confirmation emails for bookings via SMTP (Gmail supported).

---

## 🛠️ Tech Stack
- **Frontend**: Blazor WebAssembly, MudBlazor (UI Component Library).
- **Backend**: ASP.NET Core Web API 9.0.
- **Database**: Entity Framework Core (SQL Server).
- **Tooling**: MailKit (Email), Bogus (Seeding), BCrypt.Net (Security).

---

## 🔧 Setup & Installation

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB)
- Node.js (Optional, for advanced tooling)

### Step 1: Backend Configuration
1. Open `MyHotelApp.Api/appsettings.json`.
2. Update the `ConnectionStrings:DefaultConnectionString` to point to your SQL Server instance.
3. Configure `EmailSettings` with your SMTP server and credentials:
   ```json
   "EmailSettings": {
     "SmtpServer": "smtp.gmail.com",
     "SmtpPort": 587,
     "SmtpUser": "your-email@gmail.com",
     "SmtpPass": "your-app-password",
     "FromEmail": "your-email@gmail.com"
   }
   ```

### Step 2: Database Initialization
Run the following commands in the root directory to create the database:
```bash
cd MyHotelApp.Api
dotnet ef database update --project ..\MyHotelApp.Infrastructure --startup-project .
```

### Step 3: Run the Application
1. Start the API:
   ```bash
   dotnet run --project MyHotelApp.Api
   ```
2. Start the Blazor Client:
   ```bash
   dotnet run --project MyHotelApp.Client
   ```

---

## 📖 API Documentation
Once the API is running, you can access the **Swagger UI** to explore and test the endpoints:
- **URL**: `https://localhost:7048/swagger`

### Key Endpoints
- `POST /api/auth/login`: Authenticate and receive a JWT.
- `GET /api/Cities`: List all cities with hotel counts.
- `GET /api/Hotels/search`: Advanced search with filters (dates, capacity, price).
- `POST /api/Bookings`: Create a new room booking.

---

## 🐳 Deployment (Docker)
To run the entire stack using Docker:
```bash
docker-compose up --build
```

## 🛠️ CI/CD
This project includes a **GitHub Actions** workflow at `.github/workflows/main.yml` that automatically builds and tests the code on every push to the `main` branch.
