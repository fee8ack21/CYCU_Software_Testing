using App.Common.Models;
using App.DAL;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Resource
{
    public class ResourceService : IResourceService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ResourceService(
            ApplicationDbContext context,
            IMapper mapper
            )
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GetResourcesResponse> GetResourcesAsync(GetResourcesRequest request)
        {
            var query = _context.Resources.OrderBy(e => e.CreateTime).AsNoTracking();
            return new GetResourcesResponse()
            {
                Count = await query.CountAsync(),
                Data = await query.ProjectTo<ResourceViewModel>(_mapper.ConfigurationProvider).Skip(request.Start).Take(request.RowsPerPage).ToListAsync()
            };
        }
    }
}
