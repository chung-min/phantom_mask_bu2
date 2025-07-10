# Response
> The Current content is an **example template**; please edit it to fit your style and content.

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
I wrote down the xx unit tests for the APIs I built. Please check the test coverage report here.

You can run the test script by using the command below:

```bash
bundle exec rspec spec
```

## Deployment
* To deploy the project locally using Docker, run the following commands:
1. Download the source code
2. Open Command Prompt
```bash
# Navigate to the project folder containing Dockerfile and docker-compose.yml:
# If the project path is under D:\test
cd D:\test\phantom_mask_bu2-main
```
3. Start the containers (API and SQL Server) using Docker Compose
```bash
docker-compose up -d
```
4. Check the running containers
```bash
docker ps
```
> Make sure that both containers — phantom_mask_bu2-main-phant and phantommask-sqlserver — are running.
5. Import Data into the Database
```bash
# Run the following commands in order
# Check if .NET SDK is installed. Download and install the latest .NET SDK from https://dotnet.microsoft.com/download
dotnet tool install --global dotnet-ef
dotnet ef migrations add Init --project D:\test\phantom_mask_bu2-main\PhantomMask.Api\PhantomMask.Api.csproj
dotnet ef database update --project D:\test\phantom_mask_bu2-main\PhantomMask.Api\PhantomMask.Api.csproj
dotnet run --project D:\test\phantom_mask_bu2-main\PhantomMask.Api\PhantomMask.Api.csproj import_data
```
> Important: Do not run the following commands from inside the project folder.
You can run them from any directory, as long as the full project path is provided.
6. Open the Swagger UI
```bash
http://localhost:8080/swagger
```

## Additional Data
> If you have an ERD or any other materials that could help with understanding the system, please include them here.
