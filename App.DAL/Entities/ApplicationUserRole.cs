using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Entities
{
	/// <summary>
	/// 帳號角色多對多映射
	/// </summary>
	public class ApplicationUserRole : IdentityUserRole<string>
	{
		public virtual ApplicationUser User { get; set; } = null!;
		public virtual ApplicationRole Role { get; set; } = null!;
	}

	public class ApplicationUserRoleEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
	{
		public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
		{
			builder.HasOne(e => e.Role)
				.WithMany(e => e.UserRoles)
				.HasForeignKey(ur => ur.RoleId)
				.IsRequired();

			builder.HasOne(e => e.User)
				.WithMany(e => e.UserRoles)
				.HasForeignKey(e => e.UserId)
				.IsRequired();
		}
	}
}
