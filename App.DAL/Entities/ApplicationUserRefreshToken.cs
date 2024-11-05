using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Entities
{
	/// <summary>
	/// 用戶 Refresh Token
	/// </summary>
	public class ApplicationUserRefreshToken
	{
		/// <summary>
		/// 編號
		/// </summary>
		public long Id { get; set; }
		/// <summary>
		/// Refresh Token
		/// </summary>
		public string Token { get; set; } = null!;
		/// <summary>
		/// Refresh Token 過期時間
		/// </summary>
		public DateTime ExpiryTime { get; set; }
		/// <summary>
		/// 用戶ID
		/// </summary>
		public string UserId { get; set; } = null!;
		/// <summary>
		/// 用戶
		/// </summary>
		public virtual ApplicationUser User { get; set; } = null!;
	}

	public class ApplicationUserRefreshTokenEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUserRefreshToken>
	{
		public void Configure(EntityTypeBuilder<ApplicationUserRefreshToken> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(e => e.Id).ValueGeneratedOnAdd();
			builder.HasIndex(x => x.Token);
			builder.Property(b => b.Token).HasMaxLength(88);
			builder
				.HasOne(e => e.User)
				.WithMany(e => e.RefreshTokens)
				.HasForeignKey(e => e.UserId);
		}
	}
}
