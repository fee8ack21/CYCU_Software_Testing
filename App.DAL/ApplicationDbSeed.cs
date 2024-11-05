using App.DAL.Entities;
using App.DAL.Types;

namespace App.DAL
{
    public class ApplicationDbSeed
    {
        /// <summary>
        /// 預設用戶角色
        /// </summary>
        public static IEnumerable<ApplicationRole> ApplicationRoles => new[]
        {
            new ApplicationRole { Name = UserRole.SystemManager, IsSystemDefault = true, IsManager = true },
            new ApplicationRole { Name = UserRole.GeneralUser, IsSystemDefault = true, IsManager = false }
        };

        /// <summary>
        /// 預設用戶
        /// </summary>
        public static IEnumerable<(ApplicationUser User, string[] Roles)> ApplicationUsers => new[]
        {
            (new ApplicationUser { Email = "system_manager@test.com", UserName = "SystemManager", EmailConfirmed = true }, new[] { UserRole.SystemManager }),
            (new ApplicationUser { Email = "general_user@test.com", UserName = "GeneralUser", EmailConfirmed = true }, new[] { UserRole.GeneralUser }),
        };

        /// <summary>
        /// 預設受保護的資源
        /// </summary>
        public static IEnumerable<App.DAL.Entities.Resource> Resources => new[]
        {
            new Resource {Name = "受保護的資源一"},
            new Resource {Name = "受保護的資源二"},
            new Resource {Name = "受保護的資源三Ｆ"}
        };
    }
}
