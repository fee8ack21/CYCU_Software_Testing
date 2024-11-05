using App.Common.Models;

namespace App.BLL.Account
{
    public interface IAccountService
    {
        Task<GetAccountsResponse> GetAccountsAsync(GetAccountsRequest request);
        Task<AccountViewModel> RegisterAsync(RegisterRequest request);
        Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}
