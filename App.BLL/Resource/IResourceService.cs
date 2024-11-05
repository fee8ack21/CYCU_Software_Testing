using App.Common.Models;

namespace App.BLL.Resource
{
    public interface IResourceService
    {
        Task<GetResourcesResponse> GetResourcesAsync(GetResourcesRequest request);
    }
}
