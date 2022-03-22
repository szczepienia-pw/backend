using backend.Database;
using backend.Helpers;
using backend.Middlewares;
using backend.Services.Patient;
using backend.Services.Doctor;
using backend.Services.Admin;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Singletons
builder.Services.AddSingleton<JwtGenerator>();
builder.Services.AddSingleton<Seeder>();
builder.Services.AddSingleton<SecurePasswordHasher>();

builder.Services.AddScoped<PatientAuthService>();
builder.Services.AddScoped<DoctorAuthService>();
builder.Services.AddScoped<AdminAuthService>();
builder.Services.AddScoped<VaccinationSlotService>();
builder.Services.AddScoped<SettingService>();

// Connect Jwt settings to section in appsettings.json
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<HasherSettings>(builder.Configuration.GetSection("HasherSettings"));

// Connect to MySQL database
string connectionString = builder.Configuration.GetConnectionString("MySQLConnection").ToString();
ServerVersion sv = MariaDbServerVersion.AutoDetect(connectionString);
builder.Services.AddDbContext<DataContext>(options => options.UseMySql(connectionString, sv));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();
