namespace App.Common.Configs
{
	/// <summary>
	/// JWT 設定
	/// </summary>
	public class JWTConfig
	{
		public string Key { get; set; } = null!;
		public string Issuer { get; set; } = null!;
		public string Audience { get; set; } = null!;
		public double AccessTokenDurationInMinutes { get; set; }
		public int RefreshTokenValidityInDays { get; set; }
	}
}
