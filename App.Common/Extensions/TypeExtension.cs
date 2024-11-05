namespace App.Common.Extensions
{
	public static class TypeExtension
	{
		/// <summary>
		/// 是否為 BLL 實作 service
		/// </summary>
		public static bool IsServiceType(this Type type) =>
			!type.IsAbstract
			&& !type.IsInterface
			&& type.BaseType != null
			&& type.GetInterfaces().Any()
			&& type.Name.EndsWith("Service");
	}
}
