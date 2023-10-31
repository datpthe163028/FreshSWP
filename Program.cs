using Microsoft.EntityFrameworkCore;
using Project.Data;
using Microsoft.AspNetCore.Identity;
using Project.Service;
using Microsoft.AspNetCore.Identity.UI.Services;
using Org.BouncyCastle.Tls;
using WebApplication6.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddOptions();
var mailsetting = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailsetting);
builder.Services.AddSingleton<IEmailSender, SendMailService>();
var cloudsetting = builder.Configuration.GetSection("CloudinarySettings");
builder.Services.Configure<CloudinarySettings>(cloudsetting);
builder.Services.AddSingleton<ICloudinaryService, CloudinaryService>();


builder.Services.AddControllersWithViews();

var connect = builder.Configuration.GetConnectionString("ShopConnect");

builder.Services.AddDbContext<ShopContext>(options =>
{
    options.UseMySql(connect, ServerVersion.AutoDetect(connect));
});

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 6; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedAccount = true;
}).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ShopContext>()
    .AddErrorDescriber<CustomIdentityErrorDescriber>();

//builder.Services.AddSession();

var app = builder.Build();

//app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorPages();
app.UseRouting();
app.UseAuthentication(); ;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
