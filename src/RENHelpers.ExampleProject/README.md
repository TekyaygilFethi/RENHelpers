# DataAccessHelper Operations Example

This project demonstrates the usage of `DataAccessHelper` operations in a .NET 8 application. It includes various operations such as querying, caching, bulk inserts, transactions, and more, using a repository and unit of work pattern.

## Features

- **Entity Framework Core Integration**: Simplifies database interactions.
- **Repository Pattern**: Encapsulates data access logic.
- **Unit of Work**: Manages transactions and ensures consistency.
- **Caching**: Demonstrates how to use caching for efficient data access.
- **Bulk Operations**: Efficiently insert, update, delete large datasets into the database.
- **Transaction Management**: Handle multiple operations in a single transaction.
- **Queryable Support**: Use LINQ queries directly for flexible data retrieval.

---

## Requirements

- **.NET 8 SDK**: [Download .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Database**: SQL Server (or any compatible provider configured in the `appsettings.json`).
- **NuGet Packages**:
  - `Microsoft.EntityFrameworkCore`
  - `Microsoft.EntityFrameworkCore.SqlServer`
  - `Bogus` (for generating test data)
  - `EFCore.BulkExtensions` (for bulk operations)
  - `StackExchange.Redis` (for caching, optional)

---

## Installation

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/YourGitHubUsername/DataAccessHelperExample.git
   cd DataAccessHelperExample
   ```

---

2. Configure Database and Cache Connection: Update the connection string in appsettings.json:
	```json
    {
	    "ConnectionStrings": {
            "Dev": "Server=localhost,1499;Database=RENTestDb;User Id=sa;Password=mypwd;TrustServerCertificate=True"
        },
        "CacheConfiguration": {
            "RedisConfiguration": {
                "Url": "localhost:18003",
                "TimeConfiguration": {
                    "AbsoluteExpirationInHours": 12
                },
                "DatabaseId": 0,
                "Username": "default",
                "Password": "mypwd",
                "AbortOnConnectFail": false
            },
            "InMemoryConfiguration": {
                "TimeConfiguration": {
                    "AbsoluteExpirationInHours": 12,
                    "SlidingExpirationInMinutes": 30
                }
            }
        }
    }
	```

---

3. Apply Migrations: Run the following commands to create the database and apply migrations:

   ```bash
   dotnet ef database update --project DataAccessHelperExample
   ```

---

4. Run the Application: 

- Locally: Start the application locally using the following command:

   ```bash
   dotnet run --project DataAccessHelperExample
   ```

- Docker: You can containerize and run the project using Docker. Follow these steps:
    - Build the Docker Image: Ensure you have a Dockerfile in your project root. Build the image using:
         ```bash
         docker build -t dataaccesshelper-example .
         ```
    - Run the Docker Container: Start the container using:
        ```bash
		 docker run -d -p 8080:80 --name dataaccesshelper-container dataaccesshelper-example
		 ```

---

## Test
You can use .http file within the project, Postman, or Swagger to test the endpoints out!

---

## How to Extend

### Add New Entities

1. Define new entity classes in the `Models` folder.
2. Add `DbSet` properties in the `DbContext`.
3. Use `IRENRepository<T>` to handle CRUD operations.

### Add New Endpoints

Create new controller actions for your operations.

### Customize Caching

Extend the `IRENCacheService` to include additional caching strategies.

---

## Feedback

Feel free to fork, use, and submit issues or pull requests. Your feedback is welcome!

---

## Documentation

[Documentation](https://fethis-organization.gitbook.io/ren-regular-everyday-normal-helper/)
