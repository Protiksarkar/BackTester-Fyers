using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//SQLLite configuration
builder.Services.AddDbContext<TradingDBContext>(options => options.UseSqlServer("Data Source=PSARKAR02\\SQLEXPRESS;Initial Catalog=TradingDB;Integrated Security=true;"));

// Auto Mapper Configurations
//var mapperConfig = new MapperConfiguration(mc =>
//{
//    mc.AddProfile(new MappingProfile());
//});
//IMapper mapper = mapperConfig.CreateMapper();


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Services.GetRequiredService<TradingDBContext>().Database.EnsureCreated();

app.Run();
