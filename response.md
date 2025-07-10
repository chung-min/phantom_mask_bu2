# Response

## Overview

This project is a backend API for PhantomMask, using ASP.NET Core and SQL Server running in Docker containers.

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/) installed and running
- [.NET SDK](https://dotnet.microsoft.com/download) .NET SDK version 8.0 or later is installed (make sure `dotnet` command works)
- SQL Server 2019 Docker image (`mcr.microsoft.com/mssql/server:2019-latest`)

### Built With
- ASP.NET Core Web API
- Entity Framework Core
- Docker & Docker Compose
- SQL Server 2019 (Docker)

## Requirement Completion Rate
* [x] List pharmacies, optionally filtered by specific time and/or day of the week.
  * Implemented at `pharmacies` API.
* [x] List all masks sold by a given pharmacy with an option to sort by name or price.
  * Implemented at `pharmacies/{pharmacyId}/masks` API.
* [x] List all pharmacies that offer a number of mask products within a given price range, where the count is above, below, or between given thresholds.
  * Implemented at `pharmacies/mask-count-by-price-range` API.
* [x] Show the top N users who spent the most on masks during a specific date range.
  * Implemented at `users/top-spenders` API.
* [x] Process a purchase where a user buys masks from multiple pharmacies at once.
  *  Implemented at `users/{userId}/purchase` API.
* [x] Update the stock quantity of an existing mask product by increasing or decreasing it.
  * Implemented at `masks/stock/batch` API.
* [x] Create or update multiple mask products for a pharmacy at once, including name, price, and stock quantity.
  * Implemented at `pharmacies/{pharmacyId}/masks/batch` API.
* [x] Search for pharmacies or masks by name and rank the results by relevance to the search term.
  * Implemented at `pharmacies-and-masks/search` API.

## API Document
[API Documentation](docs/ApiDocs.pdf)

## Test Coverage Report
Thank you for your request. Currently, test coverage is not implemented in the project, so I am unable to provide a report. 

## Deployment
Follow these steps to deploy the project locally using Docker:
### 1. Download the source code
Clone or download the repository to your local machine.

### 2. Open Command Prompt

### 3. Navigate to the project folder
Navigate to the folder containing `Dockerfile` and `docker-compose.yml`.  
For example, if your project is in `D:\test`:
```bash
cd D:\test\phantom_mask_bu2-main
```

### 4. Start the containers
Run Docker Compose to start both API and SQL Server containers:
```bash
docker-compose up -d
```

### 5. Check the running containers
```bash
docker ps
```
> Make sure that both containers — phantommask.api and phantommask-sqlserver — are running.

### 6. Import Data into the Database
Make sure the .NET SDK is installed (download from https://dotnet.microsoft.com/download if needed).
```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add Init --project D:\test\phantom_mask_bu2-main\PhantomMask.Api\PhantomMask.Api.csproj
dotnet ef database update --project D:\test\phantom_mask_bu2-main\PhantomMask.Api\PhantomMask.Api.csproj
dotnet run --project D:\test\phantom_mask_bu2-main\PhantomMask.Api\PhantomMask.Api.csproj import_data
```
> Do not run the following commands from inside the project folder.
You can run them from any directory, as long as the full project path is provided.

### 7. Open the Swagger UI
```bash
http://localhost:8080/swagger
```

### 8.Clean Up (Optional)
If you want to reset the environment, remove the migration files, drop the database, and stop the containers:
```bash
del D:\test\phantom_mask_bu2-main\PhantomMask.Api\Migrations\*.cs
dotnet ef database drop --project D:\test\phantom_mask_bu2-main\PhantomMask.Api\PhantomMask.Api.csproj --force

cd D:\test\phantom_mask_bu2-main
docker-compose down
```
> Before deleting migration files or dropping the database, make sure you are not inside the project folder (phantom_mask_bu2-main) to avoid file locks or access issues.
It's recommended to cd to another drive (e.g., C:\) before running the delete or drop commands.

## Additional Data
> If you have an ERD or any other materials that could help with understanding the system, please include them here.
