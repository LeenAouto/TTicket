using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using TTicket.Abstractions.DAL;
using TTicket.Abstractions.Security;
using TTicket.DAL;
using TTicket.DAL.Managers;
using TTicket.Security;
using TTicket.Security.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Adding Health Check
builder.Services.AddHealthChecks().AddCheck<DbHealthCheck>("Database");

//Adding Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession( options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15);
});

//Adding JWT
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.SaveToken = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManagerPolicy", p =>
        p.RequireClaim("TypeUser", "1")
    );

    options.AddPolicy("SupportPolicy", p =>
        p.RequireClaim("TypeUser", "2")
    );

    options.AddPolicy("ClientPolicy", p =>
        p.RequireClaim("TypeUser", "3")
    );
});

//Adding Serilog
Log.Logger = new LoggerConfiguration().
    ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

//Adding CORS
builder.Services.AddCors();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuthManager, AuthManager>();

builder.Services.AddScoped<IUserManager, UserManager>();
builder.Services.AddScoped<ITicketManager, TicketManager>();
builder.Services.AddScoped<ICommentManager, CommentManager>();
builder.Services.AddScoped<IAttachmentManager, AttachmentManager>();
builder.Services.AddScoped<IProductManager, ProductManager>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

//Enabling staticFiles
app.UseStaticFiles();

//Adding CORS
app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

//Adding Health Check
app.MapHealthChecks("/_health");

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllers();

app.Run();
