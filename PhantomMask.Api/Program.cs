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


// �p�G�ҰʮɶǤJ "import_data" �ѼơA�N�]�@���פJ�{���A�M�ᵲ��
if (args.Length > 0 && args[0].Equals("import_data", StringComparison.OrdinalIgnoreCase))
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<PhantomMaskDbContext>();
    var etl = new Processor(context);

    // �A�i�H�ǲĤG�ӰѼƫ��w�ɮ׸��|�A�o��²��ܽd�g�����|
    await context.Database.MigrateAsync();
    await etl.LoadPharmaciesAsync("RawData/pharmacies.json");
    await etl.LoadUsersAsync("RawData/users.json");

    Console.WriteLine("Import finished.");
    return;  // ���������{���A���Ұ� API
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
