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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
