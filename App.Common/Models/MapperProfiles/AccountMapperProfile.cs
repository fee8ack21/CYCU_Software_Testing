using App.DAL.Entities;
using AutoMapper;

namespace App.Common.Models.MapperProfiles
{
	public class AccountMapperProfile : Profile
	{
		public AccountMapperProfile()
		{
			CreateMap<ApplicationUser, AccountViewModel>()
				.ForMember(to => to.RoleNames, opt => opt.MapFrom(from => from.UserRoles.Select(e => e.Role.Name)));
			CreateMap<RegisterRequest, ApplicationUser>();
		}
	}
}
