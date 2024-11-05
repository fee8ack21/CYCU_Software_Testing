using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.Entities
{
    /// <summary>
    /// 受保護的資源
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }

    public class ResourceEntityTypeConfiguration : IEntityTypeConfiguration<Resource>
    {
        public void Configure(EntityTypeBuilder<Resource> builder)
        {
            builder.Property(b => b.CreateTime).HasDefaultValueSql("NOW()");
            builder.Property(b => b.UpdateTime).HasDefaultValueSql("NOW()");
            builder.UseXminAsConcurrencyToken();
        }
    }
}
