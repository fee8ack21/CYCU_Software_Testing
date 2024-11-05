using App.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.PL.Controllers
{
    /// <summary>
    /// 受保護的資源
    /// </summary>

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/resources")]
    public class ResourceController : ControllerBase
    {
        private readonly App.BLL.Resource.IResourceService _resourceService;

        public ResourceController(App.BLL.Resource.IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        /// <summary>
        /// 取得受保護的資源列表
        /// </summary>
        /// <remarks>需先登入才允許呼叫，不限一般用戶或系統管理員身份</remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(GetResourcesResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GetResourcesResponse>> GetAccounts([FromQuery] GetResourcesRequest request)
        {
            var response = await _resourceService.GetResourcesAsync(request);
            return Ok(response);
        }
    }
}
