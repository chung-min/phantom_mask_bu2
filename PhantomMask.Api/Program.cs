using Microsoft.EntityFrameworkCore;
using PhantomMask.Api.Data;
using PhantomMask.Api.Data.Initializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<PhantomMaskDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

////Configure the HTTP request pipeline.
//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<PhantomMaskDbContext>();
//    context.Database.EnsureCreated();

//    var etl = new Processor(context);
//    await etl.LoadPharmaciesAsync("RawData/pharmacies.json");
//    await etl.LoadUsersAsync("RawData/users.json");
//}


// 如果啟動時傳入 "import_data" 參數，就跑一次匯入程式，然後結束
if (args.Length > 0 && args[0].Equals("import_data", StringComparison.OrdinalIgnoreCase))
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<PhantomMaskDbContext>();
    var etl = new Processor(context);

    // 你可以傳第二個參數指定檔案路徑，這裡簡單示範寫死路徑
    await context.Database.MigrateAsync();
    await etl.LoadPharmaciesAsync("RawData/pharmacies.json");
    await etl.LoadUsersAsync("RawData/users.json");

    Console.WriteLine("Import finished.");
    return;  // 直接結束程式，不啟動 API
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
