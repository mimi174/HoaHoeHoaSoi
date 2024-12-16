using HoaHoeHoaSoi.Model;
using Microsoft.Extensions.Configuration;

namespace Webbanhoa;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddRazorPages();
        builder.Services.AddSession();
        builder.Services.Configure<MomoAPI>(builder.Configuration.GetSection("MomoAPI"));
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Error");
        }
        app.UseStaticFiles();

        app.UseRouting();

        app.UseSession(); 

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}
