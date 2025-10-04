using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using BusReservation.API.Data;
using BusReservation.API.Models;
using BusReservation.API.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



builder.Services.Configure<BusReservation.API.Services.EmailSettings>(
    builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<BusReservation.API.Services.IEmailSender,
    BusReservation.API.Services.MailKitEmailSender>();



// ... existing services
builder.Services.AddSignalR();

// ---------- EF Core (SQL Server) ----------
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
// options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// var app = builder.Build(); // Removed duplicate, only build once above

// Using Local Storage 
// ✅ Replace SQL with InMemory
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("BusDb");
});

// ✅ CORS so React (http://localhost:3000) can call API
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowLocal",
        p => p.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});


// ✅ Ensure database exists and seed
// using (var scope = app.Services.CreateScope())
// {
//    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
   // db.Database.EnsureCreated();   // creates in-memory store
   // SeedData.Run(db);              // (Step 3) – seed some sample rows
// }

// ---------- Identity (users only, roles optional) ----------
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ---------- JWT Auth ----------
var jwtSection = builder.Configuration.GetSection("Jwt");
var issuer = jwtSection["Issuer"] ?? "yourapp";
var audience = jwtSection["Audience"] ?? "yourapp";
var keyBytes = Encoding.UTF8.GetBytes(jwtSection["Key"] ?? "REPLACE_WITH_LONG_RANDOM_SECRET_KEY_AT_LEAST_32_CHARS");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateLifetime = true
    };
});

// ---------- MVC / Swagger ----------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ Ensure database exists and seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();   // creates in-memory store
    SeedData.Run(db);              // (Step 3) – seed some sample rows
}

// ... existing middleware
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ... existing pipeline
app.MapHub<SeatsHub>("/hubs/seats");
app.Run();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocal");

app.Run();
