using App.Common.Configs;
using App.Common.Exceptions;
using App.Common.Models;
using App.DAL;
using App.DAL.Entities;
using App.DAL.Types;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace App.BLL.Account
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly JWTConfig _jwtConfig;
        private readonly ILogger<AccountService> _logger;
        private readonly string _backStageUrl;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext context,
            IMapper mapper,
            IOptions<JWTConfig> JWTConfig,
            ILogger<AccountService> logger,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _mapper = mapper;
            _jwtConfig = JWTConfig.Value;
            _logger = logger;
        }

        public async Task<GetAccountsResponse> GetAccountsAsync(GetAccountsRequest request)
        {
            var predicate = PredicateBuilder.New<ApplicationUser>(true);
            var roles = await _context.Roles.ToListAsync();
            var roleIds = request.QueryType == GetAccountsQueryType.MANAGER ? roles.Where(r => r.IsManager).Select(r => r.Id).ToList() :
                request.QueryType == GetAccountsQueryType.USER ? roles.Where(r => !r.IsManager).Select(r => r.Id).ToList() : roles.Select(r => r.Id).ToList();

            predicate.And(e => e.UserRoles.Any(ur => roleIds.Contains(ur.RoleId)));

            var query = _context.Users.Include(e => e.UserRoles).ThenInclude(e => e.Role).Where(predicate).AsNoTracking().OrderBy(e => e.CreateTime);
            return new GetAccountsResponse()
            {
                Count = await query.CountAsync(),
                Data = await query.ProjectTo<AccountViewModel>(_mapper.ConfigurationProvider).Skip(request.Start).Take(request.RowsPerPage).ToListAsync()
            };
        }

        public async Task<AccountViewModel> RegisterAsync(RegisterRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            // 新增用戶
            var user = _mapper.Map<ApplicationUser>(request);
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                // 可以通過比對 Error Code 做自訂處理
                if (result.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.DuplicateEmail)))
                {
                    throw new ConflictException("此信箱已被使用。");
                }
                if (result.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.DuplicateUserName)))
                {
                    throw new ConflictException("此用戶名稱已被使用。");
                }
                throw new BadRequestException(string.Join(" ", result.Errors.Select(e => e.Description)));
            }

            var userRole = await _context.Roles.FirstOrDefaultAsync(e => e.Name == UserRole.GeneralUser);
            if (userRole == null)
            {
                throw new InternalServerException("註冊失敗。");
            }

            user.UserRoles.Add(new ApplicationUserRole { RoleId = userRole.Id });
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            var response = _mapper.Map<AccountViewModel>(user);
            return response;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var response = new LoginResponse();
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                response.IsAuthenticated = false;
                response.Message = "無此帳號。";
                return response;
            }

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordCorrect)
            {
                response.IsAuthenticated = false;
                response.Message = "密碼錯誤。";
                return response;
            }

            response.AccessToken = await GenerateAccessTokenAsync(user);
            response.RefreshToken = GenerateRefreshToken();
            response.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenValidityInDays);
            response.IsAuthenticated = true;
            response.Email = user.Email;
            response.UserName = user.UserName;

            var roleNames = await _userManager.GetRolesAsync(user);
            response.Roles = roleNames.ToList();

            var refreshToken = new ApplicationUserRefreshToken
            {
                Token = response.RefreshToken,
                ExpiryTime = response.RefreshTokenExpiryTime,
            };
            user.RefreshTokens.Add(refreshToken);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return new LoginResponse { IsAuthenticated = false, Message = "登入失敗。" };
            }

            return response;
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var refreshToken = await _context.ApplicationUserRefreshTokens
                .Where(e => e.Token == request.RefreshToken && e.ExpiryTime <= DateTime.UtcNow)
                .Include(e => e.User)
                .FirstOrDefaultAsync();
            if (refreshToken == null)
            {
                throw new BadRequestException("此令牌已失效。");
            }

            refreshToken.Token = GenerateRefreshToken();
            await _context.SaveChangesAsync();

            var response = new RefreshTokenResponse { AccessToken = await GenerateAccessTokenAsync(refreshToken.User), RefreshToken = refreshToken.Token };
            return response;
        }

        /// <summary>
        /// 產出Access Token
        /// </summary>
        /// <param name="user">用戶Entity</param>
        /// <returns></returns>
        private async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roleNames = await _userManager.GetRolesAsync(user);
            var rolesClaims = roleNames.Select(e => new Claim(ClaimType.Roles, e));
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimType.UId, user.Id)
            }
            .Union(userClaims)
            .Union(rolesClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtConfig.AccessTokenDurationInMinutes),
                signingCredentials: signingCredentials);
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        /// <summary>
        /// 產出Refresh Token
        /// </summary>
        /// <returns></returns>
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
