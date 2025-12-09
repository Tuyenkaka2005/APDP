using Microsoft.EntityFrameworkCore;
using SIMS.Data;

namespace SIMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Đăng ký DbContext
            builder.Services.AddDbContext<SIMSContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Đăng ký Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Register Singleton Service
            builder.Services.AddSingleton<SIMS.Services.ISchoolInfoService, SIMS.Services.SchoolInfoService>();

            // Register Adapter Pattern
            builder.Services.AddScoped<SIMS.Patterns.Adapter.IExternalGradeSystem, SIMS.Patterns.Adapter.GradeAdapter>();

            // Register Factory Pattern
            builder.Services.AddScoped<SIMS.Patterns.Factory.IViewModelFactory, SIMS.Patterns.Factory.StudentViewModelFactory>();

            // Register Facade Pattern
            builder.Services.AddScoped<SIMS.Patterns.Facade.IStudentPortalFacade, SIMS.Patterns.Facade.StudentPortalFacade>();

            var app = builder.Build();

            // Test kết nối database (ĐẶT TRƯỚC app.Run())
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var db = scope.ServiceProvider.GetRequiredService<SIMSContext>();
                    var count = db.Users.Count();
                    Console.WriteLine($"✅ Kết nối database thành công! Có {count} người dùng trong CSDL.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Lỗi kết nối database: {ex.Message}");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession(); // Thêm middleware Session

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"); // Default route: Home/Index

            app.Run();
        }
    }
}