using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.Entities
{
	/// <summary>
	/// 帳號角色
	/// </summary>
	public class ApplicationRole : IdentityRole
	{
		/// <summary>
		/// 是否為系統預設
		/// </summary>
		public bool IsSystemDefault { get; set; }
		/// <summary>
		/// 是否為管理員帳號
		/// </summary>
		public bool IsManager { get; set; }
		/// <summary>
		/// 建立時間
		/// </summary>
		public DateTime CreateTime { get; set; }
		/// <summary>
		/// 帳號角色多對多映射
		/// </summary>
		public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
	}

	public class ApplicationRoleEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationRole>
	{
		public void Configure(EntityTypeBuilder<ApplicationRole> builder)
		{
			builder.Property(b => b.CreateTime).HasDefaultValueSql("NOW()");
			builder.UseXminAsConcurrencyToken();
		}
	}
}
