using App.PL.Others;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using App.DAL;
using Medallion.Threading.Postgres;
using Medallion.Threading;
using Microsoft.EntityFrameworkCore;
using App.Common.Configs;
using App.Common.Others;
using App.DAL.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Rewrite;
using Serilog;
using App.BLL.Account;
using App.DAL.Types;
using App.BLL.Resource;

namespace App.PL
{
	public static class DependencyInjection
	{
		#region WebApplicationBuilder
		/// <summary>
		/// 注入 appsettings 內容並轉映至 config model
		/// </summary>
		/// <param name="builder"></param>
		public static JWTConfig ConfigureConfigs(this WebApplicationBuilder builder)
		{
			var jwtSection = builder.Configuration.GetSection("JWT");
			var JWTConfig = jwtSection.Get<JWTConfig>()!;
			builder.Services.Configure<JWTConfig>(jwtSection);

			return JWTConfig;
		}

		/// <summary>
		/// 加入 swagger 內容
		/// </summary>
		/// <param name="services"></param>
		public static void AddSwagger(this IServiceCollection services)
		{
			// https://learn.microsoft.com/zh-tw/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-6.0&tabs=visual-studio
			services.AddApiVersioning(opt =>
			{
				// 默認版本
				opt.DefaultApiVersion = new ApiVersion(1, 0);
				// 如果沒有聲明就使用默認版本
				opt.AssumeDefaultVersionWhenUnspecified = true;
				// 在 header 返回支持的版本
				opt.ReportApiVersions = true;
				opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(), new HeaderApiVersionReader("x-api-version"), new MediaTypeApiVersionReader("x-api-version"));
			});

			services.AddVersionedApiExplorer(setup =>
			{
				setup.GroupNameFormat = "'v'VVV";
				setup.SubstituteApiVersionInUrl = true;
			});

			services.AddEndpointsApiExplorer();
			services.ConfigureOptions<SwaggerOptions>();
			services.AddSwaggerGen();

			services.TryAddEnumerable(ServiceDescriptor.Transient<IApplicationModelProvider, APIResponseModelProviders>());
		}

		/// <summary>
		/// 注入實作層
		/// </summary>
		/// <param name="services"></param>
		public static void AddServices(this IServiceCollection services)
		{
			services.AddScoped<IAccountService, AccountService>();
			services.AddScoped<IResourceService, ResourceService>();
		}

		public static void ConfigureDatabase(this WebApplicationBuilder builder)
		{
			// 連線字串
			var connectionString = builder.Configuration.GetConnectionString("CYCUSoftwareTesting")!;

			// Database
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
			{
				// LogTo - 紀錄 Query 
				options.UseNpgsql(connectionString).LogTo(Log.Logger.Information, LogLevel.Information, null);

				if (builder.Environment.IsDevelopment())
				{
					// 開發環境紀錄 Query 參數
					options.EnableSensitiveDataLogging();
				}
			});

			// Distributed Lock Using Postgres
			builder.Services.AddSingleton<IDistributedLockProvider>(_ => new PostgresDistributedSynchronizationProvider(connectionString));
		}

		public static void ConfigureIdentity(this IServiceCollection services, JWTConfig JWTConfig)
		{
			// 為了 UserManager 以下方法，加入 AddTokenProvider()
			// 1.GenerateEmailConfirmationTokenAsync(user)
			// 2.GeneratePasswordResetTokenAsync(user)
			services.AddIdentity<ApplicationUser, ApplicationRole>()
				.AddErrorDescriber<LocalizedIdentityErrorDescriber>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);
			services.Configure<DataProtectionTokenProviderOptions>(o =>
			{
				// 取得或設定所產生權杖保持有效的時間量。 預設為 1 天。
				o.TokenLifespan = TimeSpan.FromHours(1);
			});

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.SaveToken = true;
				options.RequireHttpsMetadata = false;
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ClockSkew = TimeSpan.Zero,
					ValidAudience = JWTConfig.Audience,
					ValidIssuer = JWTConfig.Issuer,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTConfig.Key))
				};
			});

			services.AddAuthorization(o =>
			{
				o.AddPolicy("Admin", p => p.RequireRole(UserRole.SystemManager));
			});

			services.Configure<IdentityOptions>(options =>
			{
				// https://learn.microsoft.com/zh-tw/dotnet/api/microsoft.aspnetcore.identity.passwordoptions?view=aspnetcore-6.0
				// 密碼中需要介於 0-9 之間的數位。
				options.Password.RequireDigit = true;
				// 密碼中需要小寫字元。
				options.Password.RequireLowercase = true;
				// 密碼中需要非英數字元。
				options.Password.RequireNonAlphanumeric = true;
				// 密碼中需要大寫字元。
				options.Password.RequireUppercase = true;
				// 密碼長度下限。
				options.Password.RequiredLength = 6;
				// 需要密碼中的相異字元數。
				options.Password.RequiredUniqueChars = 1;

				// https://learn.microsoft.com/zh-tw/dotnet/api/microsoft.aspnetcore.identity.lockoutoptions?view=aspnetcore-6.0
				// 發生鎖定時，使用者遭到鎖定的時長。
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				// 使用者遭到鎖定之前允許的存取嘗試失敗次數上限 (如果已啟用鎖定)。
				options.Lockout.MaxFailedAccessAttempts = 5;
				// 判斷是否可以鎖定新的使用者。
				options.Lockout.AllowedForNewUsers = true;

				// https://learn.microsoft.com/zh-tw/dotnet/api/microsoft.aspnetcore.identity.useroptions?view=aspnetcore-6.0
				// 要求每個使用者都有唯一的電子郵件。
				options.User.RequireUniqueEmail = true;
				// 使用者名稱中允許的字元。
				options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

				// https://learn.microsoft.com/zh-tw/dotnet/api/microsoft.aspnetcore.identity.signinoptions?view=aspnetcore-6.0
				// 需要確認的電子郵件才能登入。
				//options.SignIn.RequireConfirmedEmail = true;
			});
		}
		#endregion

		#region WebApplication
		public static void InitDatabaseAndCache(this WebApplication app)
		{
			using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				ApplicationDbInitializer.InitializeAsync(scope).Wait();
			}
		}

		public static void UseSwaggerService(this WebApplication app)
		{
			var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
				{
					options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
				}
			});

			// 將根路徑導至 swagger
			app.UseRewriter(new RewriteOptions().AddRedirect("^$", "swagger"));
		}
		#endregion
	}
}
