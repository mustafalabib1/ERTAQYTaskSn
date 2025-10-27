using BLLProject;
using DALProject.DbInitializer;
using PLProject;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region Dependeny injection 
builder.Services.AddBLLDependencies()
    .AddPLDependances();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

#region Initialize Database

DbInitializer dbInitializer = new DbInitializer(dbConnectionString);
await dbInitializer.InitializeAsync();

#endregion

app.Run();
