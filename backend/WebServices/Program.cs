using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebServices.DataAccess;
using WebServices.Repositories.Analytics;
using WebServices.SharedBusiness;
using WebServices.SharedBusiness.Analytics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddScoped<PatientProcess>();
builder.Services.AddScoped<InvoiceProcess>();
builder.Services.AddScoped<InsuranceProcess>();
builder.Services.AddScoped<AuthenticationProcess>();
builder.Services.AddScoped<ScheduleProcess>();
builder.Services.AddScoped<WebServices.Repositories.AppointmentsRepository>();
builder.Services.AddScoped<EncounterProcess>();

builder.Services.AddScoped<IReportsRepository, ReportsRepository>();
builder.Services.AddScoped<IReportsProcess, ReportsProcess>();

builder.Services.AddScoped<WebServices.Repositories.BillingRepository>();

// Adding Analytics services
builder.Services.AddScoped<WebServices.Repositories.Analytics.AnalyticsRepository>();
builder.Services.AddScoped<WebServices.SharedBusiness.Analytics.AnalyticsProcess>();

// Adding in-memory for temporary caching needs. In this case used for storing JTI values for token revocation. This can be replaced with a persistent caching solution if needed in the future.
builder.Services.AddMemoryCache();

// Configuring CORS to allow requests from Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configuring DBContext for SQL Server
var connectionString = builder.Configuration.GetConnectionString("PMSDbConn");
 
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(connectionString));

// Configuring JWT Authentication
var jwtKey = builder.Configuration.GetValue<string>("Jwt:Key");
var issuer = builder.Configuration.GetValue<string>("Jwt:Issuer");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();
// Commenting out HTTPS redirections to allow temporary HTTP access for Angular testing. This should be removed in production for security reasons.

app.UseRouting();

// Enabling CORS for Angular frontend
app.UseCors("AllowAngular");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
