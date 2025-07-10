# Response

## Overview

This project is a backend API for PhantomMask, using ASP.NET Core and SQL Server running in Docker containers.

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/) installed and running
- [.NET SDK](https://dotnet.microsoft.com/download) installed (make sure `dotnet` command works)

## Requirement Completion Rate
* [x] List pharmacies, optionally filtered by specific time and/or day of the week.
  * Implemented at xxx API.
* [x] List all masks sold by a given pharmacy with an option to sort by name or price.
  * Implemented at xxx API.
* [x] List all pharmacies that offer a number of mask products within a given price range, where the count is above, below, or between given thresholds.
  * Implemented at xxx API.
* [x] Show the top N users who spent the most on masks during a specific date range.
  * Implemented at xxx API.
* [x] Process a purchase where a user buys masks from multiple pharmacies at once.
  *  Implemented at xxx API.
* [x] Update the stock quantity of an existing mask product by increasing or decreasing it.
  * Implemented at xxx API.
* [x] Create or update multiple mask products for a pharmacy at once, including name, price, and stock quantity.
  * Implemented at xxx API.
* [x] Search for pharmacies or masks by name and rank the results by relevance to the search term.
  * Implemented at xxx API.

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
> Make sure that both containers — phantom_mask_bu2-main-phant and phantommask-sqlserver — are running.

### 6. Import Data into the Database
Make sure the .NET SDK is installed (download from https://dotnet.microsoft.com/download if needed).
```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add Init --project D:\test\phantom_mask_bu2-main\PhantomMask.Api\PhantomMask.Api.csproj
dotnet ef database update --project D:\test\phantom_mask_bu2-main\PhantomMask.Api\PhantomMask.Api.csproj
dotnet run --project D:\test\phantom_mask_bu2-main\PhantomMask.Api\PhantomMask.Api.csproj import_data
```
> Important: Do not run the following commands from inside the project folder.
You can run them from any directory, as long as the full project path is provided.

### 7. Open the Swagger UI
```bash
http://localhost:8080/swagger
```

### 8.Stop
```bash
docker-compose down
```

## Additional Data
> If you have an ERD or any other materials that could help with understanding the system, please include them here.
