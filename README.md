# ERTAQYTaskSn

This project is a .NET 8 web application built with an N-Tier architecture. It allows for managing products and service providers.

## Technologies Used

*   **.NET 8**
*   **ASP.NET Core MVC**
*   **SQL Server**
*   **Dapper**
*   **AutoMapper**

## Architecture

The project follows a classic N-Tier architecture:

*   **PLProject (Presentation Layer):** The web application itself, built with ASP.NET Core MVC. It handles user interaction and displays data.
*   **BLLProject (Business Logic Layer):** Contains the business logic of the application. It acts as an intermediary between the Presentation Layer and the Data Access Layer.
*   **DALProject (Data Access Layer):** Responsible for data access and persistence. It interacts with the SQL Server database using Dapper.

## Project Structure

*   `ERTAQYTaskSn.sln`: The solution file.
*   `ERTAQYTask/`: The main web application project (Presentation Layer).
    *   `Controllers/`: Contains the MVC controllers.
    *   `Views/`: Contains the Razor views.
    *   `ViewModel/`: Contains the view models.
    *   `Program.cs`: The application entry point.
*   `BLLProject/`: The Business Logic Layer.
    *   `Repository/`: Contains the repositories for accessing data.
    *   `Interface/`: Contains the repository interfaces.
    *   `UnitOfWorkPattern/`: Contains the Unit of Work implementation.
*   `DALProject/`: The Data Access Layer.
    *   `Entities/`: Contains the database entities.
    *   `DbInitializer/`: Contains the database initializer.

## Setup and Installation

1.  **Clone the repository:**
    ```bash
    git clone <repository-url>
    ```
2.  **Open the solution in Visual Studio.**
3.  **Configure the database connection:**
    *   Open `appsettings.json` in the `ERTAQYTask` project.
    *   Modify the `DefaultConnection` connection string to point to your SQL Server instance.
4.  **Initialize the database:**
    *   The project includes a database initializer. When you run the application for the first time, it should create and seed the database.
5.  **Run the application:**
    *   Press F5 or click the "Run" button in Visual Studio.

## Usage

Once the application is running, you can navigate to the following URLs:

*   `/Product`: To manage products.
*   `/ServiceProvider`: To manage service providers.
