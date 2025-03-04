using Microsoft.EntityFrameworkCore;
using KVDB.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<KVDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("KVDBContext") ?? throw new InvalidOperationException("Connection string 'KVDBContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseHttpsRedirection();

app.UseStaticFiles();
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//        Path.Combine(Directory.GetCurrentDirectory(), "PythonScripts", "files")),

//    RequestPath = "/videos"
//});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
