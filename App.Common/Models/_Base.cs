using System.Text.Json.Serialization;

namespace App.Common.Models
{
	public abstract class GetListBaseRequest
	{
		/// <summary>
		/// 當前頁數(預設為第 1 頁)
		/// </summary>
		public int Page { get; set; } = 1;
		private int _rowsPerPage = 15;
		/// <summary>
		/// 每頁幾筆(預設為 15 筆)
		/// </summary>
		public int RowsPerPage
		{
			get
			{
				return Math.Min(1000, _rowsPerPage);
			}
			set
			{
				_rowsPerPage = value;
			}
		}
		/// <summary>
		/// 開始筆數
		/// </summary>
		[JsonIgnore] // 隱藏 Model 欄位不顯示在 Swagger 上
		public int Start
		{
			get
			{
				return (Page - 1) * RowsPerPage;
			}
		}
	}

	public class GetListBaseResponse<T>
	{
		/// <summary>
		/// 資料筆數
		/// </summary>
		public int Count { get; set; }
		/// <summary>
		/// 資料
		/// </summary>
		public IEnumerable<T>? Data { get; set; }
	}

	public static class ClaimType
	{
		/// <summary>
		/// 用戶角色
		/// </summary>
		public const string Roles = "roles";
		/// <summary>
		/// 用戶權限
		/// </summary>
		public const string Permissions = "permissions";
		/// <summary>
		/// 用戶編號
		/// </summary>
		public const string UId = "uid";
	}
}
