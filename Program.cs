using Microsoft.EntityFrameworkCore;
using NewPointWebApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
#pragma warning disable CS8321 
static void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
	optionsBuilder.UseSqlServer("defaultConnection")
				  .LogTo(Console.WriteLine, LogLevel.Information);
 
}

#pragma warning restore CS8321 
builder.Services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddSwaggerServices();
// builder.Services.AddIdentityServices(builder.Configuration); 
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowFrontend");
app.UseCors(cors => cors
	.AllowAnyHeader()
	.AllowAnyMethod()
	.WithOrigins("http://localhost:3000", "http://localhost:5173")
	// .WithOrigins("http://162.216.112.217:80")
	.AllowCredentials());
app.MapControllers();
app.Run();
