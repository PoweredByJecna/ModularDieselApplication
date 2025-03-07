using Microsoft.EntityFrameworkCore;
using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Infrastructure.Persistence;
using ModularDieselApplication.Infrastructure.Repositories;
using ModularDieselApplication.Infrastructure.Mappings;
using ModularDieselApplication.Interfaces.Repositories;
using ModularDieselApplication.Application.Services;
using ModularDieselApplication.Application.MIddleware;
using Microsoft.AspNetCore.Identity;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Infrastructure.Persistence.Repositories;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;
using ModularDieselApplication.Application.Services.DieslovaniServices.DieslovaniActionService;
using ModularDieselApplication.Application.Services.DieslovaniServices.DieslovaniAssignmentService;
using ModularDieselApplication.Application.Services.DieslovaniServices.DieslovaniQueryService;
using AutoMapper;
using ModularDieselApplication.Domain.Rules;
using ModularDieselApplication.Infrastructure.CleaningDatabase;
using ModularDieselApplication.Domain.Entities;
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
 ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));




// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.ConfigureApplicationCookie(Options =>
{
    Options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

});

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddIdentity<TableUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;

})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IDieslovaniRepository, DieslovaniRepository>();
builder.Services.AddScoped<IOdstavkyRepository, OdstavkyRepository>();
builder.Services.AddScoped<IPohotovostiRepository, PohotovostiRepository>();
builder.Services.AddScoped<IRegionyRepository, RegionyRepository>();
builder.Services.AddScoped<ITechniciRepository, TechniciRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILokalityRepository, LokalityRepository>();
builder.Services.AddScoped<ILogServiceRepository, LogRepository>();

// Register services
builder.Services.AddScoped<IDieslovaniService, DieslovaniService>();
builder.Services.AddScoped<IOdstavkyService, OdstavkyService>();
builder.Services.AddScoped<ILokalityService, LokalityService>();
builder.Services.AddScoped<IPohotovostiService, PohotovostiService>();
builder.Services.AddScoped<IRegionyService, RegionyService>();
builder.Services.AddScoped<ITechnikService, TechnikService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ILogService, LogService>();



builder.Services.AddScoped<DieslovaniRules>();
builder.Services.AddScoped<DieslovaniActionService>();
builder.Services.AddScoped<DieslovaniAssignmentService>();
builder.Services.AddScoped<DieslovaniQueryService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<SignInManager<TableUser>>();
builder.Services.AddScoped<UserManager<TableUser>>();
builder.Services.AddScoped<OdstavkaAssignmentService>();
builder.Services.AddScoped<OdstavkyActionService>();
builder.Services.AddScoped<OdstavkyQueryService>();
builder.Services.AddScoped<OdstavkyRules>();

builder.Services.AddScoped<IDatabaseCleaner, DatabaseCleaner>();
builder.Services.AddHostedService<CleaningDatabaseService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
    mapper.ConfigurationProvider.AssertConfigurationIsValid();
}
app.UseRouting();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RedirectToLoginMiddleware>();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dieslovani}/{action=Index}/{id?}");
app.Run();

