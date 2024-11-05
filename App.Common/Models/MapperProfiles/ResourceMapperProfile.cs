using AutoMapper;

namespace App.Common.Models.MapperProfiles
{
	public class ResourceMapperProfile : Profile
	{
		public ResourceMapperProfile()
		{
			CreateMap<App.DAL.Entities.Resource, ResourceViewModel>();
		}
	}
}
