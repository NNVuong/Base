using System.Text.Encodings.Web;
using System.Text.Unicode;
using DataBase.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using MVC;
using MVC.Base;
using Repositories.Implements;
using Repositories.Interfaces;
using Services.Implements;
using Services.Interfaces;
using Utilities.Constants;

var builder = WebApplication.CreateBuilder(args);
var prefixUrl = builder.Environment.IsDevelopment() ? builder.Configuration.GetConnectionString("Prefix") : "";

// Khai báo các dịch vụ cần thiết cho ứng dụng
builder.Services.AddControllersWithViews();

// Cấu hình Unicode cho HTML Encoder
builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

// Thêm hỗ trợ cho Sessions
builder.Services.AddSession();

// Thêm cơ sở dữ liệu
builder.Services.AddDbContextPool<MfrDbContext>((_, options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection"),
        optionsBuilder => optionsBuilder.MigrationsAssembly("MVC"));
    options.UseModel(MfrDbContextModel.Instance);
});

// Đăng ký các dịch vụ
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(ICoreServices<,,>), typeof(CoreServices<,,>));

// Cấu hình xác thực và FEID
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "mfr_cookie";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.LoginPath = "/" + prefixUrl + $"{DefaultValue.LoginPath}";
        options.AccessDeniedPath = "/" + prefixUrl + $"{DefaultValue.AccessDeniedPath}";
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.Authority = "https://feid.ptudev.net";
        options.ClientId = "mfr_client";
        options.ClientSecret = "Tm3#Ia1$Mw2$Ag3!Jv4$Hx4@Ax7&Oh4*";
        options.ResponseType = "code";
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.Scope.Add("identity-service");
        options.SaveTokens = true;
        options.RequireHttpsMetadata = true;
    });

// Cấu hình các chính sách
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyType.AdministratorArea,
        policyBuilder => policyBuilder.RequireAssertion(context =>
            context.User.IsInRole(UserRoleType.Administrator) ||
            context.User.IsInRole(UserRoleType.SuperAdministrator)
        )
    );

    options.AddPolicy(PolicyType.DataEntryArea,
        policyBuilder => policyBuilder.RequireAssertion(context =>
            context.User.HasClaim(claim =>
                claim.Type is UserClaimTypes.CampusCode or UserClaimTypes.CampusName &&
                !string.IsNullOrEmpty(claim.Value)) &&
            (context.User.IsInRole(UserRoleType.Administrator) ||
             context.User.IsInRole(UserRoleType.SuperAdministrator))
        )
    );

    options.AddPolicy(PolicyType.IdentityArea,
        policyBuilder =>
            policyBuilder.RequireAssertion(context => context.User.IsInRole(UserRoleType.SuperAdministrator)
            )
    );
});

// Thêm AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Cấu hình chống Cross-Site Request Forgery (CSRF)
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");

var app = builder.Build();

// Xử lý ngoại lệ
app.UseExceptionHandler("/" + prefixUrl + $"{DefaultValue.ErrorPath}");

app.UseHsts();

app.UseHttpsRedirection();

app.UseStaticFiles();

// Sử dụng Sessions
app.UseSession();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// Sử dụng chống CSRF
app.UseAntiforgery();

// Định tuyến cho Controller
app.MapControllerRoute("areas", prefixUrl + "{area=" + DefaultValue.Area +
                                "}/{controller=" + DefaultValue.Controller +
                                "}/{action=" + DefaultValue.Action +
                                "}/{id?}");

// Khởi chạy ứng dụng
app.Run();