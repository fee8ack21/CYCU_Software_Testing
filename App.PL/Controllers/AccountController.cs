using App.BLL.Account;
using App.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace App.PL.Controllers
{
    /// <summary>
    /// 帳號
    /// </summary>

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        /// <summary>
        /// 取得帳號列表
        /// </summary>
        /// <remarks>僅限系統管理員身份呼叫</remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize("Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(GetAccountsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GetAccountsResponse>> GetAccounts([FromQuery] GetAccountsRequest request)
        {
            var response = await _accountService.GetAccountsAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// 註冊
        /// </summary>
        /// <remarks>
        /// 密碼規則：需包含數字/小寫英文/大寫英文/非英數字元及至少 6 字元
        /// </remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(AccountViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AccountViewModel>> Register([FromBody] RegisterRequest request)
        {
            var response = await _accountService.RegisterAsync(request);
            return Created($"api/accounts/{response.Id}", response);
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <remarks>
        /// 系統管理員：system_manager@test.com / Aa123456! \
        /// 一般用戶：general_user@test.com / Aa123456!
        /// </remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var response = await _accountService.LoginAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// 刷新 access token
        /// </summary>
        /// <remarks>
        /// 系統定義單一 access token 有效期限只有 5 分鐘，
        /// client 需向 api 端提供 refresh token 以取得新的 access token 來延續生命，
        /// 並保留此次更新 access token 時 api 回傳的 refresh token 作為下次刷新用\
        /// 單次登入狀態最久能維持一天，超過一天 refresh token 也會失效
        /// </remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var response = await _accountService.RefreshTokenAsync(request);
            return Ok(response);
        }
    }
}
