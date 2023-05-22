using AutoMapper;
using GetStockChartData;
using TestingConsole.DBServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services
    .AddDbContextFactory<TradingDBContext>()
    .AddSingleton<TradingRepo>()
    .AddSingleton<CommonService>()
    .AddSingleton<IMapper>(new MapperConfiguration(mc => mc.AddProfile(new MappingProfile())).CreateMapper());

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(builder => builder
   .AllowAnyOrigin()
   .AllowAnyMethod()
   .AllowAnyHeader());

app.UseAuthorization();

app.UseSwagger();

app.UseSwaggerUI();

app.MapControllers();

app.Run();
