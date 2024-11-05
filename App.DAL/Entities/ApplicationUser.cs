using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.Entities
{
	/// <summary>
	/// 帳號
	/// </summary>
	public class ApplicationUser : IdentityUser
	{
		/// <summary>
		/// 建立時間
		/// </summary>
		public DateTime CreateTime { get; set; }
		/// <summary>
		/// 更新時間
		/// </summary>
		public DateTime UpdateTime { get; set; }
		/// <summary>
		/// Refresh Tokens - 初始化避免無資料 null 錯誤
		/// </summary>
		public virtual ICollection<ApplicationUserRefreshToken> RefreshTokens { get; set; } = new List<ApplicationUserRefreshToken>();
		/// <summary>
		/// 帳號角色多對多映射
		/// </summary>
		public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
	}

	public class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
	{
		public void Configure(EntityTypeBuilder<ApplicationUser> builder)
		{
			builder.Property(b => b.CreateTime).HasDefaultValueSql("NOW()");
			builder.Property(b => b.UpdateTime).HasDefaultValueSql("NOW()");
			builder.UseXminAsConcurrencyToken();
		}
	}
}
