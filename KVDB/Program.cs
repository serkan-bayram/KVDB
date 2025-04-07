using Microsoft.EntityFrameworkCore;
using KVDB.Data;
using DotNetEnv;
using KVDB.Controllers;

var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

var envFile = environment == "Production" ? "./.env" : "./.env.development";
Env.TraversePath().Load(envFile);

var connectionString = Env.GetString("ConnectionString") ?? throw new InvalidOperationException("Connection string 'KVDBContext' not found.");

builder.Services.AddDbContext<KVDBContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add ActionsController to the services
builder.Services.AddScoped<ActionsController>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<KVDBContext>();
    dbContext.Database.Migrate();
}

// Call HandleFiles on app startup
using (var scope = app.Services.CreateScope())
{
    var actionsController = scope.ServiceProvider.GetRequiredService<ActionsController>();
    await actionsController.HandleFiles();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
