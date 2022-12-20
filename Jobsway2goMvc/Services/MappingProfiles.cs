using AutoMapper;
using Jobsway2goMvc.Models;
using Jobsway2goMvc.Models.ViewModel;

namespace Jobsway2goMvc.Services
{
    public class MappingProfiles :Profile
    {
        public MappingProfiles()
        {
            CreateMap<ApplicationUser, UserProfileViewModel>();
        }
    }
}
