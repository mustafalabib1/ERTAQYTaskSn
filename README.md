# ERTAQYTaskSn

[![.NET 8](https://img.shields.io/badge/.NET-8-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

This project is a .NET 8 web application built with an N-Tier architecture. It provides a simple interface to manage products and service providers.

## Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Setup and Installation](#setup-and-installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Features

- **Product Management:**
    - Create, Read, Update, and Delete (CRUD) operations for products.
    - Filter products by name, price, and quantity.
- **Service Provider Management:**
    - CRUD operations for service providers.

## Technologies Used

- **.NET 8**
- **ASP.NET Core MVC**
- **SQL Server**
- **Dapper:** For data access.
- **AutoMapper:** For object-to-object mapping.

## Architecture

The project follows a classic N-Tier architecture to separate concerns:

- **PLProject (Presentation Layer):** The main web application built with ASP.NET Core MVC. It handles user interactions, routing, and displaying data to the user.
- **BLLProject (Business Logic Layer):** Contains the core business logic, services, and repository interfaces. It acts as an intermediary between the Presentation and Data Access Layers.
- **DALProject (Data Access Layer):** Responsible for all data access and persistence. It interacts directly with the SQL Server database using Dapper and contains the data entities.

## Project Structure

```
.
├── BLLProject/         # Business Logic Layer
│   ├── Interface/
│   ├── Repository/
│   └── UnitOfWorkPattern/
├── DALProject/         # Data Access Layer
│   ├── DbInitializer/
│   └── Entities/
├── ERTAQYTask/         # Presentation Layer (ASP.NET Core MVC)
│   ├── Controllers/
│   ├── Views/
│   ├── ViewModel/
│   └── ...
└── ERTAQYTaskSn.sln
```

## Setup and Installation

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/your-username/ERTAQYTaskSn.git
    cd ERTAQYTaskSn
    ```
2.  **Open in Visual Studio:**
    - Open the `ERTAQYTaskSn.sln` file in Visual Studio 2022 or later.
3.  **Configure Database Connection:**
    - Open `appsettings.json` located in the `ERTAQYTask` project.
    - Modify the `DefaultConnection` string to point to your local or remote SQL Server instance.
4.  **Initialize the Database:**
    - The project is configured with a database initializer. When you run the application for the first time, it will automatically create the database schema and seed it with initial data.
5.  **Run the application:**
    - Press `F5` or click the "Run" button in Visual Studio to build and start the web application.

## Usage

Once the application is running, you can access the following pages:

-   **Products:** Navigate to `/Product` to manage products.
-   **Service Providers:** Navigate to `/ServiceProvider` to manage service providers.
