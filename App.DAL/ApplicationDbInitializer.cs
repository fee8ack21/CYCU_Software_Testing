using App.DAL.Entities;
using Medallion.Threading;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App.DAL
{
	public class ApplicationDbInitializer
	{
		/// <summary>
		/// 初始化資料庫
		/// </summary>
		public async static Task InitializeAsync(IServiceScope scope)
		{
			var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbInitializer>>();
			var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var lockProvider = scope.ServiceProvider.GetRequiredService<IDistributedLockProvider>();
			var @lock = lockProvider.CreateLock("DB_INIT");

			logger.LogInformation("Starting database initialization.");
			try
			{
				// 因資料庫未建立，尚未能使用 lock
				await MigrateAsync(context);

				// 到此，可以確信資料庫已建立
				using (await @lock.AcquireAsync())
				{
					await InitApplicationRolesAsync(scope);
					await InitApplicationUsersAsync(scope);
					await InitResourcesAsync(context);
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Database initialization failed.");
			}
			logger.LogInformation("Ending database initialization.");
		}

		/// <summary>
		/// 更新資料庫版本 
		/// </summary>
		private async static Task MigrateAsync(ApplicationDbContext context)
		{
			int count = 10;
			Exception? exception = null;

			while (count > 0)
			{
				try
				{
					await context.Database.MigrateAsync();
					exception = null;
					break;
				}
				catch (Exception ex)
				{
					count--;
					exception = ex;
					await Task.Delay(1000);
				}
			}

			if (exception != null)
			{
				throw exception;
			}
		}

		/// <summary>
		/// 初始化 Identity Roles
		/// </summary>
		private static async Task InitApplicationRolesAsync(IServiceScope scope)
		{
			var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
			var count = await roleManager.Roles.CountAsync();
			if (count > 0)
			{
				return;
			}

			foreach (var role in ApplicationDbSeed.ApplicationRoles)
			{
				await roleManager.CreateAsync(role);
			}
		}

		/// <summary>
		/// 初始化 Identity Users
		/// </summary>
		private static async Task InitApplicationUsersAsync(IServiceScope scope)
		{
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var count = await userManager.Users.CountAsync();
			if (count > 0)
			{
				return;
			}

			foreach (var (user, roles) in ApplicationDbSeed.ApplicationUsers)
			{
				var result = await userManager.CreateAsync(user, "Aa123456!");
				if (!result.Succeeded)
				{
					continue;
				}

				await userManager.AddToRolesAsync(user, roles);
			}
		}

		/// <summary>
		/// 初始化受保護的資源
		/// </summary>
		private static async Task InitResourcesAsync(ApplicationDbContext context)
		{

			if (await context.Resources.AnyAsync())
			{
				return;
			}

			context.Resources.AddRange(ApplicationDbSeed.Resources);
			await context.SaveChangesAsync();
		}
	}
}
