using System.Security.Claims;
using System.Text.Json.Serialization;

namespace App.Common.Models
{
	public class GetAccountsRequest : GetListBaseRequest
	{
		/// <summary>
		/// 搜尋類別
		/// </summary>
		public GetAccountsQueryType QueryType { get; set; }
	}

	public enum GetAccountsQueryType
	{
		/// <summary>
		/// 取得所有帳號
		/// </summary>
		ALL,
		/// <summary>
		/// 取得管理員帳號
		/// </summary>
		MANAGER,
		/// <summary>
		/// 取得用戶帳號
		/// </summary>
		USER
	}

	public class GetAccountsResponse : GetListBaseResponse<AccountViewModel>
	{
	}

	public class AccountViewModel
	{
		/// <summary>
		/// 用戶ID
		/// </summary>
		public string Id { get; set; } = null!;
		/// <summary>
		/// 用戶名稱
		/// </summary>
		public string UserName { get; set; } = null!;
		/// <summary>
		/// 用戶信箱
		/// </summary>
		public string Email { get; set; } = null!;
		/// <summary>
		/// 角色名稱列表
		/// </summary>
		public IEnumerable<string> RoleNames { get; set; } = new List<string>();
	}

	public class RegisterRequest
	{
		/// <summary>
		/// 用戶名稱
		/// </summary>
		public string UserName { get; set; } = null!;
		/// <summary>
		/// 用戶信箱
		/// </summary>
		public string Email { get; set; } = null!;
		/// <summary>
		/// 用戶密碼
		/// </summary>
		public string Password { get; set; } = null!;
	}

	public class UpdateAccountRequest
	{
		/// <summary>
		/// 用戶名稱
		/// </summary>
		public string? UserName { get; set; }
		/// <summary>
		/// 用戶信箱
		/// </summary>
		public string? Email { get; set; }
		/// <summary>
		/// 舊密碼
		/// </summary>
		public string? OldPassword { get; set; }
		/// <summary>
		/// 新密碼
		/// </summary>
		public string? NewPassword { get; set; }
		/// <summary>
		/// 用戶聲明列表
		/// </summary>
		[JsonIgnore]
		public IEnumerable<Claim>? Claims { get; set; }
	}

	public class LoginRequest
	{
		/// <summary>
		/// 用戶信箱
		/// </summary>
		public string Email { get; set; } = null!;
		/// <summary>
		/// 用戶密碼
		/// </summary>
		public string Password { get; set; } = null!;
	}

	public class LoginResponse
	{
		/// <summary>
		/// 錯誤訊息
		/// </summary>
		public string? Message { get; set; }
		/// <summary>
		/// 是否認證成功
		/// </summary>
		public bool IsAuthenticated { get; set; }
		/// <summary>
		/// 用戶名稱
		/// </summary>
		public string UserName { get; set; } = null!;
		/// <summary>
		/// 用戶信箱
		/// </summary>
		public string Email { get; set; } = null!;
		/// <summary>
		/// 用戶角色
		/// </summary>
		public List<string>? Roles { get; set; }
		/// <summary>
		/// Access Token
		/// </summary>
		public string? AccessToken { get; set; }
		/// <summary>
		/// Refresh Token
		/// </summary>
		public string? RefreshToken { get; set; }
		/// <summary>
		/// Refresh Token 過期時間
		/// </summary>
		public DateTime RefreshTokenExpiryTime { get; set; }
	}

	public class RefreshTokenRequest
	{
		/// <summary>
		/// Refresh Token
		/// </summary>
		public string RefreshToken { get; set; } = null!;
	}

	public class RefreshTokenResponse
	{
		/// <summary>
		/// Access Token
		/// </summary>
		public string AccessToken { get; set; } = null!;
		/// <summary>
		/// Refresh Token
		/// </summary>
		public string RefreshToken { get; set; } = null!;
	}
}
