using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using backend.Database;
using backend.Middlewares;
using backend.Services;
using backend.Services.Admin;
using backend.Services.Doctor;
using backend.Services.Patient;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

namespace backend.Helpers;

// only for the purpose of excluding Program.cs from test coverage
[ExcludeFromCodeCoverage]
public class ProgramHelper
{
    public static void Run(WebApplicationBuilder? builder)
    {
        // Add services to the container.

        builder.Services.AddControllers().AddFluentValidation(options =>
        {
            options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Singletons
        builder.Services.AddSingleton<JwtGenerator>();
        builder.Services.AddSingleton<Seeder>();
        builder.Services.AddSingleton<SecurePasswordHasher>();
        builder.Services.AddSingleton<Mailer>();

        builder.Services.AddScoped<AdminAuthService>();
        builder.Services.AddScoped<AdminDoctorsService>();
        builder.Services.AddScoped<AdminPatientsService>();

        builder.Services.AddScoped<PatientAuthService>();
        builder.Services.AddScoped<PatientService>();

        builder.Services.AddScoped<DoctorAuthService>();

        builder.Services.AddScoped<VaccinationSlotService>();
        builder.Services.AddScoped<SettingService>();
        builder.Services.AddScoped<BugService>();
        builder.Services.AddScoped<AdminDoctorsService>();
        builder.Services.AddScoped<PatientService>();
        builder.Services.AddScoped<VaccinationService>();
        builder.Services.AddScoped<CommonVaccinationService>();

        // Connect settings to sections in appsettings.json
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
        builder.Services.Configure<HasherSettings>(builder.Configuration.GetSection("HasherSettings"));
        builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
        builder.Services.Configure<FrontendUrlsSettings>(builder.Configuration.GetSection("FrontendUrls"));

        // Connect to MySQL database
        string connectionString = builder.Configuration.GetConnectionString("MySQLConnection").ToString();
        ServerVersion sv = MariaDbServerVersion.AutoDetect(connectionString);
        builder.Services.AddDbContext<DataContext>(options => { options.UseMySql(connectionString, sv); options.UseLazyLoadingProxies(); });

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
    }
}