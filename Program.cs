using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Proje4.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Proje4Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Proje4Context") ?? throw new InvalidOperationException("Connection string 'Proje4Context' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
