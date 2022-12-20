using AutoMapper;
using Jobsway2goMvc.Models;
using Jobsway2goMvc.Models.ViewModel;

namespace Jobsway2goMvc.Services
{
    public class UserProfileMapper :Profile
    {
        public UserProfileMapper()
        {
            CreateMap<ApplicationUser, UserProfileViewModel>();
        }
    }
}
